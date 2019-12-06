# Getting Started
- Open a new scene.
- Create an empty GameObject and rename it Bootstrap
- Create a new script named ExampleBootstrap. Derive your script from BootstrapBase.

```cs 

public interface ISimpleInterface
{
	int SimpleProperty { get; }
}

public interface ISimpleTypeDependency
{
	int SimpleProperty { get; }
}

public class SimpleTypeDependency : ISimpleTypeDependency
{
	public int SimpleProperty { get { return 42; } }
}

public class SimpleType : ISimpleInterface
{
	public int SimpleProperty { get { return dependency.SimpleProperty; }; }

	private ISimpleTypeDependency dependency;
	
	public SimpleType( ISimpleTypeDependency dependencyIn )
	{
		dependency = dependencyIn;
	}
}

public class ExampleBootstrap : BootstrapBase
{
    override void Compose( IContainer container )
    {
        container.Register<SimpleTypeDependency>().As<ISimpleTypeDependency>();
		container.Register<SimpleType>().As<ISimpleInterface>();
    }

    public void Start()
    {
        var simpleValue = Container.Resolve<ISimpleInterface>().SimpleProperty;
    }
}
```
