using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 具体的单个元素自身的控制器
/// </summary>
public class InventoryItemManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform m_RectTransform;      //单个元素自身的位置
    private RectTransform canvas_Transform;     //画布的位置
    private Transform selfParent;               //自身的父物体                  
    private Image m_Image;                      //图标
    private Image bar_Image;                    //耐久度
    private Text num_Text;                      //数量
    private CanvasGroup m_CanvasGroup;          //画布组，用于开启接受射线的开启与关闭

    private float lastTime;

    private string itemName;                  //背包内物品的名字
    private int itemId;                         //背包内物品的ID
    private int itemNum;                        //背包内物品的数量
    private int itemBar;                        //背包内物品的耐久条
    private bool isInventory = true;            //是否在背包中 true是   false 就是在合成栏内  
    private bool inCrafting;                    //是否在合成栏中 true是    
    private bool isDrag;                        //是否正在拖拽中，用来判断是否可以拆分材料
    private bool isBreak;                       //是否是拆分的材料 以防拆分之后拖拽到不是能接收的次图标的区域然后消失。   
    private bool isBreakIn;                     //是否正在拆分中，如果是不允许二次拆分
    private InventoryItemManager breakMaterials;//材质分离（break）出来的脚本引用


    public bool IsInventory
    {
        get { return isInventory; }
        set
        {
            isInventory = value;
            //当发生改变时更新一下位置
            m_RectTransform.localPosition = Vector3.zero;
        }
    }
    public bool InCrafting { get { return inCrafting; } set { inCrafting = value; } }
    public bool IsDrag { get { return isDrag; } set { isDrag = value; } }
    public bool IsBreakIn { get { return isBreakIn; } set { isBreakIn = value; } }

    public string ItemName { get { return itemName; } set { itemName = value; } }
    public int ItemId { get { return itemId; } set { itemId = value; } }
    public int ItemNum
    {
        get { return itemNum; }
        set
        {
            itemNum = value;
            if (num_Text != null)
            {
                //发生改变就跟着改变
                num_Text.text = ItemNum.ToString();
            }
        }
    }

    public int ItemBar { get { return itemBar; } set { itemBar = value; } }


    void Awake()
    {
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        canvas_Transform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
        m_Image = m_RectTransform.GetComponent<Image>();
        bar_Image = m_RectTransform.Find("Bar").GetComponent<Image>();
        num_Text = m_RectTransform.Find("Num").GetComponent<Text>();

        //改名
        gameObject.name = "InventoryItem";
    }

    void Update()
    {
        if (isDrag && Input.GetKeyDown(GameConst.BreakMaterialsKey))
        {
            BreakMaterials();
        }
    }

    /// <summary>
    /// 设置单个物品的值
    /// </summary>
    public void SetItemValue(string itemName, int itemId, int itemNum, int itemBar)
    {
        this.m_Image.sprite = Resources.Load<Sprite>("Textures/Inventory/" + itemName);
        this.itemName = itemName;
        this.itemId = itemId;
        ItemNum = itemNum;
        this.itemBar = itemBar;

        BarOrNum();
    }

    /// <summary>
    /// 物品是否有耐久值
    /// </summary>
    private void BarOrNum()
    {
        if (itemBar == 0)
        {
            bar_Image.gameObject.SetActive(false);
            num_Text.gameObject.SetActive(true);
        }
        else
        {
            bar_Image.gameObject.SetActive(true);
            num_Text.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// 升级UI
    /// </summary>
    public void UpdataUI(float value)
    {
        if (value == 0)
        {
            Destroy(gameObject);
            if (GameObject.Find("Canvas/MainPanel/GunUIPanel")!=null)
            {
                GameObject.Find("Canvas/MainPanel/GunUIPanel").SetActive(false);
            }
            gameObject.transform.parent.GetComponent<ToolBarSlotManager>().Normal();
        }
        bar_Image.fillAmount = value;
    }


    /// <summary>
    /// 鼠标开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //原先父物体保存
        selfParent = m_RectTransform.parent;
        //接受射线关掉 不然只能输出目前物体的名字
        m_CanvasGroup.blocksRaycasts = false;
        //是否正在拖拽
        isDrag = true;

    }

    /// <summary>
    /// 拖拽进行中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("拖拽中的物品的名字是：" + m_RectTransform.name);
        Vector3 newPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RectTransform, eventData.position, eventData.enterEventCamera, out newPos);
        //位置跟随
        m_RectTransform.position = newPos;
        //放在最外面保证不被遮挡
        m_RectTransform.SetParent(canvas_Transform);

    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //拖拽到的目标物体的引用
        GameObject placement = eventData.pointerEnter;

        DragCtrl(placement);

        //重设位置
        m_RectTransform.localPosition = Vector3.zero;
        //接收射线
        m_CanvasGroup.blocksRaycasts = true;
        //是否正在拖拽,是否正在拆分
        isBreakIn = false;
        isDrag = false;
        //（有合成物品拖拽到其他地方的时候）更新合成结果槽里的合成物品数量
        InventoryPanelController.Instance.SendCraftResultNum();
        //更新合成栏里的材料数
        InventoryPanelController.Instance.SendUpdateMaterialsCount();
    }



    /// <summary>
    /// 拖拽逻辑
    /// </summary>
    /// <param name="target">落点</param>
    private void DragCtrl(GameObject placement)
    {
        //拖拽到UI区域
        if (placement != null)
        {
            //输出拖拽物体落点的名字
            Debug.Log("落点的名字" + placement.name);
            #region   //落点在背包中的槽
            if (placement.tag == "InventorySlot")
            {
                //如果是空槽就放进去
                if (placement.transform.Find("InventoryItem") == null)
                {
                    //设置父物体
                    m_RectTransform.SetParent(placement.transform);
                    Debug.Log("父物体的名字" + placement.transform.name);
                    //重设雪碧图
                    ResetSpriteSize(m_RectTransform, 85, 85);
                    //在背包内
                    IsInventory = true;
                    //不在合成面板里
                    InCrafting = false;
                    isDrag = false;                        //是否正在拖拽中，用来判断是否可以拆分材料
                    isBreak = false;                       //是否是拆分的材料 以防拆分之后拖拽到不是能接收的次图标的区域然后消失。
                    isBreakIn = false;                     //是否正在拆分中，如果是不允许二次拆分
                    Debug.Log("拖拽到背包中的空槽"+ToString());
                }
                //不是空槽就判断ID是否一样
                else
                {
                    //id一样（以防拖拽到边框不增加，因为边框和图标并不是一样大的.或者结果槽里的东西丢到背包栏里）
                    if (placement.transform.Find("InventoryItem").GetComponent<InventoryItemManager>().ItemId == ItemId)
                    {
                        //没有耐久度。
                        if (placement.transform.Find("InventoryItem").GetComponent<InventoryItemManager>().itemBar == 0 && itemBar == 0)
                        {
                            MergeMaterials(placement.transform.Find("InventoryItem").GetComponent<InventoryItemManager>());
                            
                        }
                        //有耐久度。
                        else
                        {
                            //重新设置回父物体
                            m_RectTransform.SetParent(selfParent);
                        }
                      
                    }
                    //ID不一样 直接返回
                    else
                    {
                        ReturnPosition();
                    }
                }
                //（安全校验）如果目标的有大于1个子物体的时候，就重新设置回父物体
                Debug.Log("这个背包槽有物体：" + placement.transform.GetComponentsInChildren<InventoryItemManager>().Length + "个");
                //id一样而且原本就有一个物品就叠加起来
                if (placement.transform.GetComponentsInChildren<InventoryItemManager>().Length > 1)  //在孩子中查找会包括自己身上的脚本
                {
                    //重新设置回父物体
                    m_RectTransform.SetParent(selfParent);
                }
                
            }

            #endregion

            #region //落点在合成栏内
            else if (placement.tag == "CraftingCenterSlot")
            {
                Debug.Log(placement.GetComponent<CraftingMapItemManager>().IsReceive);

                //如果拖到的位置是可以接收物品的
                if (placement.GetComponent<CraftingMapItemManager>().IsReceive)
                {
                    //如果拖拽的物品的ID和图谱槽中图片的id一样
                    if (ItemId == placement.GetComponent<CraftingMapItemManager>().ItemMapId)
                    {
                        //如果合成槽里面没有东西那么就可以放进去，如果有那就叠加
                        if (placement.transform.Find("InventoryItem") == null)
                        {
                            m_RectTransform.SetParent(placement.transform);
                            ResetSpriteSize(m_RectTransform, 70, 60);
                            //是否在背包内，用于判断是否可以互换位置
                            IsInventory = false;
                            //是否在合成面板中拆分材料
                            InCrafting = true;
                            //变成不是拆分的材料不然会出现漏洞
                            isBreak = false;
                            isDrag = false;                        //是否正在拖拽中，用来判断是否可以拆分材料
                            isBreakIn = false;                     //是否正在拆分中，如果是不允许二次拆分

                           
                            Debug.Log("添加进来的材料数是："+ gameObject.GetComponent<InventoryItemManager>().ItemNum);
                        }
                        else
                        {
                            MergeMaterials(placement.transform.Find("InventoryItem").GetComponent<InventoryItemManager>());
                        }

                    }
                    else
                    {
                        ReturnPosition();
                    }

                    //（安全校验） 如果目标的有大于1个子物体的时候，就重新设置回父物体
                    Debug.Log("这个合成槽有物体：" + placement.transform.GetComponentsInChildren<InventoryItemManager>().Length + "个");
                    //id一样而且原本就有一个物品就叠加起来
                    if (placement.transform.GetComponentsInChildren<InventoryItemManager>().Length > 1)  //在孩子中查找会包括自己身上的脚本
                    {
                        m_RectTransform.SetParent(selfParent);
                    }

                }
                //合成栏中不能接收材料的地方
                else
                {
                    ReturnPosition();
                }

            }

            #endregion

            #region 物品互换与叠加
            else if (placement.tag == "InventoryItem")
            {
                //获得目标位置的this
                InventoryItemManager iim = placement.GetComponent<InventoryItemManager>();
                //目标位置
                Transform targetTransform = placement.GetComponent<Transform>();

                //如果ID一样
                if (ItemId==iim.ItemId)
                {
                    //如果都没有耐久值
                    if (itemBar == 0 && iim.itemBar == 0)
                    {
                        MergeMaterials(iim);
                    }
                    //其中一个有耐久值或者都有耐久值 
                    else
                    {
                        if (IsInventory && iim.IsInventory)
                        {
                            //设置为目标父物体
                            m_RectTransform.SetParent(targetTransform.parent);
                            //目标物体设置父物体
                            targetTransform.SetParent(selfParent);
                            //还原目标位置 拖拽的位置在通用代码中执行
                            targetTransform.localPosition = Vector3.zero;
                            Debug.Log("有耐久值的两个ID相同的物体互换位置");
                        }
                        else
                        {
                            ReturnPosition();
                        }
                       
                    }
                }
                //ID不一样
                else
                {
                    //如果是材质分离（break）出来的
                    if (isBreak)
                    {
                        //把材料数累加到复制出来的那个然后销毁自身
                        MergeMaterials(breakMaterials);
                        //isBreak标志位重新设置为false
                        isBreak = false;
                    }
                    else
                    {
                        if (IsInventory && iim.IsInventory)
                        {
                            Debug.Log("拖拽中的IsInventory标志位是：" + IsInventory + ",需要互换位置的IsInventory标志位是" + placement.GetComponent<InventoryItemManager>().IsInventory);
                            //设置为目标父物体
                            m_RectTransform.SetParent(targetTransform.parent);
                            //目标物体设置父物体
                            targetTransform.SetParent(selfParent);
                            //还原目标位置 拖拽的位置在通用代码中执行
                            targetTransform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            //重新设置回父物体
                            m_RectTransform.SetParent(selfParent);
                        }
                      
                    }
                }

                //（安全校验） 如果目标的有大于1个子物体的时候，就重新设置回父物体
                Debug.Log("这个合成槽有物体：" + placement.transform.GetComponentsInChildren<InventoryItemManager>().Length + "个");
                //id一样而且原本就有一个物品就叠加起来
                if (placement.transform.GetComponentsInChildren<InventoryItemManager>().Length > 1)  //在孩子中查找会包括自己身上的脚本
                {
                    m_RectTransform.SetParent(selfParent);
                }
            }
          
            #endregion

            #region//落点不是在三个背包槽，合成槽，物品上面
            else
            {
                ReturnPosition();
                Debug.Log("拖拽到的区域是UI地方但是这个UI不接受分离出来的材质");
                 
            }
            #endregion
            Debug.Log("撒手之后拖拽的" + ToString());

        }

        //拖拽到了非ui区域
        else
        {
            ReturnPosition();
        }

    }



    /// <summary>
    /// 重设雪碧图大小
    /// </summary>
    public void ResetSpriteSize(RectTransform rect_Transform, float width, float height)
    {
        //设置大小在当前的锚点上，参数是轴向和大小
        rect_Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rect_Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    /// <summary>
    /// 合成材料
    /// </summary>
    /// <param name="target">传过来一个目标~~就是不用销毁的那个</param>
    public void MergeMaterials(InventoryItemManager target)
    {
        target.ItemNum += ItemNum;
        Destroy(gameObject);
    }

    /// <summary>
    /// 拆分材料
    /// </summary>
    private void BreakMaterials()
    {
        if (itemNum <= 1) return;
        if (isBreakIn) return;
        Debug.Log("拆分");

        isBreakIn = true;
        isBreak = true;

        //复制一份出来
        GameObject tempB = GameObject.Instantiate<GameObject>(gameObject);
        InventoryItemManager iim = tempB.GetComponent<InventoryItemManager>();
        //获取复制出来的引用以防用户把分离的材质丢到不能接收的地方去
        breakMaterials = tempB.GetComponent<InventoryItemManager>();
        Debug.Log("复制出来的的isInventory标志位是:" + iim.IsInventory);

        //如果是在背包栏里拆分材料
        if (IsInventory)
        {
            //把复制出来的物品的InCrafting设置为否
            iim.InCrafting = false;
            iim.IsInventory = true;
        }
        //如果是在合成栏里拆分材料
        if (inCrafting)
        {
            //把复制出来的物品的IsInventory设置为否,inCrafting设置为true
            iim.IsInventory = false;
            iim.InCrafting = true;
        }
        //赋值出来的东西设置父物体
        tempB.transform.SetParent(selfParent);

        //计算拆分数量
        int TotleNum = itemNum;
        int tempNumB = TotleNum / 2;
        int tempNumA = TotleNum - tempNumB;

        //重新赋值
        iim.ItemNum = tempNumB;
        ItemNum = tempNumA;
        //重置位置
        iim.m_RectTransform.localPosition = Vector3.zero;
        //重置ID
        iim.ItemId = ItemId;
       
        iim.IsDrag = false;


        //复制的物体重新接收射线
        tempB.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Debug.Log("复制出来的" + tempB.GetComponent<InventoryItemManager>().ToString());
        Debug.Log("刚刚拖拽的" + ToString());
    }


    /// <summary>
    /// 返回之前的位置(分别是直接拖拽的或者拆分出来的)
    /// </summary>
    public void ReturnPosition() {
        //如果是材质分离（break）出来的
        if (isBreak)
        {
            //把材料数累加到复制出来的那个然后销毁自身
            MergeMaterials(breakMaterials);
            //isBreak标志位重新设置为false
            isBreak = false;

        }
        else
        {
            //重新设置回父物体
            m_RectTransform.SetParent(selfParent);
        }
    }


    public override string ToString()
    {
        return string.Format("背包内物品的ID是{0}，背包内物品的数量是{1}，背包内物品的耐久条是{2},是否在背包中{3},是否在合成栏中{4},是否正在拖拽{5}，是否是拆分的材料{6}", itemId, itemNum, itemBar, isInventory, inCrafting, isDrag, isBreak);
    }


}
