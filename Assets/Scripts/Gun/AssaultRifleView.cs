using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 步枪武器V层
/// </summary>
public class AssaultRifleView : GunViewBase {

    private Transform bulletEffect_Transform;
    private Transform bulletParentTransform;
    private Transform gunPointEffectParent;

    private GameObject prefab_bullet;
   
    public Transform BulletEffect_Transform { get { return bulletEffect_Transform; } }
    public Transform BulletParentTransform { get { return bulletParentTransform; } }
    public Transform GunPointEffectParent { get { return gunPointEffectParent; } }

    public GameObject Prefab_bullet { get {return prefab_bullet; } }

    //子类实现父类的定义的初始化瞄准位置的抽象方法
    protected override void InitHoldPostPos()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        //EndPos = new Vector3(-0.066f, -1.86f, 0.15f);
        //EndRot = new Vector3(5.6f, 1.35f, 0.1f);
        EndPos = new Vector3(-0.073f, -1.837f, 0.15f);
        EndRot = new Vector3(0.12f, 0,0);
    }

    //子类实现父类的定义的初始化枪口位置的抽象方法
    protected override void FindGunPointTransform()
    {
        FireEffect_Transform = M_Transform.Find("Assault_Rifle/FireEffect").GetComponent<Transform>();
    }

    protected override void Init()
    {
        bulletEffect_Transform = M_Transform.Find("Assault_Rifle/BulletEffect").GetComponent<Transform>();
        bulletParentTransform = GameObject.Find("TempObject/BulletParent").GetComponent<Transform>();
        gunPointEffectParent = GameObject.Find("TempObject/GunPointEffectParent").GetComponent<Transform>();

        prefab_bullet = Resources.Load<GameObject>("Prefabs/Gun/Bullet");
    }
}
