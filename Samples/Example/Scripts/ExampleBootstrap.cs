using System.Collections;
using System.Collections.Generic;
using IoCLight;
using UnityEngine;

public class ExampleBootstrap : BootstrapBase
{
    public Material material;

    public override void Compose()
    {
        container.Register<ColorSwitcher>().SingleInstance();
    }

    public void Update()
    {
        material.SetColor( "_Color", Container.Resolve<ColorSwitcher>().MakeNextColor( Time.time ) );
    }
}
