using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JH.IoCLight
{
    public static class MonoBehaviourIoCExtensions
    {
        private static readonly Dictionary<Type, MemberInfo[]> injectableMembers =
            new Dictionary<Type, MemberInfo[]>();

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
                throw new InvalidOperationException(
                    $"Failed to find an IoC container for {behaviour.name}. "
                        + "Make sure a BootstrapBase exists on this GameObject or one of its parents."
                );
            }

            if (bootstrap.Container == null)
            {
                throw new InvalidOperationException(
                    $"The BootstrapBase for {behaviour.name} has no container. "
                        + "Make sure its Awake() has run before resolving dependencies."
                );
            }

            return bootstrap.Container;
        }

        public static void ResolveInjectables(this MonoBehaviour behaviour, IContainer container)
        {
            if (behaviour == null)
            {
                throw new ArgumentNullException(nameof(behaviour));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            foreach (var injectable in GetInjectableMembers(behaviour.GetType()))
            {
                Inject(injectable, behaviour, container);
            }
        }

        private static void Inject(MemberInfo member, MonoBehaviour behaviour, IContainer container)
        {
            if (member is PropertyInfo property)
            {
                property.SetValue(behaviour, container.Resolve(property.PropertyType));
                return;
            }

            var field = (FieldInfo)member;
            field.SetValue(behaviour, container.Resolve(field.FieldType));
        }

        private static MemberInfo[] GetInjectableMembers(Type behaviourType)
        {
            if (injectableMembers.TryGetValue(behaviourType, out var members))
            {
                return members;
            }

            members = behaviourType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<InjectAttribute>() != null)
                .Cast<MemberInfo>()
                .Concat(
                    behaviourType
                        .GetFields(
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                        )
                        .Where(x => x.GetCustomAttribute<InjectAttribute>() != null)
                )
                .ToArray();

            foreach (var member in members)
            {
                if (member is PropertyInfo property && !property.CanWrite)
                {
                    throw new InvalidOperationException(
                        $"[Inject] property {property.Name} on {behaviourType.Name} must be writable."
                    );
                }

                if (member is FieldInfo field && (field.IsInitOnly || field.IsLiteral))
                {
                    throw new InvalidOperationException(
                        $"[Inject] field {field.Name} on {behaviourType.Name} must not be readonly or const."
                    );
                }
            }

            injectableMembers.Add(behaviourType, members);
            return members;
        }
    }
}
