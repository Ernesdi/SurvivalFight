using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuildPanelController : MonoBehaviour {

    public static BuildPanelController Instance;

    private BuildPanelModel m_BuildPanelModel;
    private BuildPanelView m_BuildPanelView;
    private List<BuildItem> BuildItemList = new List<global::BuildItem>();      //BuildItem集合
    private bool ShowWheelBG = true;                                            //是否显示整个扇形UI
    //Item
    private float mouseValue = 90000;                                           //鼠标的值  设值大一点这样就不会乱序
    private int index;                                                          //鼠标索引
    private BuildItem currentItem;                                              //当前高亮
    private BuildItem targerItem;                                               //要切换到的目标
    //Material
    private float mouseValue_M = 30000;                                           //鼠标的值  设值大一点这样就不会乱序
    private int index_M;                                                          //鼠标索引
    private MaterialItem currentMaterial;                                              //当前高亮
    private MaterialItem targerMaterial;                                               //要切换到的目标
   
    private int zindex=20;                                                      //z 旋转增值
    private bool MouseInItem=true;                                              //鼠标中键是否操作item，false就是操作二级菜单
    private bool isBuild;                                                       //是否正在建设
    private GameObject currentModel = null;                                     //当前要创建的模型   prefabs
    private GameObject buildModel = null;                                       //移动的模型
    private RaycastHit hit;
    private Ray ray;
    private bool isCanPut = true;                                               //是否可以放置这个模型
    private bool rayStop;

    public bool RayStop
    {
        get { return rayStop; }
        set { rayStop = value; }
    }

    public bool IsCanPut
    {
        get { return isCanPut; }
        set { isCanPut = value; }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start () {
        Init();
        CreateItem();
        ShowOrCloseWheelBG();
    }

    void Update()
    {
        
        //创建实体
        if (Input.GetMouseButtonDown(0) && currentModel != null)
        {
            Debug.Log("执行~sasssssssssssssssssssssssssssssssssss"+ IsCanPut);
            //如果可以摆放
            if (IsCanPut)
            {
                if (buildModel != null)
                {
                    //把颜色给变回来
                    buildModel.GetComponent<BuildMaterialsBase>().Normal();
                    //销毁墙壁Transform
                    if (buildModel.GetComponent<Wall>() && buildModel.GetComponent<Wall>().IsOnPlatform == true)
                    {
                        buildModel.GetComponent<Wall>().DestroyOnTransform();
                    }
                    //暂时先销毁这个脚本防止颜色交互
                    Destroy(buildModel.GetComponent<BuildMaterialsBase>());
                }
                //生成！
                buildModel = Instantiate<GameObject>(currentModel,m_BuildPanelView.Player_Transform.position + new Vector3(0, 0, 10), Quaternion.identity,m_BuildPanelView.BuildModelParent);
               
                //射线继续
                RayStop = false;
            }
            //改变标志位，一直为true调用下面的方法就会隐藏当前扇形UI
            ShowWheelBG = true;
            ShowOrCloseWheelBG();
            isBuild = true;
        }
        //取消建造
        if (Input.GetMouseButtonDown(1) && isBuild ==true)
        {
            if (buildModel != null)
            {
                //重置属性
                Destroy(buildModel);
                currentModel = null;
                buildModel = null;
                isBuild = false;
                currentMaterial.Normal();
                currentMaterial = null;
                TransNameText();
                MouseInItem = true;
                IsCanPut = true;
            }
        }
        //当扇形UI展开
        if (ShowWheelBG==true)
        {
            //退出一级菜单item，进入二级菜单material
            if (Input.GetMouseButtonDown(0) && MouseInItem)
            {
                if (targerItem == null) return;
                //是空的就不让进来
                if (targerItem.MaterialList.Count==0) return;
                Debug.Log("退出一级菜单item，进入二级菜单material");
                MouseInItem = false;
                //文字变化
                TransNameTextMaterial();
                HightLightMaterialBG(true);
            }
            //退出二级菜单material，进入一级菜单item
            if (Input.GetMouseButtonDown(1) && MouseInItem ==false)
            {
                Debug.Log("退出二级菜单material，进入一级菜单item");
                currentMaterial.Normal();
                currentMaterial = null;
                TransNameText();
                MouseInItem = true;
                currentModel = null;
            }
            //切换显示Item背景
            if (Input.GetAxis("Mouse ScrollWheel")!=0 && MouseInItem)
            {
                HightLightItemBG();
            }
            //切换高亮material
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && MouseInItem ==false)
            {
                HightLightMaterialBG(false);
            }
          
        }

        SetModelPos();
    }

    /// <summary>
    /// 默认初始化
    /// </summary>
    private void Init()
    {
        m_BuildPanelModel = gameObject.GetComponent<BuildPanelModel>();
        m_BuildPanelView = gameObject.GetComponent<BuildPanelView>();
    }

    /// <summary>
    /// 创建所有Item
    /// </summary>
    private void CreateItem()
    {
        for (int i = 0; i <m_BuildPanelModel.ItemSpriteList.Count; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(m_BuildPanelView.BuildItem, m_BuildPanelView.WheelBG_Transform);
            BuildItemList.Add(go.GetComponent<BuildItem>());
            if (m_BuildPanelModel.ItemSpriteList[i] == null)
            {
                go.GetComponent<BuildItem>().Init(Quaternion.Euler(new Vector3(0, 0, i * 360 / m_BuildPanelModel.ItemSpriteList.Count)), true, m_BuildPanelModel.ItemSpriteList[i], false);
            }
            else
            {
                go.GetComponent<BuildItem>().Init(Quaternion.Euler(new Vector3(0, 0, i * 360 / m_BuildPanelModel.ItemSpriteList.Count)), false, m_BuildPanelModel.ItemSpriteList[i], true);
                //在Item不为空的时候创建子元素
                for (int j = 0; j < m_BuildPanelModel.ItemIconList[i].Length; j++)
                {
                    //计算旋转角度   20+=起始角度+360/9个item/3个子类
                    zindex += 360 / m_BuildPanelModel.ItemSpriteList.Count / m_BuildPanelModel.ItemIconList[i].Length;
                    //Debug.Log(zindex);
                    if (m_BuildPanelModel.ItemIconList[i][j]!=null)
                    {
                        //首先要设置父物体（为整个圈的子类），不然转不了圈。
                        GameObject material = Instantiate<GameObject>(m_BuildPanelView.BuildMaterial, m_BuildPanelView.WheelBG_Transform);
                        material.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, zindex));
                        material.GetComponent<Transform>().Find("Icon").GetComponent<Image>().sprite = m_BuildPanelModel.ItemIconList[i][j];
                        material.GetComponent<Transform>().Find("Icon").GetComponent<Transform>().rotation = Quaternion.Euler(Vector3.zero);
                        //转完圈之后再设置父物体
                        material.GetComponent<Transform>().SetParent(go.GetComponent<Transform>());
                        //添加带集合中统一管理
                        go.GetComponent<BuildItem>().AddMaterialInList(material);
                    }
                }
                //创建完之后隐藏
                go.GetComponent<BuildItem>().CloseBG();
            }

        }
        //第一个BuildItemList[0]
        currentItem = BuildItemList[0];
        //改变名字先
        TransNameText();
    }

    /// <summary>
    /// 显示与隐藏UI
    /// </summary>
    public void ShowOrCloseWheelBG ()
    {
        if (ShowWheelBG == true)
        {
            m_BuildPanelView.WheelBG_Transform.gameObject.SetActive(false);
            ShowWheelBG = false;
        }
        else
        {
            m_BuildPanelView.WheelBG_Transform.gameObject.SetActive(true);
            ShowWheelBG = true;
        }
    }


    /// <summary>
    /// 高亮选中的UI背景（鼠标逻辑）--item
    /// </summary>
    private void HightLightItemBG()
    {
        //乘5是因为鼠标滚轮太慢
        mouseValue += Input.GetAxis("Mouse ScrollWheel")*5;
        index = Mathf.Abs((int)mouseValue) % m_BuildPanelModel.ItemSpriteList.Count;
        Debug.Log(index);
        targerItem = BuildItemList[index];
        if (currentItem!= targerItem)
        {
            targerItem.ShowBG();
            currentItem.CloseBG();
            currentItem = targerItem;
        }
        TransNameText();
    }

    /// <summary>
    /// 高亮选中的材料背景（鼠标逻辑）--Material
    /// </summary>
    /// <param name="FirstShow">是否默认显示第一个</param>
    private void HightLightMaterialBG(bool FirstShow)
    {
        mouseValue_M += Input.GetAxis("Mouse ScrollWheel") * 5;
        index_M = Mathf.Abs((int)mouseValue_M) % m_BuildPanelModel.ItemSpriteList.Count;
        Debug.Log(index_M);
        if (FirstShow)
        {
            Debug.Log(index+"-----------"+index_M % targerItem.MaterialList.Count);
            targerMaterial = targerItem.MaterialList[targerItem.MaterialList.Count-1].GetComponent<MaterialItem>();
            currentModel = m_BuildPanelModel.MaterialsModelList[index][index_M % targerItem.MaterialList.Count];
            Debug.Log("当前的模型为：" + currentModel.name);
        }
        else
        {  //targerItem_M 等于一级菜单的MaterialList中的某一个数
            targerMaterial = targerItem.MaterialList[index_M % targerItem.MaterialList.Count].GetComponent<MaterialItem>();
        }
        if (currentMaterial != targerMaterial)
        {
            targerMaterial.Height();
            currentModel = m_BuildPanelModel.MaterialsModelList[index][index_M % targerItem.MaterialList.Count];
            Debug.Log("当前的模型为："+currentModel.name);
            if (currentMaterial != null)
                currentMaterial.Normal();
            currentMaterial = targerMaterial;
        }
        TransNameTextMaterial();
    }


    /// <summary>
    /// 改变扇形UI中间的文字--item
    /// </summary>
    private void TransNameText()
    {
        m_BuildPanelView.NameText.text = m_BuildPanelModel.Names[index % BuildItemList.Count];
    }

    /// <summary>
    /// 改变扇形UI中间的文字--material
    /// </summary>
    private void TransNameTextMaterial()
    {
        m_BuildPanelView.NameText.text =m_BuildPanelModel.MaterialsName[index % BuildItemList.Count][index_M % targerItem.MaterialList.Count];
    }




    /// <summary>
    /// 设置模型的位置
    /// </summary>
    private void SetModelPos()
    {
        if (RayStop)
            return;
        ray = m_BuildPanelView.ENV_Camera.ScreenPointToRay(Input.mousePosition);
        //最远检测距离为15米,无视第13层的碰撞检测（模型）
        if (Physics.Raycast(ray, out hit, 15 ,~(1<<13)))
        {
            if (buildModel != null)
            {
                if (buildModel.GetComponent<BuildMaterialsBase>()!=null)
                {
                    //没有吸附属性，可以移动
                    if (buildModel.GetComponent<BuildMaterialsBase>().IsAttach == false)
                    {
                        //if (isCanPut==false)
                        //{
                            buildModel.GetComponent<Transform>().position = hit.point;
                        //}
                    }
                    //鼠标与模型离开一定距离就无吸附属性
                    if (Vector3.Distance(hit.point, buildModel.GetComponent<Transform>().position) > 1)
                    {
                        buildModel.GetComponent<BuildMaterialsBase>().IsAttach = false;
                    }
                }
            }    
        }
    }
}
