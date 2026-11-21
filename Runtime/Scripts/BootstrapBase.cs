using UnityEngine;

namespace JH.IoCLight
{
    public abstract class BootstrapBase : MonoBehaviour
    {
        public IContainer Container { get; private set; }

        protected abstract void Compose();

        protected virtual void Shutdown()
        {
            Container?.Terminate();
            Container = null;
        }

        public virtual void Awake()
        {
            Container = new Container();

            Compose();
        }

        public virtual void OnDestroy()
        {
            Shutdown();
        }
    }
}
