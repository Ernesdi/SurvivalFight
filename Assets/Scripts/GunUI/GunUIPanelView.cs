using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 枪械UI  V层
/// </summary>
public class GunUIPanelView : MonoBehaviour {

   

    private Transform m_Transform;
    private Transform item_Transform;
    //当前子弹数
    private Text currentBullet;
    //一个弹夹的数量
    private Text cartridgeNum;
    private Text fireModel;
    private Image reloadBullet;

    public Transform M_Transform { get { return m_Transform; }set { m_Transform = value; } }
    public Transform Item_Transform { get { return item_Transform; }set { item_Transform = value; } }
    public Text CurrentBullet { get { return currentBullet; }set { currentBullet = value; } }
    public Text CartridgeNum { get { return cartridgeNum; }set { cartridgeNum = value; } }
    public Text FireModel { get { return fireModel; }set { fireModel = value; } }
    public Image ReloadBullet { get { return reloadBullet; } set { reloadBullet = value; } }

    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();
        item_Transform = m_Transform.Find("Item").GetComponent<Transform>();
        currentBullet = m_Transform.Find("CurrentBullet").GetComponent<Text>();
        cartridgeNum = m_Transform.Find("CartridgeNum").GetComponent<Text>();
        fireModel = m_Transform.Find("FireModel").GetComponent<Text>();
        reloadBullet = m_Transform.Find("ReloadBullet").GetComponent<Image>();
    }
	
	
}
