using UnityEngine;

namespace JH.IoCLight.Samples.ColorSwitcher
{
    public class ColorSwitcherBootstrap : BootstrapBase
    {
        public Material material;

        public override void Compose()
        {
            Container.Register<ColorSwitcher>().SingleInstance();
        }

        public void Update()
        {
            material.color = Container.Resolve<ColorSwitcher>().MakeNextColor(Time.time);
        }
    }
}
