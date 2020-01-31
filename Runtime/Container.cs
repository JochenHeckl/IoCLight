using System;
using System.Collections.Generic;
using System.Linq;

namespace de.JochenHeckl.IoCLight
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

        public ITypeBinding RegisterInstance<InstanceType>( InstanceType instance )
        {
            var binding = new TypeBindingBase( instance )
            {
                SingleInstance = true,
                LookupType = typeof( InstanceType ),
                ResolveType = typeof( InstanceType )
            };

            typeBindings.Add( binding );

            return binding;
        }

        public InstanceType Resolve<InstanceType>() where InstanceType : class
        {
            return typeBindings.First( x => typeof( InstanceType ).IsAssignableFrom( x.LookupType ) ).Resolve<InstanceType>( this );
        }

        public InstanceType[] ResolveAll<InstanceType>() where InstanceType : class
        {
            return typeBindings
                .Where( x => typeof( InstanceType ).IsAssignableFrom( x.LookupType ) )
                .Select( x => x.Resolve<InstanceType>( this ) )
                .ToArray();
        }

        public object Resolve( Type typeofInstanceType )
        {
            return typeBindings.First( x => typeofInstanceType.IsAssignableFrom( x.LookupType ) ).Resolve( this );
        }

        public void Terminate()
        {
            typeBindings.Clear();
        }
    }
}
