using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//模式：观察者模式  此类是被观察者(主播)    粉丝关注的是主播 粉丝是观察者
public delegate void AnchorDel(int num);

/// <summary>
/// 合成面板C层
/// </summary>
public class CraftingPanelController : MonoBehaviour {

    //事件配合委托实现观察者模式(更新合成槽中材料最小的数目)  事件可以实现委托链更新所有加到事件里面的，不用事件只能实现单个更新
    public event AnchorDel UpdateMinMaterialNum;
    public static CraftingPanelController Instance;

    private Transform m_Transform;
    private CraftingPanelModel craftingPanelModel;
    private CraftingPanelView craftingPanelView;
    private CraftingCraft craftingCraft;


    private List<GameObject> tabList;                       //tab集合
    private List<GameObject> contentList;                   //内容集合
    private List<GameObject> slotList;                      //中间的空槽集合

    private bool mapIsCanTAB = true;      //是否图谱可以进行切换（如果材料放不回背包栏，那么不给切换。就是背包里的材料满了而且背包中没有和合成栏中相同ID的材料）

    private int currentIndex = -1;
    private int slotNum = 25;         //有多少个图谱槽

    private int needMaterialsCount=-1;    //合成物品需要的材料总数
    private int minMaterialNum=1;           //最小的材料数
   

    public int MinMaterialNum
    {
        get { return minMaterialNum; }
        set {
            minMaterialNum = value;
            //当最小材料数发生改变时向粉丝发送消息
            UpdateMinMaterialNum(minMaterialNum);
        }
    }

    public bool MapIsCanTAB { get { return mapIsCanTAB; } }

    void Awake()
    {
        Instance = this;
       
    }

    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        craftingPanelModel = gameObject.GetComponent<CraftingPanelModel>();
        craftingPanelView = gameObject.GetComponent<CraftingPanelView>();
        craftingCraft = m_Transform.Find("Right").GetComponent<CraftingCraft>();

        tabList = new List<GameObject>();
        contentList = new List<GameObject>();
        slotList = new List<GameObject>();


        CreateTabs();
        CreateContents();

        ShowTabAndContent(0);

        CreateSlot();

