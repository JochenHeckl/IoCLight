using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.IoCLight
{ 
    public static class ContainerExtensions
    {
        public static ITypeBinding As<LookupType>( this ITypeBinding binding )
        {
            binding.LookupType = typeof( LookupType );
            return binding;
        }

        public static ITypeBinding SingleInstance( this ITypeBinding binding )
        {
            binding.SingleInstance = true;
            return binding;
        }
    }
}
