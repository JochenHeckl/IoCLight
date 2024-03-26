using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReplaceMeWithYourProjectsNamespace
{
    internal interface IConfiguration
    {
        public string StringConfigurationValue { get; }
        public int IntConfigurationValue { get; }
        public float FloatConfigurationValue { get; }
    }
}
