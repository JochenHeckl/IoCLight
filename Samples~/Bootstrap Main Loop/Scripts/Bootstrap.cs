using System.Linq;
using JH.IoCLight;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    public class Bootstrap : BootstrapBase
    {
        public TextAsset configuration;

        protected override void Compose()
        {
            Container.RegisterInstance(ParseConfiguration(configuration.text)).As<IConfiguration>();
            Container.Register<RuntimeData>().SingleInstance();
            Container.Register<SystemOne>().As<ISystem>().SingleInstance();
            Container.Register<SystemTwo>().As<ISystem>().SingleInstance();
        }

        private void Start()
        {
            foreach (var system in Container.ResolveAll<ISystem>().Where(system => system.Autostart))
            {
                system.Start();
            }
        }

        private void FixedUpdate()
        {
            foreach (var system in Container.ResolveAll<ISystem>())
            {
                system.Update();
            }
        }

        protected override void Shutdown()
        {
            foreach (var system in Container.ResolveAll<ISystem>())
            {
                system.Stop();
            }
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
