using JH.IoCLight;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    public class Bootstrap : BootstrapBase
    {
        public TextAsset configuration;

        public override void Compose()
        {
            Container.RegisterInstance(ParseConfiguration(configuration.text)).As<IConfiguration>();
            Container.Register<RuntimeData>().As<IRuntimeData>().SingleInstance();
        }

        private Configuration ParseConfiguration(string configuration)
        {
            // insert code to parse your configuration.
            // For Example:
            //
            // return JsonConvert.Deserialize<Configuration>( text )

            return new Configuration();
        }
    }
}
