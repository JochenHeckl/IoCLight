using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    internal class Configuration : IConfiguration
    {
        public string StringConfigurationValue { get; set; }
        public int IntConfigurationValue { get; set; }
        public float FloatConfigurationValue { get; set; }
    }
}
