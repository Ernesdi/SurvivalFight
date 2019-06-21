using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工具栏单个槽自身的管理脚本
/// </summary>
public class ToolBarSlotManager : MonoBehaviour {

    private Transform m_Transform;
    private Image m_Image;
    private Button m_Button;

    private Text m_Text;

    private bool isActive;

    public bool IsActive { get { return isActive; } }

    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Image = gameObject.GetComponent<Image>();
        m_Button = gameObject.GetComponent<Button>();

        m_Text = m_Transform.Find("Text").GetComponent<Text>();

        m_Button.onClick.AddListener(SlotClick);
    }


    /// <summary>
    /// 初始化槽
    /// </summary>
    public void InitSlot(string name,int orderNum)
    {
        gameObject.name = name;
        m_Text.text = orderNum.ToString();
    }


    /// <summary>
    /// 工具栏中的某个物品被点击的时候
    /// if else 只能实现单个工具来回点击的切换状态
    /// </summary>
    public void SlotClick() {
        if (isActive)
        {
            Normal();
        }
        else
        {
            Active();
        }

        //向ToolBarController C层发送当前槽 用于判断激活显示那个物品槽
        SendMessageUpwards("ManagerSlotActive",gameObject);
    }

    /// <summary>
    /// 没有选中工具栏中的某个工具时
    /// </summary>
    public void Normal() {
        m_Image.color = Color.white;
        isActive = false;
    }


    /// <summary>
    /// 选中工具栏中的某个工具时
    /// </summary>
    private void Active() {
        m_Image.color = Color.red;
        isActive = true;
    }

}
