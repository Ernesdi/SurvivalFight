using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个内容自身的管理器
/// </summary>
public class CraftingContentManager : MonoBehaviour {

    private GameObject CraftingContentItem;
    private Transform m_Transform;

    private List<CraftingContentItemManager> cciList;
    private CraftingContentItemManager currentItemShow;

    void Awake () {
        CraftingContentItem = Resources.Load<GameObject>("Prefabs/Crafting/CraftingContentItem");
        m_Transform = gameObject.GetComponent<Transform>();

        cciList = new List<CraftingContentItemManager>();
    }
    
    /// <summary>
    /// 初始化内容面板
    /// </summary>
    /// <param name="list"></param>
    public void InitContent(List<CraftingContentItem> list) {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject contentItem =  GameObject.Instantiate(CraftingContentItem, m_Transform);
            CraftingContentItemManager ccim = contentItem.GetComponent<CraftingContentItemManager>();
            cciList.Add(ccim);
            ccim.SetItemValue(i,list[i].ItemId, list[i].ItemName);
        }
       
    }


    

    /// <summary>
    /// 设置那个内容显示
    /// </summary>
    public void SetItemContentShow(CraftingContentItemManager ccim)
    {
        if (currentItemShow == ccim) return;
        //先发送，再切换显示（向合成面板C层发送消息）
        SendMessageUpwards("CreateMaps", ccim.ItemId);
        //判断合成面板C层里的MapIsCanTAB的值，true为可以切换，false就是不行，（材料是否有地方放回背包栏）
        if (CraftingPanelController.Instance.MapIsCanTAB == false)
        {
            //不能切换的话就直接返回不执行下面的语句,就是不切换内容的显示
            return;
        }
        Debug.Log("切换tab里面的内容：" + ccim.ItemId);
        if (currentItemShow != null)
        {
            currentItemShow.Normal();
        }
        ccim.Active();
        
        currentItemShow = ccim;
    }

    /// <summary>
    /// 用于给上一级C层调用
    /// </summary>
    public void ShowContent() {
        gameObject.SetActive(true);

    }

    /// <summary>
    /// 用于给上一级C层调用
    /// </summary>
    public void HideContent()
    {
        gameObject.SetActive(false);
    }

}
