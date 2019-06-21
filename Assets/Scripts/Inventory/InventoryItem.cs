using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包内的单个物品  （实体类）用于解析json数据存储成一个List集合
/// </summary>
public class InventoryItem {
    private string itemName;
    private int itemNum;
    private int itemId;
    private int itemBar;

    public int ItemId
    {
        get { return itemId; }
        set { itemId = value; }
    }

    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    public int ItemNum
    {
        get { return itemNum; }
        set { itemNum = value; }
    }

    public int ItemBar { get { return itemBar; } set { itemBar = value; } }

    public InventoryItem() { }
    public InventoryItem(int itemId, string itemName, int itemNum, int itemBar)
    {
        this.ItemId = itemId;
        this.ItemName = itemName;
        this.ItemNum = itemNum;
        this.ItemBar = itemBar;
    }

    public override string ToString()
    {
        return string.Format("物品的ID是:{0},物品的名字是：{1}，物品的数量是:{2}，物品是否有血条{3}", itemId, itemName, ItemNum, ItemBar);
    }

}
