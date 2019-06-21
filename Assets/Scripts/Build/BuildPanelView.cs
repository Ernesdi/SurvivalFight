using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanelView : MonoBehaviour {

    private Transform wheelBG_Transform;
    private Transform m_Transform;
    private Transform player_Transform;     //玩家的位置
    private Transform buildModelParent;

    private Text nameText;                  //当前所高亮名字
    private Camera env_Camera;

    private GameObject buildItem;            //预制体加载    
    private GameObject buildMaterial;       //建造材料UI

    public Transform WheelBG_Transform
    {
        get { return wheelBG_Transform; }
    }
    public Transform M_Transform
    {
        get { return m_Transform; }
    }
    public Transform Player_Transform
    {
        get { return player_Transform; }
    }
    public Transform BuildModelParent
    {
        get { return buildModelParent; }
    }
    public Text NameText
    {
        get { return nameText; }
    }
    public Camera ENV_Camera
    {
        get { return env_Camera; }
    }
    public GameObject BuildItem
    {
        get { return buildItem; }
    }
    public GameObject BuildMaterial
    {
        get { return buildMaterial; }
    }

    void Awake () {
        Init();
    }

    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        //默认查找的就是Transform组件
        wheelBG_Transform = m_Transform.Find("WheelBG");
        player_Transform = GameObject.Find("FPSController").GetComponent<Transform>();
        buildModelParent = GameObject.Find("BuildModelParent").GetComponent<Transform>();

        nameText = m_Transform.Find("WheelBG/ItemName").GetComponent<Text>();
        env_Camera = GameObject.Find("FPSController/ModelCamera/EnvCamera").GetComponent<Camera>();


        buildItem = Resources.Load<GameObject>("Build/Prefabs/BuildItem");
        buildMaterial = Resources.Load<GameObject>("Build/Prefabs/BuildMaterial");
    }

}
