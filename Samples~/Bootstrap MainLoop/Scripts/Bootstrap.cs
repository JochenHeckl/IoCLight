using System;
using System.Collections;
using System.Collections.Generic;
using IoCLight;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    public class Bootstrap : BootstrapBase
    {
        public TextAsset configuration;

        private MainLoop mainLoop;

        public override void Compose()
        {
            Container.RegisterInstance( ParseConfiguration( configuration.text ) ).As<IConfiguration>();
            Container.Register<RuntimeData>().As<IRuntimeData>().SingleInstance();

            Container.Register<TimeLapseConstantTime>().As<ITimeLapse>();
            Container.Register<MainLoop>();
        }

        private Configuration ParseConfiguration( string configuration )
        {
            // insert code to parse your configuration.
            // For Example:
            //
            // return JsonConvert.Deserialize<Configuration>( text )

            return new Configuration()
            {
                TimeScaleFactor = 1f
            };
        }

        void Start()
        {
            mainLoop = Container.Resolve<MainLoop>();
        }

        // Update is called once per frame
        void Update()
        {
            mainLoop.UpdateSimulation( Time.deltaTime );
        }
    }
}