using System;
using System.Linq;

namespace IoCLight
{
    public class TypeBindingBase : ITypeBinding
    {
        public Type LookupType { get; set; }
        public Type ResolveType { get; set; }

        public TypeToResolve Resolve<TypeToResolve>( IContainer container )
        {
            if( !typeof(TypeToResolve).IsAssignableFrom( LookupType ))
            {
                // we can not resolve this type
                throw new InvalidOperationException( 
                    $"{GetType().Name} can not resolve type {typeof(TypeToResolve).Name} because it is not assignable from {LookupType.Name}" );
            }

            // prefer constructors with more parameters for maximum resolve
            var constructors = ResolveType.GetConstructors().OrderByDescending( x => x.GetParameters().Count() );

            foreach( var constructor in constructors )
            {
                var resolvedParameters = constructor.GetParameters()
                    .Select( x => container.Resolve<object>( x.GetType() ) )
                    .ToArray();

                return (TypeToResolve) Activator.CreateInstance( ResolveType, resolvedParameters );
            }

            throw new InvalidOperationException($"failed to resolve constructor for type {ResolveType.Name}.");
        }
    }
}
