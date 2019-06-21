using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个Tab自身的控制器
/// </summary>
public class CraftingTabItemManager : MonoBehaviour {

    private Transform m_Transform;

    private Image center_BG;
    private Image icon;

    private int index;

    private Button m_Button;

    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();
        center_BG = m_Transform.Find("Center_BG").GetComponent<Image>();
        icon = m_Transform.Find("Icon").GetComponent<Image>();
        m_Button = gameObject.GetComponent<Button>();

        //向合成面板C层发送消息
        m_Button.onClick.AddListener(()=> { SendMessageUpwards("ShowTabAndContent", index);Debug.Log("第一人称的时候鼠标监听有时候会没有反应,Tab"+ index); });

    }


    /// <summary>
    /// 给单个Item进行赋值的方法
    /// </summary>
    public void SetItemValue(int index,string name) {
        this.index = index;
        icon.sprite = Resources.Load<Sprite>("Textures/Crafting/" + name);
    }

    /// <summary>
    /// 未选中显示,显示背景图片,用于合成面板C层调用
    /// </summary>
    public void NormalTab() {
        if (center_BG.IsActive()==false)
        {
            center_BG.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 选中的时候显示，隐藏背景图片,用于合成面板C层调用
    /// </summary>
    public void ActiveTab()
    {
        center_BG.gameObject.SetActive(false);
    }

}
