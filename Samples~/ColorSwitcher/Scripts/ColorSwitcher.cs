using System;
using UnityEngine;

namespace JH.IoCLight.Samples.ColorSwitcher
{
    internal class ColorSwitcher
    {
        public const float redScale = 5f;
        public const float greenScale = 7f;
        public const float blueScale = 11f;

        internal Color MakeNextColor(float time)
        {
            return new Color(
                Mathf.Max(0f, Mathf.Sin(time * redScale)),
                Mathf.Max(0f, Mathf.Sin(time * greenScale)),
                Mathf.Max(0f, Mathf.Sin(time * blueScale)),
                1f
            );
        }
    }
}
