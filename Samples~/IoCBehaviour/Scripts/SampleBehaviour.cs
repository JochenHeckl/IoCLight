using UnityEngine;

namespace JH.IoCLight.Samples.PropertyInjection
{
    public class SampleBehaviour : IoCBehaviour
    {
        private ISampleInterface Dependency { get; set; }

        private void Start()
        {
            Dependency = Resolve<ISampleInterface>();

            if (Dependency != null)
            {
                Debug.Log("Dependency was resolved successfully.");
            }
            else
            {
                Debug.LogError("Dependency was NOT resolved successfully.");
            }
        }
    }
}
