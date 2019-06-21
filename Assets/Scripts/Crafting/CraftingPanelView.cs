using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 合成面板V层
/// </summary>
public class CraftingPanelView : MonoBehaviour {

    private GameObject craftingTabItem;
    private GameObject craftingContent;
    private GameObject craftingCenterSlot;
    private GameObject inventoryItem;

    private Transform tabs_Transform;
    private Transform contents_Transform;
    private Transform center_Transform;



    public GameObject CraftingTabItem { get { return craftingTabItem; } }
    public GameObject CraftingContent { get { return craftingContent; } }
    public GameObject CraftingCenterSlot { get { return craftingCenterSlot; } }
    public GameObject InventoryItem { get { return inventoryItem; } }

    public Transform Tabs_Transform { get { return tabs_Transform; } }
    public Transform Contents_Transform { get { return contents_Transform; } }
    public Transform Center_Transform { get { return center_Transform; } }

    void Awake () {
        craftingTabItem = Resources.Load<GameObject>("Prefabs/Crafting/CraftingTabItem");
        craftingContent = Resources.Load<GameObject>("Prefabs/Crafting/CraftingContent");
        craftingCenterSlot = Resources.Load<GameObject>("Prefabs/Crafting/CraftingCenterSlot");
        inventoryItem = Resources.Load<GameObject>("Prefabs/Inventory/InventoryItem");

        tabs_Transform = GameObject.Find("Canvas/InventoryPanel/CraftingPanel/Left/Tabs").GetComponent<Transform>();
        contents_Transform = GameObject.Find("Canvas/InventoryPanel/CraftingPanel/Left/Contents").GetComponent<Transform>();
        center_Transform = GameObject.Find("Canvas/InventoryPanel/CraftingPanel/Center").GetComponent<Transform>();
    }

    /// <summary>
    /// 通过名字读取一个材料的雪碧图
    /// </summary>
    /// <param name="spriteName"></param>
    public Sprite LoadMaterialSpriteByName(string spriteName) {
        Sprite sprite = Resources.Load<Sprite>("Textures/Material/"+ spriteName);
        return sprite;
    }
}
