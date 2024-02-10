using de.JochenHeckl.Unity.IoCLight;
using UnityEngine;

namespace IoCLight.Samples.PropertyInjection
{
    public class SampleIoCBehaviour : IoCBehaviour
    {
        private ISampleInterface Dependency { get; set; }

        private void Start()
        {
            Dependency = Resolve<ISampleInterface>();
            Debug.Log($"dependency is: {Dependency}.");
        }
    }
}
