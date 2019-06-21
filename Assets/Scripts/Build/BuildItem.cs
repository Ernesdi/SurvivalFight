using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildItem : MonoBehaviour {

    private Transform m_Transform;
    private Transform icon_Transform;
    private Image m_Image;
    private Image icon_Image;
    private List<GameObject> materialList = new List<GameObject>();             //材料集合
    public List<GameObject> MaterialList
    {
        get { return materialList; }
        set { materialList = value;}
    }

    void Awake () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Image = gameObject.GetComponent<Image>();
        icon_Transform = m_Transform.Find("Icon").GetComponent<Transform>();
        icon_Image = m_Transform.Find("Icon").GetComponent<Image>();
       
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="quaternion">旋转调度</param>
    /// <param name="isShowBG">是否选中背景</param>
    /// <param name="sprite">icon图片</param>
    /// <param name="isShowIcon">是否显示Icon</param>
    public void Init(Quaternion quaternion, bool isShowBG, Sprite sprite, bool isShowIcon)
    {
        m_Transform.rotation = quaternion;
        m_Image.enabled = isShowBG;
        icon_Image.sprite = sprite;
        icon_Transform.rotation = Quaternion.Euler(Vector3.zero);
        icon_Image.enabled = isShowIcon;
    }

    /// <summary>
    /// 显示BG
    /// </summary>
    public void ShowBG()
    {
        m_Image.GetComponent<Image>().enabled = true;
        ShowAndHideMaterials(true);
    }
    /// <summary>
    /// 隐藏BG
    /// </summary>
    public void CloseBG()
    {
        m_Image.GetComponent<Image>().enabled = false;
        ShowAndHideMaterials(false);
    }


    /// <summary>
    /// 添加GameObject对象到List中
    /// </summary>
    public void AddMaterialInList(GameObject go)
    {
        materialList.Add(go);
    }

    /// <summary>
    /// 显示和隐藏自己所有的材料UI
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowAndHideMaterials(bool isShow)
    {
        if (materialList == null) return;
        for (int i = 0; i < materialList.Count; i++)
        {
            materialList[i].SetActive(isShow);
        }
    }

  
}
