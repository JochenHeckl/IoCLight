using UnityEngine;

namespace JH.IoCLight
{
  public abstract class BootstrapBase : MonoBehaviour
  {
    public IContainer Container { get; private set; }

    protected abstract void Compose();

    protected virtual void Shutdown() { }

    public virtual void Awake()
    {
      Container = new Container();

      Compose();
    }

    public virtual void OnDestroy()
    {
      if (Container == null)
      {
        return;
      }

      try
      {
        Shutdown();
      }
      finally
      {
        Container.Terminate();
        Container = null;
      }
    }
  }
}
