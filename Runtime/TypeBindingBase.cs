using System;
using System.Linq;

namespace IoCLight
{
    public class TypeBindingBase : ITypeBinding
    {
        public bool SingleInstance { get; set; }
        public Type LookupType { get; set; }
        public Type ResolveType { get; set; }

        private object instance;

        public TypeBindingBase()
        {

        }

        public TypeBindingBase( object instanceIn )
        {
            instance = instanceIn;
        }

        public object Resolve( IContainer container )
        {
            if (instance != null)
            {
                return instance;
            }

            // prefer constructors with more parameters for maximum resolve
            var constructors = ResolveType.GetConstructors().OrderByDescending( x => x.GetParameters().Count() ).ToArray();

            foreach (var constructor in constructors)
            {
                var resolvedParameters = constructor.GetParameters()
                    .Select( x => container.Resolve( x.ParameterType ) )
                    .ToArray();

                if (SingleInstance)
                {
                    instance = Activator.CreateInstance( ResolveType, resolvedParameters );

                    return instance;
                }
                else
                {
                    return Activator.CreateInstance( ResolveType, resolvedParameters );
                }
            }

            throw new InvalidOperationException( $"failed to resolve constructor for type {ResolveType.Name}." );
        }

        public TypeToResolve Resolve<TypeToResolve>( IContainer container ) where TypeToResolve : class
        {
            if (!typeof( TypeToResolve ).IsAssignableFrom( LookupType ))
            {
                // we can not resolve this type
                throw new InvalidOperationException(
                    $"{GetType().Name} can not resolve type {typeof( TypeToResolve ).Name} because it is not assignable from {LookupType.Name}" );
            }

            return (TypeToResolve) Resolve( container );
        }
    }
}
