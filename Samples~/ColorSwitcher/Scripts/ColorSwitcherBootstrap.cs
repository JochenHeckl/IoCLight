using System.Collections;
using System.Collections.Generic;
using de.JochenHeckl.Unity.IoCLight;
using UnityEngine;

public class ColorSwitcherBootstrap : BootstrapBase
{
    public Material material;

    public override void Compose()
    {
        Container.Register<ColorSwitcher>().SingleInstance();
    }

    public void Update()
    {
        material.color = Container.Resolve<ColorSwitcher>().MakeNextColor( Time.time );
    }
}
