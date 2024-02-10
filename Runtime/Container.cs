using System;
using System.Collections.Generic;
using System.Linq;

namespace de.JochenHeckl.Unity.IoCLight
{
    public class Container : IContainer
    {
        private readonly List<ITypeBinding> typeBindings = new List<ITypeBinding>();

        public Container()
        {
            RegisterInstance(this).As<IContainer>();
        }

        public ITypeBinding Register<InstanceType>()
        {
            var binding = new TypeBindingBase()
            {
                LookupType = typeof(InstanceType),
                ResolveType = typeof(InstanceType)
            };

            typeBindings.Add(binding);

            return binding;
        }

        public ITypeBinding RegisterInstance<InstanceType>(InstanceType instance)
        {
            var binding = new TypeBindingBase(instance)
            {
                SingleInstance = true,
                LookupType = typeof(InstanceType),
                ResolveType = typeof(InstanceType)
            };

            typeBindings.Add(binding);

            return binding;
        }

        public ITypeBinding RegisterFactory<ProductType>(Func<IContainer, ProductType> producer)
        {
            var binding = new FactoryBinding<ProductType>(producer)
            {
                SingleInstance = false,
                LookupType = typeof(ProductType)
            };

            typeBindings.Add(binding);

            return binding;
        }

        public InstanceType Resolve<InstanceType>()
            where InstanceType : class
        {
            var typeBinding = typeBindings.FirstOrDefault(x =>
                typeof(InstanceType).IsAssignableFrom(x.LookupType)
            );

            if (typeBinding == null)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve type {typeof(InstanceType)}."
                        + $" Make sure you registered  {typeof(InstanceType)} with your container?"
                );
            }

            return typeBinding.Resolve<InstanceType>(this);
        }

        public InstanceType[] ResolveAll<InstanceType>()
            where InstanceType : class
        {
            return typeBindings
                .Where(x => typeof(InstanceType).IsAssignableFrom(x.LookupType))
                .Select(x => x.Resolve<InstanceType>(this))
                .ToArray();
        }

        public object Resolve(Type typeOfInstanceType)
        {
            var typeBinding = typeBindings.FirstOrDefault(x =>
                typeOfInstanceType.IsAssignableFrom(x.LookupType)
            );

            if (typeBinding == null)
            {
                throw new InvalidOperationException(
                    $"{typeOfInstanceType.Name} can not be resolved."
                        + $" Make sure you registered {typeOfInstanceType} with your container?"
                );
            }

            return typeBinding.Resolve(this);
        }

        public void Terminate()
        {
            typeBindings.Clear();
        }
    }
}
