using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可投掷武器C层
/// </summary>
public abstract class ThrowerControllerBase : GunControllerBase {
    protected override void Controller()
    {
        base.Controller();
        //直接射击
        if (Input.GetMouseButtonDown(0)&& canShoot)
        {
            MouseLeftDown();
        }
    }

}
