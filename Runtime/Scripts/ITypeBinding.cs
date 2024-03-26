using System;

namespace JH.IoCLight
{
    public interface ITypeBinding
    {
        bool SingleInstance { get; set; }
        Type LookupType { get; set; }
        Type ResolveType { get; }

        object Resolve(IContainer container);
        TypeToResolve Resolve<TypeToResolve>(IContainer container)
            where TypeToResolve : class;
    }
}
