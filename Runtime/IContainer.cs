using System;

namespace IoCLight
{
    public interface IContainer
    {
        InstanceType Resolve<InstanceType>();
        InstanceType Resolve<InstanceType>( Type typeofInstanceType );

        TypeBindingBase Register<InstanceType>();
        InstanceBinding<InstanceType> RegisterInstance<InstanceType>( InstanceType instance );
    }
}