        //把预制体赋予给合成面板用于合成显示
        craftingCraft.NewItem = craftingPanelView.InventoryItem;
    }
   

    /// <summary>
    /// 创建左边所有的选项
    /// </summary>
    private void CreateTabs() {
        List<CraftingTabItem> tempTabList =  JsonTools.GetFirstJsonList<CraftingTabItem>("CraftingTapsJsonData");
        for (int i = 0; i < tempTabList.Count; i++)
        {
            GameObject tab =  Instantiate(craftingPanelView.CraftingTabItem,craftingPanelView.Tabs_Transform);
            tab.gameObject.name = "tab_" + i;
            tabList.Add(tab);
            CraftingTabItemManager ctim = tab.GetComponent<CraftingTabItemManager>();
            ctim.SetItemValue(i,tempTabList[i].Category);
        }
     
    }

    /// <summary>
    /// 创建左边所有的内容
    /// </summary>
    private void CreateContents()
    {
        List<List<CraftingContentItem>> tempItemList = craftingPanelModel.GetContentJsonByName("CraftingContentsJsonData");
        for (int i = 0; i < tempItemList.Count; i++)
        {
            GameObject content = GameObject.Instantiate(craftingPanelView.CraftingContent, craftingPanelView.Contents_Transform);
            content.gameObject.name = "Content_" + i;
            contentList.Add(content);
            CraftingContentManager ccm = content.GetComponent<CraftingContentManager>();
            ccm.InitContent(tempItemList[i]);
            ccm.HideContent();
        }

    }

    
    /// <summary>
    /// 设置那个tab和内容的显示
    /// </summary>
    /// <param name="index"></param>
    public void ShowTabAndContent(int index) {
        //判断重复点击次界面
        if (currentIndex == index) return;

        //判断合成面板C层里的MapIsCanTAB的值，true为可以切换，false就是不行，（材料是否有地方放回背包栏）
        if (MapIsCanTAB == false)
        {
            bool temp = ResetMaterials(slotList);
            Debug.Log("合成面板的材料放不到背包栏里去");
            if (temp)
            {
                Debug.Log("切换到tab:" + index);
                for (int i = 0; i < tabList.Count; i++)
                {
                    tabList[i].GetComponent<CraftingTabItemManager>().NormalTab();
                    contentList[i].SetActive(false);
                }
                tabList[index].GetComponent<CraftingTabItemManager>().ActiveTab();
                contentList[index].SetActive(true);
                //重新赋值
                currentIndex = index;
                //要记得重新赋值啊
                mapIsCanTAB = true;
                Debug.Log("材料丢回背包栏成功！");
            }
            else
            {
                //不能切换的话就直接返回不执行下面的语句,就是不切换内容的显示
                return;
            }
        }
        Debug.Log("切换到tab:" + index);
        for (int i = 0; i < tabList.Count; i++)
        {
            tabList[i].GetComponent<CraftingTabItemManager>().NormalTab();
            contentList[i].SetActive(false);
        }
        tabList[index].GetComponent<CraftingTabItemManager>().ActiveTab();
        contentList[index].SetActive(true);
        //重新赋值
        currentIndex = index;
    }


    /// <summary>
    /// 创建所有的图谱槽
    /// </summary>
    private void CreateSlot() {
        for (int i = 0; i < slotNum; i++)
        {
            GameObject slot = GameObject.Instantiate(craftingPanelView.CraftingCenterSlot, craftingPanelView.Center_Transform);
            slot.gameObject.name = "slot_" + i;
            slotList.Add(slot);
        }
    }


    /// <summary>
    /// 创建某个材料的合成图谱
    /// </summary>
    public void CreateMaps(int mapId)
    {
        //先恢复到默认状态
        craftingCraft.NorCraft();
        Debug.Log("item" + mapId);
        CraftingMapItem cmi = craftingPanelModel.GetCraftingMapItemByMapId(mapId);
        if (cmi == null) return;
        //先把结果槽里的合成物品丢回背包栏
        for (int i = 0; i < craftingCraft.ResultNum; i++)
        {
            List<GameObject> tempList = new List<GameObject>();
            tempList.Add(craftingCraft.GoodItem_Transform.gameObject);
            mapIsCanTAB = ResetMaterials(tempList);
            Debug.Log("执行了：" + tempList.Count);
        }
        //更新一下结果槽的数量
        craftingCraft.CraftResultNum();

        if (!mapIsCanTAB)
        {
            return;
        }
        //mapIsCanTAB等于true表示现在合成槽里的材料为0  false的话说明合成槽里还有材料
        mapIsCanTAB = ResetMaterials(slotList);
        //如果合成槽里还有材料就直接返回
        if (!mapIsCanTAB)
        {
            return;
        }
        //重置合成图谱
        ResetMap();
        //重新进行赋值
        for (int i = 0; i < cmi.MapContents.Length; i++)
        {
            if (cmi.MapContents[i] != "0")
            {
                Sprite sprite = craftingPanelView.LoadMaterialSpriteByName(cmi.MapContents[i]);
                //进行赋值
                slotList[i].GetComponent<CraftingMapItemManager>().SetItemValue(cmi.MapContents[i], sprite, cmi.MapName);
            }
        }
        craftingCraft.SetItemValue(cmi.MapId, cmi.MapName);
        Debug.Log("创建id为"+ cmi.MapId+"雪碧图名称为：" + cmi.MapName+"的图谱");
        //把需要的合成的材料总数存储到临时变量中去
        needMaterialsCount = cmi.MapMaterialCount;
    }


    /// <summary>
    /// 重置图谱
    /// </summary>
    private void ResetMap() {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].GetComponent<CraftingMapItemManager>().ResetMap();
        }
    }


    /// <summary>
    /// 查找当前某个槽中的材料（合成槽这样的大槽）加到集合中，把材料丢回背包栏。
    /// </summary>
    /// <param name="FindInList">要在那个列表槽里查找这些材料</param>
    /// <returns></returns>
    private bool ResetMaterials(List<GameObject> FindInList) {
        List<GameObject> materialList = new List<GameObject>();
        for (int i = 0; i < FindInList.Count; i++)
        {
            //找到面板中的材料
            Transform materialTransform = FindInList[i].transform.Find("InventoryItem");
            if (materialTransform!=null)
            {
                materialList.Add(materialTransform.gameObject);
            }
        }
        
        Debug.Log("面板中有"+ materialList.Count+"个材料");

        //获取到的材料列表为0 证明上次的剩余材料都放回背包了，直接返回（省事）
        if (materialList.Count==0)
        {
            return true;
        }
        //列表不为空执行就丢回背包的方法
        else
        {
            return InventoryPanelController.Instance.ReciveMaterial(materialList);
        }
      
    }

    /// <summary>
    /// 更新放入的材料数,合成栏的材料总数
    /// </summary>
    public void UpdateMaterialsCount()
    {
        int currentMaterialsCount=0; //当前放入的材料总数
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].transform.Find("InventoryItem")!=null)
            {
                currentMaterialsCount++;
            }
        }
        Debug.Log("当前的材料数：" + currentMaterialsCount + "，需要的材料数：" + needMaterialsCount);
        //如果当前丢进去的材料数等于合成需求数  
        if (currentMaterialsCount == needMaterialsCount)
        {
            //高亮按钮
            craftingCraft.HeiCraft();
        }
        else
        {
            //普通按钮
            craftingCraft.NorCraft();
        }
    }


    /// <summary>
    /// 计算最小材料数
    /// </summary>
    public void CalcMinMaterial() {
        List<InventoryItemManager> orderList = new List<InventoryItemManager>();
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].transform.Find("InventoryItem") != null)
            {
                orderList.Add(slotList[i].transform.Find("InventoryItem").GetComponent<InventoryItemManager>());
            }
        }
        Debug.Log("orderList.Count" + orderList.Count);
        //循环对比
        MinMaterialNum = orderList[0].ItemNum;
        for (int i = 0; i < orderList.Count; i++)
        {
            if (orderList[i].ItemNum < MinMaterialNum)
            {
                MinMaterialNum = orderList[i].ItemNum;
            }
        }
        Debug.Log("最小材料数为：" + MinMaterialNum);
    }

    /// <summary>
    /// 合成完毕
    /// </summary>
    public void CraftingOK() {
        Debug.Log("减少材料数或者销毁");
        for (int i = 0; i < slotList.Count; i++)
        {
            //找到面板中的材料
            Transform materialTransform = slotList[i].transform.Find("InventoryItem");
            if (materialTransform != null)
            {
                InventoryItemManager iim = materialTransform.GetComponent<InventoryItemManager>();
                //如果材料数本身就是1
                if (iim.ItemNum == 1)
                {
                    Debug.Log("材料销毁");
                    //销毁
                    Destroy(iim.gameObject);
                    craftingCraft.NorCraft();
                   
                }
                else
                {
                   
                    Debug.Log("材料-1");
                    iim.ItemNum -= 1;
                }
            }
        }
        MinMaterialNum--;
        Debug.Log("单个合成完毕更新最小材料数：" + MinMaterialNum);
        //StartCoroutine("CraftingOKMaterialsBackInventory");

    }

    /// <summary>
    /// 全部合成完毕
    /// </summary>
    public void CraftingAllOK()
    {
        Debug.Log("减少材料数或者销毁");
        for (int i = 0; i < slotList.Count; i++)
        {
            //找到面板中的材料
            Transform materialTransform = slotList[i].transform.Find("InventoryItem");
            if (materialTransform != null)
            {
                InventoryItemManager iim = materialTransform.GetComponent<InventoryItemManager>();
                //如果材料数本身就是1
                if (iim.ItemNum == MinMaterialNum)
                {
                    Debug.Log("材料销毁");
                    //销毁
                    Destroy(iim.gameObject);
                    craftingCraft.NorCraft();
                }
                else
                {
                    Debug.Log("材料-1");
                    iim.ItemNum -= MinMaterialNum;
                }
            }
        }
        MinMaterialNum =999999;
        Debug.Log("全部合成完毕更新最小材料数：" + MinMaterialNum);
        StartCoroutine("CraftingOKMaterialsBackInventory");
    }


    /// <summary>
    /// 全部合成完毕之后等待两秒其余的材料回归到背包中
    /// </summary>
    /// <returns></returns>
    IEnumerator CraftingOKMaterialsBackInventory()
    {

        yield return new WaitForSeconds(2);
        //执行把材料丢回背包栏
        ResetMaterials(slotList);
       
    }


    /// <summary>
    /// 向子物体craftingCraft类发送消息(两个基类之间狗头)
    /// </summary>
    public void SendCraftResultNum() {
        craftingCraft.CraftResultNum();
    }

}
