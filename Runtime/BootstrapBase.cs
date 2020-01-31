using UnityEngine;

namespace de.JochenHeckl.IoCLight
{
    public abstract class BootstrapBase : MonoBehaviour
    {
        public Container Container { get; private set; }

        public abstract void Compose();

        public virtual void Awake()
        {
            Container = new Container();
            Compose();
        }

        public virtual void OnDestroy()
        {
            Container.Terminate();
            Container = null;
        }
    }
}