using System;
using System.Collections.Generic;
using System.Linq;

namespace JH.IoCLight
{
    public class TypeBindingBase : ITypeBinding
    {
        public bool SingleInstance { get; set; }
        public Type LookupType { get; set; }
        public Type ResolveType { get; set; }

        private object instance;

        public TypeBindingBase() { }

        public TypeBindingBase(object instanceIn)
        {
            instance = instanceIn;
        }

        public object Resolve(IContainer container)
        {
            if (instance != null)
            {
                return instance;
            }

            // prefer constructors with more parameters for maximum resolve
            var constructors = ResolveType
                .GetConstructors()
                .OrderByDescending(x => x.GetParameters().Count())
                .ToArray();

            var failures = new List<string>();

            foreach (var constructor in constructors)
            {
                object[] resolvedParameters;

                try
                {
                    resolvedParameters = constructor
                        .GetParameters()
                        .Select(x => container.Resolve(x.ParameterType))
                        .ToArray();
                }
                catch (CircularDependencyException)
                {
                    throw;
                }
                catch (InvalidOperationException exception)
                {
                    failures.Add(exception.Message);
                    continue;
                }

                var resolvedInstance = Activator.CreateInstance(ResolveType, resolvedParameters);

                if (SingleInstance)
                {
                    instance = resolvedInstance;
                }

                return resolvedInstance;
            }

            throw new InvalidOperationException(
                $"failed to resolve constructor for type {ResolveType.Name}."
                    + (failures.Count > 0
                        ? $" Resolution failures: {string.Join(" | ", failures)}"
                        : string.Empty)
            );
        }

        public TypeToResolve Resolve<TypeToResolve>(IContainer container)
            where TypeToResolve : class
        {
            if (!typeof(TypeToResolve).IsAssignableFrom(LookupType))
            {
                // we can not resolve this type
                throw new InvalidOperationException(
                    $"{GetType().Name} can not resolve type {typeof(TypeToResolve).Name} because it is not assignable from {LookupType.Name}"
                );
            }

            return (TypeToResolve)Resolve(container);
        }

        public void Terminate()
        {
            instance = null;
        }
    }
}
