using System;

namespace IoCLight
{
    public interface ITypeBinding
    {
        Type LookupType { get; }
        Type ResolveType { get; }

        TypeToResolve Resolve<TypeToResolve>( IContainer container );
    }
}
