using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主面板UI工具条V层
/// </summary>
public class ToolBarView : MonoBehaviour {
    private Transform sniperAim_Transform;

    private Transform m_Transform;
    private Transform grid_Transform;
    private GameObject prefab_ToolBarSlot;


    public Transform SniperAim_Transform { get { return sniperAim_Transform; } }
    public Transform Grid_Transform { get { return grid_Transform; } }
    public GameObject Prefab_ToolBarSlot { get { return prefab_ToolBarSlot; } }
  

    void Awake () {
        sniperAim_Transform = GameObject.Find("Canvas/MainPanel/SniperAim").GetComponent<Transform>();
        sniperAim_Transform.gameObject.SetActive(false);
        m_Transform = gameObject.GetComponent<Transform>();

        grid_Transform = m_Transform.Find("Grid").GetComponent<Transform>();
        prefab_ToolBarSlot = Resources.Load<GameObject>("Prefabs/ToolBar/ToolBarSlot");

    }
	
	
	
}
