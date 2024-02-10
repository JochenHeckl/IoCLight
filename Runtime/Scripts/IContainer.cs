using System;

namespace de.JochenHeckl.Unity.IoCLight
{
	public interface IContainer
    {
        void Terminate();

		ITypeBinding Register<InstanceType>();
        ITypeBinding RegisterInstance<InstanceType>( InstanceType instance );

        ITypeBinding RegisterFactory<ProductType>( Func<IContainer, ProductType> producer );

		InstanceType Resolve<InstanceType>() where InstanceType : class;
        InstanceType[] ResolveAll<InstanceType>() where InstanceType : class;

        object Resolve( Type typeofInstanceType );
    }
}