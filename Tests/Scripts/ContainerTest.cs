using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace JH.IoCLight.Test
{
  public class ContainerTest
  {
    public class SimpleDefaultConstructor { };

    public class SimpleCustomConstructor
    {
      public SimpleCustomConstructor()
      {
        Debug.Log("We hit the custom constructor without arguments.");
      }
    };

    public interface ISimpleInterface
    {
      int SimpleProperty { get; }
    }

    public class SimpleType : ISimpleInterface
    {
      public int SimpleProperty
      {
        get { return 42; }
      }
    }

    public class AnOtherSimpleType : ISimpleInterface
    {
      public int SimpleProperty
      {
        get { return 21; }
      }
    }

    public class ASimpleContainerType
    {
      public ISimpleInterface SimpleProperty { get; set; }
    }

    public interface ISimpleTypeDependency
    {
      int SimpleProperty { get; }
    }

    public class SimpleTypeDependency : ISimpleTypeDependency
    {
      public int SimpleProperty
      {
        get { return 42; }
      }
    }

    public class SimpleTypeWithDependency : ISimpleInterface
    {
      public int SimpleProperty
      {
        get { return dependency.SimpleProperty; }
      }

      private ISimpleTypeDependency dependency;

      public SimpleTypeWithDependency(ISimpleTypeDependency dependencyIn)
      {
        dependency = dependencyIn;
      }
    }

    public interface IMissingDependency { }

    public class TypeWithFallbackConstructor : ISimpleInterface
    {
      public string ConstructorUsed { get; private set; }

      public TypeWithFallbackConstructor(IMissingDependency missing)
      {
        ConstructorUsed = "with dependency";
      }

      public TypeWithFallbackConstructor()
      {
        ConstructorUsed = "default";
      }

      public int SimpleProperty
      {
        get { return 42; }
      }
    }

    public interface ICircularA { }

    public interface ICircularB { }

    public class CircularA : ICircularA
    {
      public CircularA(ICircularB dependency) { }
    }

    public class CircularB : ICircularB
    {
      public CircularB(ICircularA dependency) { }
    }

    public interface IFallbackA { }

    public interface IFallbackB { }

    public class FallbackA : IFallbackA
    {
      public FallbackA(IFallbackB dependency) { }
    }

    public class FallbackB : IFallbackB
    {
      public FallbackB(IFallbackA dependency) { }

      public FallbackB() { }
    }

    [Test]
    public void TestSimpleResolve()
    {
      var container = new Container();

      container.Register<SimpleDefaultConstructor>();
      container.Register<SimpleCustomConstructor>();

      Assert.IsNotNull(container.Resolve<SimpleDefaultConstructor>());
      Assert.IsNotNull(container.Resolve<SimpleCustomConstructor>());
    }

    [Test]
    public void TestInheritanceResolve()
    {
      var container = new Container();

      container.Register<SimpleType>().As<ISimpleInterface>();

      Assert.IsNotNull(container.Resolve<ISimpleInterface>());
      Assert.IsTrue(container.Resolve<ISimpleInterface>() is SimpleType);
    }

    [Test]
    public void TestInheritanceResolveWithDependencies()
    {
      var container = new Container();

      container.Register<SimpleTypeWithDependency>().As<ISimpleInterface>();
      container.Register<SimpleTypeDependency>().As<ISimpleTypeDependency>();

      Assert.IsNotNull(container.Resolve<ISimpleInterface>());
      Assert.IsTrue(container.Resolve<ISimpleInterface>() is SimpleTypeWithDependency);
      Assert.AreEqual(container.Resolve<ISimpleInterface>().SimpleProperty, 42);
    }

    [Test]
    public void TestRegisterInstance()
    {
      var container = new Container();

      var simpleTypeInstance = new SimpleType();

      container.RegisterInstance(simpleTypeInstance).As<ISimpleInterface>();

      Assert.IsNotNull(container.Resolve<ISimpleInterface>());
      Assert.IsTrue(container.Resolve<ISimpleInterface>() is SimpleType);
      Assert.AreEqual(
        container.Resolve<ISimpleInterface>().GetHashCode(),
        simpleTypeInstance.GetHashCode()
      );
    }

    [Test]
    public void TestSingleInstance()
    {
      var container = new Container();

      container.Register<SimpleType>().As<ISimpleInterface>().SingleInstance();

      Assert.AreEqual(
        container.Resolve<ISimpleInterface>().GetHashCode(),
        container.Resolve<ISimpleInterface>().GetHashCode()
      );
    }

    [Test]
    public void TestMultipleInstances()
    {
      var container = new Container();

      container.Register<SimpleType>().As<ISimpleInterface>();
      container.Register<AnOtherSimpleType>().As<ISimpleInterface>();

      Assert.AreEqual(container.ResolveAll<ISimpleInterface>().Length, 2);
    }

    [Test]
    public void TestFactoryResolve()
    {
      var container = new Container();

      container.Register<SimpleType>().As<ISimpleInterface>();

      container.RegisterFactory(
        (x) => new ASimpleContainerType() { SimpleProperty = x.Resolve<ISimpleInterface>() }
      );

      var product = container.Resolve<ASimpleContainerType>();

      Assert.NotNull(product);
      Assert.NotNull(product.SimpleProperty);
    }

    [Test]
    public void TestRegisterFactoryRejectsNullProducer()
    {
      var container = new Container();

      Assert.Throws<ArgumentNullException>(() =>
        container.RegisterFactory<ASimpleContainerType>(null)
      );
    }

    [Test]
    public void TestFactoryProducerReceivesContainer()
    {
      var container = new Container();
      container.Register<SimpleType>().As<ISimpleInterface>();

      IContainer receivedContainer = null;
      container.RegisterFactory(c =>
      {
        receivedContainer = c;
        return new ASimpleContainerType() { SimpleProperty = c.Resolve<ISimpleInterface>() };
      });

      var product = container.Resolve<ASimpleContainerType>();

      Assert.AreSame(container, receivedContainer);
      Assert.NotNull(product.SimpleProperty);
    }

    [Test]
    public void TestSingleInstanceFactoryProducesOnce()
    {
      var container = new Container();
      var callCount = 0;

      container
        .RegisterFactory(c =>
        {
          callCount++;
          return new ASimpleContainerType();
        })
        .SingleInstance();

      container.Resolve<ASimpleContainerType>();
      container.Resolve<ASimpleContainerType>();

      Assert.AreEqual(1, callCount);
    }

    [Test]
    public void TestTerminateClearsSingleInstanceCache()
    {
      var container = new Container();
      var binding = container.Register<SimpleType>().As<ISimpleInterface>().SingleInstance();

      var first = binding.Resolve(container);
      container.Terminate();
      var second = binding.Resolve(container);

      Assert.AreNotSame(first, second);
    }

    [Test]
    public void TestRegisterInstanceRejectsNull()
    {
      var container = new Container();

      Assert.Throws<ArgumentNullException>(() =>
        container.RegisterInstance<ISimpleInterface>(null)
      );
    }

    [Test]
    public void TestConstructorFallsBackWhenDependencyMissing()
    {
      var container = new Container();

      container.Register<TypeWithFallbackConstructor>().As<ISimpleInterface>();

      var resolved = (TypeWithFallbackConstructor)container.Resolve<ISimpleInterface>();

      Assert.AreEqual("default", resolved.ConstructorUsed);
    }

    [Test]
    public void TestCircularDependencyThrows()
    {
      var container = new Container();

      container.Register<CircularA>().As<ICircularA>();
      container.Register<CircularB>().As<ICircularB>();

      var exception = Assert.Throws<CircularDependencyException>(() =>
        container.Resolve<ICircularA>()
      );

      Assert.IsTrue(exception.Message.Contains("Circular dependency"));
    }

    [Test]
    public void TestCircularDependencyIsNotSwallowedByConstructorFallback()
    {
      var container = new Container();

      container.Register<FallbackA>().As<IFallbackA>();
      container.Register<FallbackB>().As<IFallbackB>();

      Assert.Throws<CircularDependencyException>(() => container.Resolve<IFallbackA>());
    }

    [Test]
    public void TestConstructorFallsBackWhenNestedDependencyCannotBeResolved()
    {
      var container = new Container();

      container.Register<FallbackNestedWithMissingDependency>().As<IFallbackNested>();
      container.Register<FallbackOuter>();

      Assert.AreEqual("default", container.Resolve<FallbackOuter>().ConstructorUsed);
    }

    [Test]
    public void TestFactoryExceptionIsNotSwallowedByConstructorFallback()
    {
      var container = new Container();

      container.Register<TypeWithFallbackConstructor>().As<ISimpleInterface>();
      container.RegisterFactory<IMissingDependency>(c =>
        throw new InvalidOperationException("factory failed")
      );

      var exception = Assert.Throws<InvalidOperationException>(() =>
        container.Resolve<ISimpleInterface>()
      );

      Assert.AreEqual("factory failed", exception.Message);
    }

    [Test]
    public void TestValueTypeFactoryRunsProducer()
    {
      var container = new Container();
      var calls = 0;

      container.RegisterFactory<int>(c =>
      {
        calls++;
        return 42;
      });

      Assert.AreEqual(42, container.Resolve(typeof(int)));
      Assert.AreEqual(1, calls);
    }

    [Test]
    public void TestSingleInstanceValueTypeFactoryProducesOnce()
    {
      var container = new Container();
      var calls = 0;

      container
        .RegisterFactory<int>(c =>
        {
          calls++;
          return 7;
        })
        .SingleInstance();

      Assert.AreEqual(7, container.Resolve(typeof(int)));
      Assert.AreEqual(7, container.Resolve(typeof(int)));
      Assert.AreEqual(1, calls);
    }

    [Test]
    public void TestSingleInstanceFactoryCachesNullProduct()
    {
      var container = new Container();
      var calls = 0;

      container
        .RegisterFactory<ASimpleContainerType>(c =>
        {
          calls++;
          return null;
        })
        .SingleInstance();

      Assert.IsNull(container.Resolve<ASimpleContainerType>());
      Assert.IsNull(container.Resolve<ASimpleContainerType>());
      Assert.AreEqual(1, calls);
    }

    [Test]
    public void TestTerminateRunsBindingsInReverseRegistrationOrder()
    {
      var container = new Container();
      var log = new List<string>();

      AddRawBinding(container, new RecordingBinding("first", log));
      AddRawBinding(container, new RecordingBinding("second", log));
      AddRawBinding(container, new RecordingBinding("third", log));

      container.Terminate();

      Assert.That(log, Is.EqualTo(new[] { "third", "second", "first" }));
    }

    public interface IFallbackNested { }

    public class FallbackNestedWithMissingDependency : IFallbackNested
    {
      public FallbackNestedWithMissingDependency(IMissingDependency missing) { }
    }

    public class FallbackOuter
    {
      public string ConstructorUsed { get; private set; }

      public FallbackOuter(IFallbackNested nested)
      {
        ConstructorUsed = "with nested";
      }

      public FallbackOuter()
      {
        ConstructorUsed = "default";
      }
    }

    private sealed class RecordingBinding : ITypeBinding
    {
      private readonly string name;
      private readonly List<string> log;

      public RecordingBinding(string name, List<string> log)
      {
        this.name = name;
        this.log = log;
      }

      public bool SingleInstance { get; set; }

      public Type LookupType { get; set; } = typeof(object);

      public Type ResolveType => typeof(object);

      public object Resolve(IContainer container)
      {
        throw new NotSupportedException();
      }

      public TypeToResolve Resolve<TypeToResolve>(IContainer container)
        where TypeToResolve : class
      {
        throw new NotSupportedException();
      }

      public void Terminate()
      {
        log.Add(name);
      }
    }

    private static void AddRawBinding(Container container, ITypeBinding binding)
    {
      var field = typeof(Container).GetField(
        "typeBindings",
        BindingFlags.NonPublic | BindingFlags.Instance
      );

      ((List<ITypeBinding>)field.GetValue(container)).Add(binding);
    }
  }
}
