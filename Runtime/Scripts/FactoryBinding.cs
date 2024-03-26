using System;
using System.Linq;
using System.Reflection;

namespace JH.IoCLight
{
    public class FactoryBinding<ProductType> : ITypeBinding
    {
        Func<IContainer, ProductType> producer;
        private ProductType instance;

        public FactoryBinding(Func<IContainer, ProductType> producer)
        {
            this.producer = producer;
        }

        public bool SingleInstance { get; set; }
        public Type LookupType { get; set; }
        public Type ResolveType => typeof(ProductType);

        public object Resolve(IContainer container)
        {
            if (instance != null)
            {
                return instance;
            }

            var methodInfo = producer.GetMethodInfo();

            if (!typeof(ProductType).IsAssignableFrom(methodInfo.ReturnType))
            {
                throw new InvalidOperationException(
                    $"Invalid producer supplied to factory binding. {typeof(ProductType).Name} can not be assigned from {methodInfo.ReturnType.Name}."
                );
            }

            var resolvedParameters = methodInfo
                .GetParameters()
                .Select(x => container.Resolve(x.ParameterType))
                .ToArray();

            var product = (ProductType)producer.DynamicInvoke(resolvedParameters);

            if (SingleInstance)
            {
                instance = product;
            }

            return product;
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
    }
}
