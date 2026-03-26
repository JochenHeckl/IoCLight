using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JH.IoCLight
{
  public static class MonoBehaviourIoCExtensions
  {
    public static TypeToResolve Resolve<TypeToResolve>(this MonoBehaviour behaviour)
      where TypeToResolve : class
    {
      return FindContainer(behaviour).Resolve<TypeToResolve>();
    }

    public static void ResolveInjectables(this MonoBehaviour behaviour)
    {
      ResolveInjectables(behaviour, FindContainer(behaviour));
    }

    public static IContainer FindContainer(this MonoBehaviour behaviour)
    {
      var bootstrap = behaviour.GetComponentInParent<BootstrapBase>();

      if (bootstrap == null)
      {
        Debug.LogWarning(
          $"BootstrapBase not found in the ancestry of {behaviour.name}. "
            + "This can lead to problems if you are using more than one Bootstrapper in your scene."
        );

        bootstrap = UnityEngine
          .Object.FindObjectsByType<BootstrapBase>(FindObjectsInactive.Exclude)
          .FirstOrDefault();
      }

      if (bootstrap == null)
      {
        throw new InvalidOperationException(
          $"Failed to find an IoC container for {behaviour.name}. "
            + "Make sure a BootstrapBase exists in the scene."
        );
      }

      return bootstrap.Container;
    }

    public static void ResolveInjectables(this MonoBehaviour behaviour, IContainer container)
    {
      var propertiesToInject = behaviour
        .GetType()
        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(x => x.GetCustomAttribute<InjectAttribute>() != null);

      foreach (var injectable in propertiesToInject)
      {
        injectable.SetValue(behaviour, container.Resolve(injectable.PropertyType));
      }

      var fieldsToInject = behaviour
        .GetType()
        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(x => x.GetCustomAttribute<InjectAttribute>() != null);

      foreach (var injectable in fieldsToInject)
      {
        injectable.SetValue(behaviour, container.Resolve(injectable.FieldType));
      }
    }
  }
}
