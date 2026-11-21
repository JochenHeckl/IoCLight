using System;
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

        public interface IMissingDependency
        {
        }

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

        public interface ICircularA
        {
        }

        public interface ICircularB
        {
        }

        public class CircularA : ICircularA
        {
            public CircularA(ICircularB dependency)
            {
            }
        }

        public class CircularB : ICircularB
        {
            public CircularB(ICircularA dependency)
            {
            }
        }

        public interface IFallbackA
        {
        }

        public interface IFallbackB
        {
        }

        public class FallbackA : IFallbackA
        {
            public FallbackA(IFallbackB dependency)
            {
            }
        }

        public class FallbackB : IFallbackB
        {
            public FallbackB(IFallbackA dependency)
            {
            }

            public FallbackB()
            {
            }
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
        public void TestTerminateClearsSingleInstanceCache()
        {
            var container = new Container();
            var binding = container
                .Register<SimpleType>()
                .As<ISimpleInterface>()
                .SingleInstance();

            var first = binding.Resolve(container);
            container.Terminate();
            var second = binding.Resolve(container);

            Assert.AreNotSame(first, second);
        }

        [Test]
        public void TestRegisterInstanceRejectsNull()
        {
            var container = new Container();

            Assert.Throws<ArgumentNullException>(
                () => container.RegisterInstance<ISimpleInterface>(null)
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

            var exception = Assert.Throws<InvalidOperationException>(
                () => container.Resolve<ICircularA>()
            );

            Assert.IsTrue(exception.Message.Contains("Circular dependency"));
        }

        [Test]
        public void TestCircularDependencyFallsBackToParameterlessConstructor()
        {
            var container = new Container();

            container.Register<FallbackA>().As<IFallbackA>();
            container.Register<FallbackB>().As<IFallbackB>();

            Assert.IsNotNull(container.Resolve<IFallbackA>());
        }
    }
}
