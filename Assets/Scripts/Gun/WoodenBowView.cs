using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 弓箭V层
/// </summary>
public class WoodenBowView : GunViewBase
{
    private GameObject prefab_Arrow;

    public GameObject Prefab_Arrow { get { return prefab_Arrow; } }

    protected override void Init()
    {
        prefab_Arrow = Resources.Load<GameObject>("Prefabs/Gun/Arrow");
    }

    protected override void FindGunPointTransform()
    {
        FireEffect_Transform = M_Transform.Find("Armature/Arm_L/Forearm_L/Wrist_L/Weapon/FireEffect").GetComponent<Transform>();
    }

    protected override void InitHoldPostPos()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        EndPos = new Vector3(0.8f, -1.19f, 0.3f);
        EndRot = new Vector3(0, -1.05f, 36);
    }
}
