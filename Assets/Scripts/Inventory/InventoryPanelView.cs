using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包模块V层
/// </summary>
public class InventoryPanelView : MonoBehaviour {
    private Transform grid_Transform;
    private Button bagArrange_Btn;
    private Button idFuse_Btn;

    private GameObject inventorySlot;
    private GameObject inventoryItem;

    

    public Transform Grid_Transform { get { return grid_Transform; } }
    public Button BagArrange_Btn { get { return bagArrange_Btn; } }
    public Button IdFuse_Btn { get { return idFuse_Btn; } }
    public GameObject InventorySlot { get { return inventorySlot; } }
    public GameObject InventoryItem { get { return inventoryItem; } }





    void Awake () {
        grid_Transform = GameObject.Find("Canvas/InventoryPanel/BackGround/Grid").GetComponent<Transform>();
        bagArrange_Btn = GameObject.Find("Canvas/InventoryPanel/BackGround/Tools/BagArrange").GetComponent<Button>();
        idFuse_Btn = GameObject.Find("Canvas/InventoryPanel/BackGround/Tools/IdFuse").GetComponent<Button>();

        inventorySlot = Resources.Load<GameObject>("Prefabs/Inventory/InventorySlot");
        inventoryItem = Resources.Load<GameObject>("Prefabs/Inventory/InventoryItem");


    }


}
