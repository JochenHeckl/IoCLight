using System;

namespace IoCLight
{
    public interface IContainer
    {
        void Terminate();

        TypeBindingBase Register<InstanceType>();
        ITypeBinding RegisterInstance<InstanceType>( InstanceType instance );

        InstanceType Resolve<InstanceType>() where InstanceType : class;
        object Resolve( Type typeofInstanceType );
    }
}