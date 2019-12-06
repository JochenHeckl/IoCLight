using UnityEngine;

namespace IoCLight
{
    public abstract class BootstrapBase : MonoBehaviour
    {
        public Container Container { get; private set; }

        public abstract void Compose( IContainer container );

        public virtual void Awake()
        {
            Container = new Container();

            Compose( Container );
        }

        public virtual void OnDestroy()
        {
            Container = null;
        }
    }
}