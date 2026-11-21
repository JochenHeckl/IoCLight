using UnityEngine;

namespace JH.IoCLight
{
  [AddComponentMenu(nameof(IoCBehaviour))]
  public class IoCBehaviour : MonoBehaviour
  {
    protected TypeToResolve Resolve<TypeToResolve>()
      where TypeToResolve : class
    {
      return this.FindContainer().Resolve<TypeToResolve>();
    }

    protected void ResolveInjectables()
    {
      this.ResolveInjectables(this.FindContainer());
    }
  }
}
