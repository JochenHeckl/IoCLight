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
      public override void Compose()
      {
        Container.Register<TestService>().As<ITestService>();
      }
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
  }
}
