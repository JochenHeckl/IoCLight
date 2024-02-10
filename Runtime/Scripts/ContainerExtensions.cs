using System;

namespace IoCLight
{
	public static class ContainerExtensions
	{
		public static ITypeBinding As<LookupType>( this ITypeBinding binding )
		{
			if ( !typeof( LookupType ).IsAssignableFrom( binding.ResolveType ) )
			{
				throw new InvalidOperationException( $"{binding.ResolveType.Name} can not be registered as {typeof( LookupType ).Name}." +
					$" Did you forget to declare an interface dependency to {typeof( LookupType ).Name}?" );
			}

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
