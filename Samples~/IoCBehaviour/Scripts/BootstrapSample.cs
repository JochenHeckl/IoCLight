namespace JH.IoCLight.Samples.PropertyInjection
{
    public class BootstrapSample : BootstrapBase
    {
        protected override void Compose()
        {
            Container.Register<SampleImplementation>().As<ISampleInterface>();
        }
    }
}
