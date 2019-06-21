using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 长矛V层
/// </summary>
public class WoodenSpearView : GunViewBase {


    private GameObject prefab_Spear;

    public GameObject Prefab_Spear { get { return prefab_Spear; } }

    protected override void Init()
    {
        prefab_Spear = Resources.Load<GameObject>("Prefabs/Gun/Wooden_Spear");
    }

    protected override void FindGunPointTransform()
    {
        FireEffect_Transform = M_Transform.Find("Armature/Arm_R/Forearm_R/Wrist_R/Weapon/FireEffect").GetComponent<Transform>();
    }

    protected override void InitHoldPostPos()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        EndPos = new Vector3(0, -1.58f, 0.32f);
        EndRot = new Vector3(0.37f, -0.18f, 0.3f);
    }

}
