using System;
using System.Collections.Generic;
using System.Linq;

namespace JH.IoCLight
{
  public class Container : IContainer
  {
    private readonly List<ITypeBinding> typeBindings = new List<ITypeBinding>();
    private readonly List<ITypeBinding> resolutionStack = new List<ITypeBinding>();

    public Container()
    {
      RegisterInstance(this).As<IContainer>();
    }

    public ITypeBinding Register<InstanceType>()
    {
      var binding = new TypeBindingBase()
      {
        LookupType = typeof(InstanceType),
        ResolveType = typeof(InstanceType),
      };

      typeBindings.Add(binding);

      return binding;
    }

    public ITypeBinding RegisterInstance<InstanceType>(InstanceType instance)
    {
      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var binding = new TypeBindingBase(instance)
      {
        SingleInstance = true,
        LookupType = typeof(InstanceType),
        ResolveType = typeof(InstanceType),
      };

      typeBindings.Add(binding);

      return binding;
    }

    public ITypeBinding RegisterFactory<ProductType>(Func<IContainer, ProductType> producer)
    {
      var binding = new FactoryBinding<ProductType>(producer)
      {
        SingleInstance = false,
        LookupType = typeof(ProductType),
      };

      typeBindings.Add(binding);

      return binding;
    }

    private TResult ResolveTypeBinding<TResult>(
      ITypeBinding typeBinding,
      Type requestedType,
      Func<ITypeBinding, TResult> resolve
    )
    {
      if (resolutionStack.Contains(typeBinding))
      {
        var chain = string.Join(" -> ", resolutionStack.Select(x => x.LookupType.Name));
        throw new CircularDependencyException(
          $"Circular dependency detected while resolving {requestedType.Name}:"
            + $" {chain} -> {requestedType.Name}."
        );
      }

      resolutionStack.Add(typeBinding);
      try
      {
        return resolve(typeBinding);
      }
      finally
      {
        resolutionStack.RemoveAt(resolutionStack.Count - 1);
      }
    }

    public InstanceType Resolve<InstanceType>()
      where InstanceType : class
    {
      var typeBinding = typeBindings.FirstOrDefault(x =>
        typeof(InstanceType).IsAssignableFrom(x.LookupType)
      );

      if (typeBinding == null)
      {
        throw new ResolutionFailedException(
          $"Failed to resolve type {typeof(InstanceType)}."
            + $" Make sure you registered  {typeof(InstanceType)} with your container?"
        );
      }

      return ResolveTypeBinding(
        typeBinding,
        typeof(InstanceType),
        binding => binding.Resolve<InstanceType>(this)
      );
    }

    public InstanceType[] ResolveAll<InstanceType>()
      where InstanceType : class
    {
      return typeBindings
        .Where(x => typeof(InstanceType).IsAssignableFrom(x.LookupType))
        .Select(x =>
          ResolveTypeBinding(
            x,
            typeof(InstanceType),
            binding => binding.Resolve<InstanceType>(this)
          )
        )
        .ToArray();
    }

    public object Resolve(Type typeOfInstanceType)
    {
      var typeBinding = typeBindings.FirstOrDefault(x =>
        typeOfInstanceType.IsAssignableFrom(x.LookupType)
      );

      if (typeBinding == null)
      {
        throw new ResolutionFailedException(
          $"{typeOfInstanceType.Name} can not be resolved."
            + $" Make sure you registered {typeOfInstanceType} with your container?"
        );
      }

      return ResolveTypeBinding(typeBinding, typeOfInstanceType, binding => binding.Resolve(this));
    }

    public void Terminate()
    {
      resolutionStack.Clear();

      for (var index = typeBindings.Count - 1; index >= 0; index--)
      {
        typeBindings[index].Terminate();
      }

      typeBindings.Clear();
    }
  }
}
