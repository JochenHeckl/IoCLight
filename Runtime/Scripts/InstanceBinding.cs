namespace IoCLight
{
    public class InstanceBinding<InstanceType> : TypeBindingBase
    {
        public InstanceType boundInstance;
        
        public InstanceBinding( InstanceType boundInstanceIn )
        {
            boundInstance = boundInstanceIn;

            LookupType = typeof( InstanceType );
            ResolveType = boundInstanceIn.GetType();
        }
    }
}