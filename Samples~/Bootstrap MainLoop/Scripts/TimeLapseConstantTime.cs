using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceMeWithYourProjectsNamespace
{
    internal class TimeLapseConstantTime : ITimeLapse
    {
        private IConfiguration configuration;

        public TimeLapseConstantTime( IConfiguration configurationIn )
        {
            configuration = configurationIn;
        }

        public float ToSimulationTime( float realTimeSec ) => realTimeSec * configuration.TimeScaleFactor;
    }
}
