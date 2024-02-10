using System;
using System.Linq;
using UnityEngine;

namespace IoCLight
{
    public class IoCBehaviour : MonoBehaviour
    {
        private IContainer container;

        protected TypeToResolve Resolve<TypeToResolve>()
            where TypeToResolve : class
        {
            if (container == null)
            {
                var bootstrapBase = GetComponentInParent<BootstrapBase>();

                if (bootstrapBase == null)
                {
                    Debug.LogWarning(
                        $"BootstrapBase not found in the ancestry of IoCBehaviour {name}. This can lead to problems if you are using more than one Bootstrapper in your scene."
                    );

                    // As a fallback we are searching the entire scene.
                    bootstrapBase = FindObjectsByType<BootstrapBase>(
                            FindObjectsInactive.Exclude,
                            FindObjectsSortMode.None
                        )
                        .FirstOrDefault();
                }

                container = bootstrapBase != null ? bootstrapBase.Container : null;
            }

            if (container == null)
            {
                throw new InvalidOperationException(
                    $"Failed to find inversion of control container to resolve {nameof(TypeToResolve)} for object {name} from hierarchy."
                );
            }

            return container.Resolve<TypeToResolve>();
        }
    }
}
