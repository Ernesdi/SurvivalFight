using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 枪械工厂
/// </summary>
public class GunFactory : MonoBehaviour {

    public static GunFactory Instance;

    private Transform m_Transform;

    private GameObject m_AssaultRifle;
    private GameObject m_Shotgun;
    private GameObject m_WoodenBow;
    private GameObject m_WoodenSpear;
    private GameObject m_HuntingRifle;


    private int id;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        LoadModel();
    }

    /// <summary>
    /// 读取模型
    /// </summary>
    private void LoadModel() {
        m_AssaultRifle = Resources.Load<GameObject>("Prefabs/Gun/Prefabs/Assault Rifle");
        m_Shotgun = Resources.Load<GameObject>("Prefabs/Gun/Prefabs/Shotgun");
        m_WoodenBow = Resources.Load<GameObject>("Prefabs/Gun/Prefabs/Wooden Bow");
        m_WoodenSpear = Resources.Load<GameObject>("Prefabs/Gun/Prefabs/Wooden Spear");
        m_HuntingRifle = Resources.Load<GameObject>("Prefabs/Gun/Prefabs/Hunting Rifle");
    }

    /// <summary>
    /// 用名字创建枪械模型
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject CreateModelByName(string name,GameObject item) {
        GameObject model = null;
        switch (name)
        {
            case "Assault Rifle":
                model = Instantiate<GameObject>(m_AssaultRifle,m_Transform);
                InitGun(model, 90, 100, WeapenType.AssaultRifle, item,30,true);
                break;
            case "Shotgun":
                model = Instantiate<GameObject>(m_Shotgun, m_Transform);
                InitGun(model, 200, 100, WeapenType.Shotgun, item,5,true);
                break;
            case "Wooden Bow":
                model = Instantiate<GameObject>(m_WoodenBow, m_Transform);
                InitGun(model, 70, 100, WeapenType.WoodenBow, item,1,false);
                break;
            case "Wooden Spear":
                model = Instantiate<GameObject>(m_WoodenSpear, m_Transform);
                InitGun(model, 70, 1, WeapenType.WoodenSpear, item,1,false);
                break;
            case "Hunting Rifle":
                model = Instantiate<GameObject>(m_HuntingRifle, m_Transform);
                InitGun(model, 100, 50, WeapenType.HuntingRifle, item, 5, true);
                break;
        }
        return model;
    }

    /// <summary>
    /// 初始化枪械数值
    /// </summary>
    private void InitGun(GameObject gun,int demage,int durable,WeapenType type,GameObject item,int cratridgeNum,bool isWeapenGun)
    {
        gun.GetComponent<GunControllerBase>().Id = id++;
        gun.GetComponent<GunControllerBase>().Demage = demage;
        gun.GetComponent<GunControllerBase>().Durable = durable;
        gun.GetComponent<GunControllerBase>().GunType = type;
        gun.GetComponent<GunControllerBase>().Gun_UI = item;
        GunUIPanel.Instance.InitGunUI(item, cratridgeNum, cratridgeNum,durable, isWeapenGun);
    }
}
