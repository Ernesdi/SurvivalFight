using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 背包模块C层
/// </summary>
public class InventoryPanelController : MonoBehaviour {

    public static InventoryPanelController Instance;

    private InventoryPanelModel inventoryPanelModel;
    private InventoryPanelView inventoryPanelView;


    private List<GameObject> slotList;
    private List<GameObject> itemList;

    void Awake()
    {
        Instance = this;
    }

    void Start () {
        inventoryPanelModel = gameObject.GetComponent<InventoryPanelModel>();
        inventoryPanelView = gameObject.GetComponent<InventoryPanelView>();


        slotList = new List<GameObject>();
        itemList = new List<GameObject>();

        CreateAllSlot();
        CreateAllItem();

        //整理背包按钮绑定
        inventoryPanelView.BagArrange_Btn.onClick.AddListener(()=>BagArrange());
        //背包内id相同融合按钮绑定
        inventoryPanelView.IdFuse_Btn.onClick.AddListener(()=> IdFuse());
    }

    /// <summary>
    /// 创建背包中所有的格子
    /// </summary>
    private void CreateAllSlot() {
        for (int i = 0; i < 27; i++)
        {
           GameObject slot =  GameObject.Instantiate(inventoryPanelView.InventorySlot, inventoryPanelView.Grid_Transform);
           slot.gameObject.name = "InventorySlot_" +i;
           slotList.Add(slot);
        }
    }


