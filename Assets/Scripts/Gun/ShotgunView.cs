using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 散弹枪V层
/// </summary>
public class ShotgunView : GunViewBase
{
    //子弹弹出特效的位置
    private Transform bulletEffect_Transform;
    //手动拉拴的声音的声音
    private AudioClip bulletEffect_Clip;
    //子弹弹出的特效
    private GameObject bulletEffect_Shell;
    //散弹枪子弹预制体加载
    private GameObject prefab_bullet;

    public Transform BulletEffect_Transform { get { return bulletEffect_Transform; } }
    public AudioClip BulletEffect_Clip { get { return bulletEffect_Clip; } }
    public GameObject BulletEffect_Shell { get { return bulletEffect_Shell; } }
    public GameObject Prefab_bullet { get { return prefab_bullet; } }


    protected override void Init()
    {
        bulletEffect_Transform = M_Transform.Find("Armature/Weapon/BulletEffect").GetComponent<Transform>();
        bulletEffect_Clip = Resources.Load<AudioClip>("Soures/Shotgun_Pump");
        bulletEffect_Shell = Resources.Load<GameObject>("Prefabs/Gun/Shotgun_Shell");
        prefab_bullet = Resources.Load<GameObject>("Prefabs/Gun/Shotgun_Bullet");
    }

    protected override void InitHoldPostPos()
    {
        StartPos = M_Transform.localPosition;
        StartRot = M_Transform.localRotation.eulerAngles;
        EndPos = new Vector3(-0.125f, -1.7675f, 0.05f);
        EndRot = new Vector3(2, 0, 0);
    }

    protected override void FindGunPointTransform()
    {
        FireEffect_Transform = M_Transform.Find("Armature/Weapon/FireEffect").GetComponent<Transform>();
    }

   
}
