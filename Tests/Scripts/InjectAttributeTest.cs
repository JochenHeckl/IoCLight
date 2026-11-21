using System;
using NUnit.Framework;
using UnityEngine;

namespace JH.IoCLight.Test
{
    public class InjectAttributeTest
    {
        public interface ITestService
        {
            int Value { get; }
        }

        public class TestService : ITestService
        {
            public int Value => 42;
        }

        private class TestBootstrap : BootstrapBase
        {
            protected override void Compose()
            {
                Container.Register<TestService>().As<ITestService>();
            }

            protected override void Shutdown() { }
        }

        private class BehaviourWithPropertyInjection : IoCBehaviour
        {
            [Inject]
            public ITestService Service { get; private set; }

            public string NotInjected { get; set; } = "untouched";

            public void Inject() => ResolveInjectables();
        }

        private class BehaviourWithFieldInjection : IoCBehaviour
        {
            [Inject]
            private ITestService service;

            public ITestService Service => service;

            public void Inject() => ResolveInjectables();
        }

        private class BehaviourWithReadOnlyProperty : IoCBehaviour
        {
            [Inject]
            public ITestService Service { get { return null; } }

            public void Inject() => ResolveInjectables();
        }

        private class BehaviourWithReadOnlyField : IoCBehaviour
        {
            [Inject]
            private readonly ITestService service = null;

            public void Inject() => ResolveInjectables();
        }

        private GameObject bootstrapGO;

        [SetUp]
        public void SetUp()
        {
            bootstrapGO = new GameObject("Bootstrap");
            bootstrapGO.AddComponent<TestBootstrap>().Awake();
        }

        [TearDown]
        public void TearDown()
        {
            if (bootstrapGO != null)
                UnityEngine.Object.DestroyImmediate(bootstrapGO);
        }

        private T CreateChildBehaviour<T>()
            where T : MonoBehaviour
        {
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(bootstrapGO.transform);
            return go.AddComponent<T>();
        }

        [Test]
        public void TestInjectProperty()
        {
            var behaviour = CreateChildBehaviour<BehaviourWithPropertyInjection>();
            behaviour.Inject();

            Assert.IsNotNull(behaviour.Service);
            Assert.IsInstanceOf<TestService>(behaviour.Service);
            Assert.AreEqual(42, behaviour.Service.Value);
        }

        [Test]
        public void TestInjectField()
        {
            var behaviour = CreateChildBehaviour<BehaviourWithFieldInjection>();
            behaviour.Inject();

            Assert.IsNotNull(behaviour.Service);
            Assert.IsInstanceOf<TestService>(behaviour.Service);
        }

        [Test]
        public void TestNonAnnotatedPropertyNotTouched()
        {
            var behaviour = CreateChildBehaviour<BehaviourWithPropertyInjection>();
            behaviour.Inject();

            Assert.AreEqual("untouched", behaviour.NotInjected);
        }

        [Test]
        public void TestResolveInjectablesWithoutBootstrapThrows()
        {
            UnityEngine.Object.DestroyImmediate(bootstrapGO);
            bootstrapGO = null;

            var go = new GameObject("Orphan");

            try
            {
                var behaviour = go.AddComponent<BehaviourWithPropertyInjection>();
                Assert.Throws<InvalidOperationException>(() => behaviour.Inject());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void TestShutdownOverrideDoesNotSkipTermination()
        {
            var bootstrap = bootstrapGO.GetComponent<TestBootstrap>();
            var container = bootstrap.Container;
            Assert.IsNotNull(container.Resolve<ITestService>());

            bootstrap.OnDestroy();

            Assert.Throws<InvalidOperationException>(() => container.Resolve<ITestService>());
        }

        [Test]
        public void TestFindContainerRequiresBootstrapInAncestry()
        {
            var orphanGO = new GameObject("Orphan");
            try
            {
                var behaviour = orphanGO.AddComponent<BehaviourWithPropertyInjection>();

                var exception = Assert.Throws<InvalidOperationException>(() => behaviour.Inject());

                Assert.IsTrue(exception.Message.Contains("BootstrapBase exists"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(orphanGO);
            }
        }

        [Test]
        public void TestFindContainerRequiresAwakeBootstrap()
        {
            var bootstrapGO = new GameObject("BootstrapWithoutAwake");
            var behaviourGO = new GameObject("Behaviour");
            try
            {
                behaviourGO.transform.SetParent(bootstrapGO.transform);
                bootstrapGO.AddComponent<TestBootstrap>();
                var behaviour = behaviourGO.AddComponent<BehaviourWithPropertyInjection>();

                var exception = Assert.Throws<InvalidOperationException>(() => behaviour.Inject());

                Assert.IsTrue(exception.Message.Contains("Awake() has run"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(behaviourGO);
                UnityEngine.Object.DestroyImmediate(bootstrapGO);
            }
        }

        [Test]
        public void TestResolveInjectablesUsesCurrentContainer()
        {
            var firstBootstrapGO = new GameObject("BootstrapOne");
            firstBootstrapGO.AddComponent<TestBootstrap>().Awake();
            var behaviourGO = new GameObject("Behaviour");
            try
            {
                behaviourGO.transform.SetParent(firstBootstrapGO.transform);
                var behaviour = behaviourGO.AddComponent<BehaviourWithPropertyInjection>();
                behaviour.Inject();

                firstBootstrapGO.GetComponent<TestBootstrap>().OnDestroy();
                behaviourGO.transform.SetParent(null);

                var secondBootstrapGO = new GameObject("BootstrapTwo");
                try
                {
                    secondBootstrapGO.AddComponent<TestBootstrap>().Awake();
                    behaviourGO.transform.SetParent(secondBootstrapGO.transform);

                    Assert.DoesNotThrow(() => behaviour.Inject());
                    Assert.IsNotNull(behaviour.Service);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(secondBootstrapGO);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(behaviourGO);
                UnityEngine.Object.DestroyImmediate(firstBootstrapGO);
            }
        }

        [Test]
        public void TestReadOnlyInjectTargetsAreRejected()
        {
            var behaviour = CreateChildBehaviour<BehaviourWithReadOnlyProperty>();

            var exception = Assert.Throws<InvalidOperationException>(() => behaviour.Inject());

            Assert.IsTrue(exception.Message.Contains("must be writable"));
        }

        [Test]
        public void TestReadOnlyInjectFieldIsRejected()
        {
            var behaviour = CreateChildBehaviour<BehaviourWithReadOnlyField>();

            var exception = Assert.Throws<InvalidOperationException>(() => behaviour.Inject());

            Assert.IsTrue(exception.Message.Contains("must not be readonly or const"));
        }
    }
}
