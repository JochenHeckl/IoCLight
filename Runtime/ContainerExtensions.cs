using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCLight
{ 
    public static class ContainerExtensions
    {
        public static TypeBindingBase As<LookupType>( this TypeBindingBase binding )
        {
            binding.LookupType = typeof( LookupType );
            return binding;
        }

        public static TypeBindingBase SingleInstance( this TypeBindingBase binding )
        {
            throw new NotImplementedException();
        }
    }
}
