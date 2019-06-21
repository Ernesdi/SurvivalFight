using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个内容面板实体类
/// </summary>
public class CraftingContentItem  {

    private int itemId;
    private string itemName;

    public int ItemId { get { return itemId; }set { itemId = value; } }
    public string ItemName { get { return itemName; }set { itemName = value; } }

    public CraftingContentItem() { }

    public override string ToString()
    {
        return string.Format("面板的ID是{0}，面板的名字是{1}",itemId,itemName);
    }

}
