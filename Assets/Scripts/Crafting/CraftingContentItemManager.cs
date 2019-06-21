using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 左边内容中单个元素自身的管理器
/// </summary>
public class CraftingContentItemManager : MonoBehaviour {
    
    private Transform m_Transform;

    private Button m_Button;

    private int index;
    private int itemId;
    private Image item_BG;
    private Text item_Name;

    public int ItemId { get { return itemId; } }


    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();

        item_BG = m_Transform.Find("BG").GetComponent<Image>();
        item_Name = m_Transform.Find("Name").GetComponent<Text>();

        m_Button = gameObject.GetComponent<Button>();
        //向craftingContentManager发送消息。
        m_Button.onClick.AddListener(() => SendMessageUpwards("SetItemContentShow", this));

        Normal();
    }

    /// <summary>
    /// 设置自身的值
    /// </summary>
    public void SetItemValue(int index,int itemId, string itemName) {
        this.index = index;
        this.itemId = itemId;
        this.item_Name.text = itemName;
        gameObject.name = "CraftingContentItem" + index;
    }

    /// <summary>
    /// 未选中状态
    /// </summary>
    public void Normal() {
        item_BG.gameObject.SetActive(false);
    }

    /// <summary>
    /// 选中状态
    /// </summary>
    public void Active()
    {
        item_BG.gameObject.SetActive(true);

    }
}
