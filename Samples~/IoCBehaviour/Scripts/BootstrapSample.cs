using IoCLight;
using UnityEngine;

namespace IoCLight.Samples.PropertyInjection
{
    public class BootstrapSample : BootstrapBase
    {
        public override void Compose()
        {
            Container.Register<SampleImplementation>().As<ISampleInterface>();
        }
    }
}
