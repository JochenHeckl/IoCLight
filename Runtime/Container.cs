using System;
using System.Collections.Generic;
using System.Linq;

namespace IoCLight
{
    public class Container : IContainer
    {
        private readonly List<ITypeBinding> typeBindings = new List<ITypeBinding>();

        public TypeBindingBase Register<InstanceType>()
        {
            var binding = new TypeBindingBase()
            {
                LookupType = typeof( InstanceType ),
                ResolveType = typeof( InstanceType )
            };

            typeBindings.Add( binding );

            return binding;
        }

        public InstanceBinding<InstanceType> RegisterInstance<InstanceType>( InstanceType instance )
        {
            return new InstanceBinding<InstanceType>( instance );
        }

        public InstanceType Resolve<InstanceType>()
        {
            return typeBindings.First( x => typeof( InstanceType ).IsAssignableFrom( x.LookupType ) ).Resolve<InstanceType>( this );
        }

        public InstanceType Resolve<InstanceType>( Type typeofInstanceType )
        {
            return Resolve<InstanceType>();
        }
    }
}