    /// <summary>
    /// 创建所有的Item
    /// </summary>
    private void CreateAllItem() {
        List<InventoryItem> jsonItemList = inventoryPanelModel.GetJsonByName("InventoryJsonData");
        for (int i = 0; i < jsonItemList.Count; i++)
        {
           GameObject item = Instantiate(inventoryPanelView.InventoryItem, slotList[i].transform);
           itemList.Add(item);
           InventoryItemManager manager = item.GetComponent<InventoryItemManager>();
           manager.SetItemValue(jsonItemList[i].ItemName, jsonItemList[i].ItemId, jsonItemList[i].ItemNum, jsonItemList[i].ItemBar);
        }
    }
    /// <summary>
    /// 隐藏当前面板
    /// </summary>
    public void HideCurrentPanel()
    {
      
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示当前面板
    /// </summary>
    public void ShowCurrentPanel()
    {
        gameObject.SetActive(true);
      
    }

    /// <summary>
    /// 检查单个物品槽中的个数
    /// </summary>
    public void CheckSlotItemNum() {
        for (int i = 0; i < slotList.Count; i++)
        {
            InventoryItemManager[] iim = slotList[i].GetComponentsInChildren<InventoryItemManager>();
            Debug.Log(iim.Length);
        }
    }

    /// <summary>
    /// 接收合成面板返回来的材料放到背包里面（先看看背包里面有没有同id的，如果有就放回同id的地方，没有就找一个材料的地方放下）
    /// </summary>
    /// <param name="list">材料列表</param>
    public bool ReciveMaterial(List<GameObject> materialList)
    {
        int tempIndex = 0;
        for (int i = 0; i < materialList.Count; i++)
        {
            if (tempIndex == materialList.Count) break;
            for (int j = 0; j < slotList.Count; j++)
            {
                //当前背包槽里有物品且槽里面的东西没用耐久值就允许叠加
                if (slotList[j].transform.Find("InventoryItem")!=null)
                {
                    if (materialList[tempIndex].GetComponent<InventoryItemManager>().ItemId == slotList[j].transform.Find("InventoryItem").GetComponent<InventoryItemManager>().ItemId)
                    {
                        //二重判断，结果槽里的东西丢回背包栏可能会有耐久度，因此不能叠加
                        if (materialList[tempIndex].GetComponent<InventoryItemManager>().ItemBar==0 && slotList[j].transform.Find("InventoryItem").GetComponent<InventoryItemManager>().ItemBar==0)
                        {
                            materialList[tempIndex].GetComponent<InventoryItemManager>().MergeMaterials(slotList[j].transform.Find("InventoryItem").GetComponent<InventoryItemManager>());
                            tempIndex++;
                            Debug.Log("材料放回到背包的已经有次材料的地方了");
                            break;
                        }
                        //id相同的话有耐久值就什么也不做，等着出循环然后最后会在空槽里放这个东西。
                    }
                }
                //在空槽里放这个东西
                else
                {
                    materialList[tempIndex].transform.SetParent(slotList[j].transform);
                    InventoryItemManager iim = materialList[tempIndex].GetComponent<InventoryItemManager>();
                    iim.IsInventory = true;
                    iim.InCrafting = false;
                    Debug.Log("材料放回到背包的空槽：" + iim.ToString());
                    iim.ResetSpriteSize(materialList[tempIndex].GetComponent<RectTransform>(), 85, 85);
                    tempIndex++;
                    break;
                }
            }
        }
        //如果当前材料都放回背包槽了，就返回true
        if (tempIndex == materialList.Count)
        {
            return true;
        }
        //如果当前材料放回背包放不下，就返回false
        return false;

        ////临时引用   （直接丢回来不加判断）
        //int tempIndex = 0;
        //for (int i = 0; i < slotList.Count; i++)
        //{

        //    Transform itemTransform = slotList[i].transform.Find("InventoryItem");
        //    //如果背包栏的位置是空的且当前索引小于传过来的材料列表的大小，因为大于或者等于了就等于说材料都放回背包里面了
        //    if (itemTransform == null && tempIndex < materialList.Count)
        //    {
        //        materialList[tempIndex].transform.SetParent(slotList[i].transform);
        //        InventoryItemManager iim = materialList[tempIndex].GetComponent<InventoryItemManager>();
        //        iim.IsInventory = true;
        //        iim.InCrafting = false;
        //        Debug.Log("材料放回背包：" + iim.ToString());
        //        iim.ResetSpriteSize(materialList[tempIndex].GetComponent<RectTransform>(), 85, 85);
        //        tempIndex++;

        //    }
        //}

        //背包栏中的ID相同的融合
        //IdFuse(slotList);
    }


    /// <summary>
    /// 背包整理，空位置就拿后面的材料补上。
    /// </summary>
    public void BagArrange()
    {
        int index = 1;
        for (int i = 0; i < slotList.Count; i++)
        {
            //获得引用
            Transform itemTransform = slotList[i].transform.Find("InventoryItem");
            //如果这个槽没有东西，就从这个槽往后开始循环整个背包找有材料的槽挖过来
            if (itemTransform == null)
            {
                Debug.Log(i);
                Debug.Log("第" + index + "个槽没有东西");
                //int nullIndex = 1;
                if (index == slotList.Count)
                {
                    Debug.Log("最后一个槽也为空的话就直接返回了");
                    break;
                }
                for (int j = index; j < slotList.Count; j++)
                {
                    Debug.Log(j);
                    Debug.Log("从第" + (j + 1) + "个槽开始查找。");
                    //后面的槽有材料
                    if (slotList[j].transform.Find("InventoryItem") != null)
                    {
                        Debug.Log("找到材料了并且放到第" + (i + 1) + "个槽了");
                        Transform temp = slotList[j].transform.Find("InventoryItem").transform;
                        temp.SetParent(slotList[i].transform);
                        temp.localPosition = Vector3.zero;
                        break;
                    }
                }
            }
            index++;
        }
        //更新背包数据到Json文件中
        //UpdataJsonData();
    }


    ///// <summary>
    ///// 合成栏丢下背包栏中的ID相同的融合(明天要做的)  这个太垃圾了
    ///// </summary>
    //private void IdFuse(List<InventoryItemManager> tempList) {
    //    Debug.Log("合成栏丢下背包栏中的ID相同的融合");
    //    Debug.Log("tempList：" + tempList.Count);
    //    int index = 1;
    //    for (int i = 0; i < tempList.Count; i++)
    //    {
    //        //到底了(都判断完了)就返回
    //        if (index == tempList.Count) break;
    //        //如果这一个的ID和下一个的id相同
    //        if (tempList[i].ItemId == tempList[index].ItemId)
    //        {
    //            //调用合成方法
    //            tempList[i].MergeMaterials(tempList[index]);
    //        }
    //        index++;
    //    }

    //}


    /// <summary>
    /// 集合中的材料ID相同的融合到第一个
    /// </summary>
    public void IdFuse() {
        int index = 1;
        for (int i = 0; i < slotList.Count; i++)
        {
        //获得引用
        Transform itemTransform = slotList[i].transform.Find("InventoryItem");
        //如果这个槽有东西，就从这个槽往后开始循环整个背包找到Id相同（且没有耐久值）的然后融合在一起
            if (itemTransform != null)
            {
                Debug.Log(i);
                Debug.Log("第" + (i + 1) + "个槽有东西");
               
                for (int j = index; j < slotList.Count; j++)
                {
                    Debug.Log(j);
                    Debug.Log("从第" + (j + 1) + "个槽开始查找。");
                    if (index == slotList.Count)
                    {
                        Debug.Log("判断到最后一个槽了");
                        break;
                    }
                    //后面的槽有材料
                    if (slotList[j].transform.Find("InventoryItem") != null)
                    {
                        //前面的槽的引用
                        InventoryItemManager currentIIM = slotList[i].transform.Find("InventoryItem").GetComponent<InventoryItemManager>();
                        //后面的槽的引用
                        InventoryItemManager backIIM = slotList[j].transform.Find("InventoryItem").GetComponent<InventoryItemManager>();

                        //如果后面的槽跟前面的槽材料ID是一样的且二者的耐久度都不等于1（就是没有耐久度）
                        if (currentIIM.ItemId == backIIM.ItemId && currentIIM.ItemBar!=1&& backIIM.ItemBar != 1)
                        {
                            //调用合成方法,把后一位的数量累加到前一位身上
                            backIIM.MergeMaterials(currentIIM);
                            Debug.Log("找到ID相同的了，进行合成,当前总数" + currentIIM.ItemNum);
                           
                        }
                       
                    }
                }
            }
            //循环累加
            index++;
        }
    }


    /// <summary>
    /// 更新Json文件数据
    /// </summary>
    private void UpdataJsonData() {
        List<InventoryItem> iiList = new List<InventoryItem>();
        for (int i = 0; i < slotList.Count; i++)
        {
            //当前背包槽里有物品
            if (slotList[i].transform.Find("InventoryItem") != null)
            {
                InventoryItemManager iim =  slotList[i].transform.Find("InventoryItem").GetComponent<InventoryItemManager>();
                InventoryItem ii = new InventoryItem(iim.ItemId, iim.ItemName, iim.ItemNum, iim.ItemBar);
                iiList.Add(ii);
            }
        }
        JsonTools.UpdateJsonFile<InventoryItem>(iiList, "InventoryJsonData");
    }


    /// <summary>
    /// 用于下级物体合成所需要的材料数  只有两个C层之间才会有沟通 （子类）不允许沟通 
    /// </summary>
    /// <param name="material"></param>
    public void SendUpdateMaterialsCount()
    {
        CraftingPanelController.Instance.UpdateMaterialsCount();
    }

    /// <summary>
    /// 用于iim类拖拽结束后的引用  只有两个C层之间才会有沟通 （子类）不允许沟通 
    /// </summary>
    /// <param name="material"></param>
    public void SendCraftResultNum() {
        CraftingPanelController.Instance.SendCraftResultNum();
    }

   

}
