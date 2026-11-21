using System;

namespace JH.IoCLight
{
    public class FactoryBinding<ProductType> : ITypeBinding
    {
        private readonly Func<IContainer, ProductType> producer;
        private ProductType instance;

        public FactoryBinding(Func<IContainer, ProductType> producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

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

            var product = producer(container);

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

        public void Terminate()
        {
            instance = default;
        }
    }
}
