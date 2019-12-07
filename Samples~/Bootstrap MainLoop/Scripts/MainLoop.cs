using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    internal class MainLoop
    {
        private IConfiguration configuration;
        private IRuntimeData runtimeData;
        private ITimeLapse timeLapse;

        public MainLoop( IConfiguration configurationIn, IRuntimeData runtimeDataIn, ITimeLapse timeLapseIn )
        {
            configuration = configurationIn;
            runtimeData = runtimeDataIn;
            timeLapse = timeLapseIn;
        }

        public void UpdateSimulation( float deltaTimeSec )
        {
            runtimeData.SimulationTimeSec += timeLapse.ToSimulationTime( deltaTimeSec );
        }
    }
}