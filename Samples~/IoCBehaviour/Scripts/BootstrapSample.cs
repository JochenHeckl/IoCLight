namespace IoCLight.Samples.IoCBehaviour
{
  public class BootstrapSample : BootstrapBase
  {
    protected override void Compose()
    {
      Container.Register<SampleImplementation>().As<ISampleInterface>();
    }
  }
}
