using System;

namespace de.JochenHeckl.Unity.IoCLight
{
    public interface IContainer
    {
        void Terminate();

        TypeBindingBase Register<InstanceType>();
        ITypeBinding RegisterInstance<InstanceType>( InstanceType instance );

        InstanceType Resolve<InstanceType>() where InstanceType : class;
        InstanceType[] ResolveAll<InstanceType>() where InstanceType : class;

        object Resolve( Type typeofInstanceType );
    }
}