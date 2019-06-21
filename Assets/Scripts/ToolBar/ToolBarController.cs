using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主面板UI工具条C层
/// </summary>
public class ToolBarController : MonoBehaviour {

    public static ToolBarController Instance;

  
    private ToolBarModel m_ToolBarModel;
    private ToolBarView m_ToolBarView;

    private GameObject tempSlot = null;                     //当前框框
    private GameObject tempModel = null;                    //当前模型
    private List<GameObject> slotList;                      //工具槽list集合
    private Dictionary<GameObject, GameObject> modelList;   //枪械模型字典集合

    public GameObject TempModel { get { return tempModel; } } //属性 GameController 调用

    private int CurrentKeyCode = -1;                       //当前武器的索引；

    void Awake()
    {
        Instance = this;
    }


    void Start () {
        Init();
        CreateAllToolSlot();
    }


    private void Init()
    {
    
        m_ToolBarModel = gameObject.GetComponent<ToolBarModel>();
        m_ToolBarView = gameObject.GetComponent<ToolBarView>();

        slotList = new List<GameObject>();
        modelList = new Dictionary<GameObject, GameObject>();
    }

    /// <summary>
    /// 创建所有的工具槽
    /// </summary>
    private void CreateAllToolSlot() {
        for (int i = 0; i < 8; i++)
        {
            GameObject go = Instantiate<GameObject>(m_ToolBarView.Prefab_ToolBarSlot, m_ToolBarView.Grid_Transform);
            go.GetComponent<ToolBarSlotManager>().InitSlot(m_ToolBarView.Prefab_ToolBarSlot.name + i,i+1);

            slotList.Add(go);
        }
    }

    /// <summary>
    /// 鼠标管理工具栏中的物品激活状态
    /// </summary>
    private void ManagerSlotActive(GameObject slot) {
        //判断临时槽存储不为Null 且 临时槽存储不等于传过来的槽
        if (tempSlot != null && tempSlot!= slot)
        {
            //把临时槽中的物品设置为不激活
            tempSlot.GetComponent<ToolBarSlotManager>().Normal();
        }
        //把传过来的槽设置为临时槽
        tempSlot = slot;
    }

    /// <summary>
    /// 按键管理工具栏中的物品激活状态
    /// </summary>
    public void ManagerSlotActiveOnKey(int keyNum)
    {
        //如果当前物品槽没有东西
        if (slotList[keyNum].GetComponent<Transform>().Find("InventoryItem")==null)
        {
            return;
        }
        //判断临时槽存储不为Null 且 临时槽存储不等于list集合里指定角标的槽
        if (tempSlot != null && slotList[keyNum]!= tempSlot)
        {
            //把临时槽中的物品设置为不激活  方框正常
            tempSlot.GetComponent<ToolBarSlotManager>().Normal();
        }
        //把传过来的槽设置为临时槽
        tempSlot = slotList[keyNum];

        //重新点击一下 方框高亮
        tempSlot.GetComponent<ToolBarSlotManager>().SlotClick();
        //如果上一个按键与这次的按键一样，且当前模型不为空
        if (CurrentKeyCode == keyNum && tempModel != null)
        {
            StartCoroutine("WeapeonDown");
        }
        else
        {
            //更换武器
            FindInventoryItem();
        }
        //存储用户的按键
        CurrentKeyCode = keyNum;
    }


    /// <summary>
    /// 调用枪械工厂类
    /// </summary>
    private void FindInventoryItem() {
        Transform m_temp = tempSlot.GetComponent<Transform>().Find("InventoryItem");
        StartCoroutine("CallGunFactory", m_temp);
    }

    IEnumerator CallGunFactory(Transform m_temp) {
        //如果找到这个元素 就不实例化
        if (m_temp != null)
        {
            Debug.Log("呼叫枪械工厂");
            //把当前武器隐藏
            if (tempModel != null)
            {
                //先放下武器
                tempModel.GetComponent<GunControllerBase>().Holster();
                Debug.Log("放下当前武器");
                yield return new WaitForSeconds(0.3f);
                tempModel.SetActive(false);
            }
            GameObject temp = null;
            modelList.TryGetValue(m_temp.gameObject, out temp);
            //如果能不能在字典集合中找到武器
            if (temp == null)
            {//实例化一个武器 并加到字典集合中
                temp = GunFactory.Instance.CreateModelByName(m_temp.GetComponent<Image>().sprite.name, m_temp.gameObject);
                modelList.Add(m_temp.gameObject, temp);
            }
            else
            {//如果当前方框是高亮的 显示的 就显示武器  不是就不显示武器 因为头部每次调用都是先隐藏的
                if (tempSlot.GetComponent<ToolBarSlotManager>().IsActive)
                {
                    temp.SetActive(true);
                }
            }
            //重置当前武器
            tempModel = temp;
        }

    }


    /// <summary>
    /// 放下当前武器
    /// </summary>
    /// <returns></returns>
    IEnumerator WeapeonDown() {
        tempModel.GetComponent<GunControllerBase>().Holster();
        Debug.Log("放下当前武器");
        yield return new WaitForSeconds(0.3f);
        //隐藏  
        tempModel.SetActive(false);
        //卸下武器  置空当前模型
        tempModel = null;
    }


    /// <summary>
    /// 是否使用狙击镜子
    /// </summary>
    public void ChangeSniperAimState(bool isUse)
    {
        if (isUse)
            m_ToolBarView.SniperAim_Transform.gameObject.SetActive(true);
        else
            m_ToolBarView.SniperAim_Transform.gameObject.SetActive(false);
    }
}
