using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 某个具体的材料UI自身的控制脚本
/// </summary>
public class MaterialItem : MonoBehaviour {


    private Transform m_Transform;
    private Image m_Image;

    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Image = m_Transform.Find("Icon").GetComponent<Image>();
    }

    public void Height()
    {
         m_Image.color = Color.red;
    }

    public void Normal()
    {
        m_Image.color = Color.white;
    }
}
