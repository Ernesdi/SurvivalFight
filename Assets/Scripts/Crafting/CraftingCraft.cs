using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 合成具体操作类
/// </summary>
public class CraftingCraft : MonoBehaviour {

   
    private Transform m_Transform;                  
    private Transform goodItem_Transform;           //框框
    private Image goodItem_Image;                   //图片
    private Button craft_Button;                    //单个合成
    private Button craftAll_Button;                 //合成全部按钮
    private Text craft_NumText;                     //结果槽个数的Text文本                  
    private GameObject newItem;

    private int tempId;
    private string tempSpriteName;

    private int min;                                //合成槽中材料最小的数目
    private int resultNum;                          //结果槽中物品的数量


    public GameObject NewItem { set { newItem = value; } }
    public Transform GoodItem_Transform { get { return goodItem_Transform; } }
    public int ResultNum { get { return resultNum; }set { value = resultNum; } }


    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        goodItem_Transform = m_Transform.Find("GoodItem").GetComponent<Transform>();
        goodItem_Image = m_Transform.Find("GoodItem/ItemImage").GetComponent<Image>();
        craft_Button = m_Transform.Find("Craft").GetComponent<Button>();
        craftAll_Button = m_Transform.Find("CraftAll").GetComponent<Button>();
        craft_NumText = m_Transform.Find("CraftNum").GetComponent<Text>();

        craft_Button.onClick.AddListener(()=> CraftClick());
        craftAll_Button.onClick.AddListener(()=> CraftAllClick());

        //加到事件中自动更新最小材料数（事件）
        CraftingPanelController.Instance.UpdateMinMaterialNum += Fens;

        goodItem_Image.gameObject.SetActive(false);
        craft_NumText.gameObject.SetActive(false);

        NorCraft();
    }


    /// <summary>
    /// 设置面板的值
    /// </summary>
    public void SetItemValue(int id, string spName)
    {
        goodItem_Image.gameObject.SetActive(true);
        goodItem_Image.sprite = Resources.Load<Sprite>("Textures/Inventory/" + spName);

        tempId = id;
        tempSpriteName = spName;
       
    }


    /// <summary>
    /// （高亮）材料填满可以合成  （单个合成）
    /// </summary>
    public void HeiCraft() {
      
        //单个合成可点击（变红）
        craft_Button.interactable = true;
        craft_Button.GetComponent<Image>().color = Color.red;
        //全部合成可点击（变红）
        craftAll_Button.interactable = true;
        craftAll_Button.GetComponent<Image>().color = Color.red;

    }

    /// <summary>
    /// 普通状态 （单个合成）
    /// </summary>
    public void NorCraft()
    {
        //单个合成不可点击（变暗）
        craft_Button.interactable = false;
        craft_Button.GetComponent<Image>().color = Color.white;
        //全部合成不可点击（变暗）
        craftAll_Button.interactable = false;
        craftAll_Button.GetComponent<Image>().color = Color.white;
    }

    /// <summary>
    /// 单个合成点击
    /// </summary>
    private void CraftClick()
    {
        SendMessageUpwards("CalcMinMaterial");
        Debug.Log("单个合成");
        GameObject go = Instantiate<GameObject>(newItem);
        //设置父物体
        go.transform.SetParent(goodItem_Transform);
        //查找组件，预制体上面有了
        InventoryItemManager iim =  go.GetComponent<InventoryItemManager>();
        //设置初始值
        iim.SetItemValue(tempSpriteName, tempId, 1, 1);
        //重置位置
        go.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //重置大小
        iim.ResetSpriteSize(go.GetComponent<RectTransform>(), 110, 110);
        //不在背包里
        iim.IsInventory = false;
        //变为普通按钮
        //NorCraft();
        //结果槽显示数量
        CraftResultNum();
        Debug.Log("结果槽里面"+go.GetComponent<InventoryItemManager>().ToString());
        //向C层发送消息
        SendMessageUpwards("CraftingOK");
    }


    /// <summary>
    /// 全部合成点击
    /// </summary>
    private void CraftAllClick()
    {
        SendMessageUpwards("CalcMinMaterial");
        Debug.Log("全部合成");
        Debug.Log("最小数是：" + min);
        for (int i = 0; i < min; i++)
        {
            GameObject go = Instantiate<GameObject>(newItem);
            //设置父物体
            go.transform.SetParent(goodItem_Transform);
            //查找组件，预制体上面有了
            InventoryItemManager iim = go.GetComponent<InventoryItemManager>();
            //设置初始值
            iim.SetItemValue(tempSpriteName, tempId, 1, 1);
            //重置位置
            go.GetComponent<RectTransform>().localPosition = Vector3.zero;
            //重置大小
            iim.ResetSpriteSize(go.GetComponent<RectTransform>(), 110, 110);
            //不在背包里
            iim.IsInventory = false;
        }
        //结果槽显示数量
        CraftResultNum();
        //向C层发送消息
        SendMessageUpwards("CraftingAllOK");
    }

    //观察者 观察的是C层的最小材料数
    private void Fens(int num)
    {
        this.min = num;
    }



    /// <summary>
    /// 合成结果槽的数量
    /// </summary>
    public void CraftResultNum() {
        if (goodItem_Transform.GetComponentsInChildren<InventoryItemManager>().Length!=0)
        {
            craft_NumText.gameObject.SetActive(true);
            resultNum = goodItem_Transform.GetComponentsInChildren<InventoryItemManager>().Length;
            craft_NumText.text = resultNum.ToString();
            Debug.Log("合成结果槽的数量" + resultNum);
        }
        else
        {
            craft_NumText.gameObject.SetActive(false);
        }
    }


}
