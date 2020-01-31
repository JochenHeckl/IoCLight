using NUnit.Framework;
using UnityEngine;


namespace de.JochenHeckl.IoCLight.Test
{
    public class ContainerTest
    {
        public class SimpleDefaultConstructor {};
        
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
            public int SimpleProperty { get { return 42; } }
        }

        public class AnOtherSimpleType : ISimpleInterface
        {
            public int SimpleProperty { get { return 21; } }
        }

        public interface ISimpleTypeDependency
        {
            int SimpleProperty { get; }
        }

        public class SimpleTypeDependency : ISimpleTypeDependency
        {
            public int SimpleProperty { get { return 42; } }
        }

        public class SimpleTypeWithDependency : ISimpleInterface
        {
            public int SimpleProperty { get { return dependency.SimpleProperty; } }

            private ISimpleTypeDependency dependency;

            public SimpleTypeWithDependency( ISimpleTypeDependency dependencyIn )
            {
                dependency = dependencyIn;
            }
        }

        [Test]
        public void TestSimpleResolve()
        {
            var container = new Container();

            container.Register<SimpleDefaultConstructor>();
            container.Register<SimpleCustomConstructor>();

            Assert.IsNotNull( container.Resolve<SimpleDefaultConstructor>() );
            Assert.IsNotNull( container.Resolve<SimpleCustomConstructor>() );
        }


        [Test]
        public void TestInheritanceResolve()
        {
            var container = new Container();

            container.Register<SimpleType>().As<ISimpleInterface>();            
            
            Assert.IsNotNull( container.Resolve<ISimpleInterface>() );
            Assert.IsTrue( container.Resolve<ISimpleInterface>() is SimpleType );
        }

        [Test]
        public void TestInheritanceResolveWithDependencies()
        {
            var container = new Container();

            container.Register<SimpleTypeWithDependency>().As<ISimpleInterface>();
            container.Register<SimpleTypeDependency>().As<ISimpleTypeDependency>();

            Assert.IsNotNull( container.Resolve<ISimpleInterface>() );
            Assert.IsTrue( container.Resolve<ISimpleInterface>() is SimpleTypeWithDependency );
            Assert.AreEqual( container.Resolve<ISimpleInterface>().SimpleProperty, 42 );
        }

        [Test]
        public void TestRegisterInstance()
        {
            var container = new Container();

            var simpleTypeInstance = new SimpleType();

            container.RegisterInstance( simpleTypeInstance ).As<ISimpleInterface>();

            Assert.IsNotNull( container.Resolve<ISimpleInterface>() );
            Assert.IsTrue( container.Resolve<ISimpleInterface>() is SimpleType );
            Assert.AreEqual( container.Resolve<ISimpleInterface>().GetHashCode() , simpleTypeInstance.GetHashCode() );
        }

        [Test]
        public void TestSingleInstance()
        {
            var container = new Container();

            container.Register<SimpleType>().As<ISimpleInterface>().SingleInstance();

            Assert.AreEqual(
                container.Resolve<ISimpleInterface>().GetHashCode(),
                container.Resolve<ISimpleInterface>().GetHashCode() );
        }


        [Test]
        public void TestMultipleInstances()
        {
            var container = new Container();

            container.Register<SimpleType>().As<ISimpleInterface>();
            container.Register<AnOtherSimpleType>().As<ISimpleInterface>();

            Assert.AreEqual( container.ResolveAll<ISimpleInterface>().Length, 2 );
        }
    }
}
