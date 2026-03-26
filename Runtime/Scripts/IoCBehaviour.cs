using UnityEngine;

namespace JH.IoCLight
{
  [AddComponentMenu(nameof(IoCBehaviour))]
  public class IoCBehaviour : MonoBehaviour
  {
    private IContainer container;

    protected TypeToResolve Resolve<TypeToResolve>()
      where TypeToResolve : class
    {
      container ??= this.FindContainer();
      return container.Resolve<TypeToResolve>();
    }

    protected void ResolveInjectables()
    {
      container ??= this.FindContainer();
      this.ResolveInjectables(container);
    }
  }
}
