using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个内容槽的自己的管理器
/// </summary>
public class CraftingMapItemManager : MonoBehaviour {

    private Transform m_Transform;          
    private Image itemImage;                //图片
    private CanvasGroup m_CanvasGroup;      //是否接收射线

    private int itemMapId;                  //图谱的ID
    private bool isReceive;                 //是否能接受物品 默认为false 在SetItemValue方法被调用后修改为接受
    public bool IsReceive { get { return isReceive; } }

    public int ItemMapId { get { return itemMapId; } }

    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();
        itemImage = m_Transform.Find("Item").GetComponent<Image>();
        m_CanvasGroup = m_Transform.Find("Item").GetComponent<CanvasGroup>();

       
        //item图标不接收射线
        m_CanvasGroup.blocksRaycasts = false;

        ResetMap();
    }


    /// <summary>
    /// 给自身进行赋值。C层调用
    /// </summary>
    public void SetItemValue(string itemMapId,Sprite sprite,string itemMapName) {
        //先启用
        itemImage.gameObject.SetActive(true);
        //这个槽可以接受物品，要放在启用之后。
        isReceive = true;
        itemImage.gameObject.name = itemMapName;

        this.itemMapId = int.Parse(itemMapId);
        this.itemImage.sprite = sprite;
    }
    /// <summary>
    /// 重置图谱
    /// </summary>
    public void ResetMap()
    {
        //这个槽不可以接收物品，要放在关闭之前。
        isReceive = false;
        itemImage.gameObject.name = "null";
        itemImage.gameObject.SetActive(false);
    }


}
