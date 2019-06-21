using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
/// <summary>
/// 游戏控制类
/// </summary>
public class GameController : MonoBehaviour {

    private bool InventoryPanelShow = false;

    //控制脚本
    private FirstPersonController m_FirstPersonController;

    void Start()
    {
        InventoryPanelController.Instance.HideCurrentPanel();
        FindInit();
    }

    void Update()
    {
        InventoryPanelKey();
        BuildKey();
        //当背包是打开状态时不执行数字键盘的检测
        if (InventoryPanelShow==false)
        {
            ToolBarKey();
        }
    }

    /// <summary>
    /// 查找初始化
    /// </summary>
    private void FindInit() {
        m_FirstPersonController = GameObject.Find("FPSController").GetComponent<FirstPersonController>();
    }

    /// <summary>
    /// 背包按钮检测
    /// </summary>
    private void InventoryPanelKey() {
        if (Input.GetKeyDown(GameConst.InventoryPanelKey))
        {
            if (InventoryPanelShow)
            {
                Debug.Log("不显示");
                InventoryPanelController.Instance.HideCurrentPanel();
                //角色模型的显示
                if (ToolBarController.Instance.TempModel!=null)
                {
                    ToolBarController.Instance.TempModel.SetActive(true);
                }
                m_FirstPersonController.enabled = true;
            }
            else
            {
               
                Debug.Log("显示");
                InventoryPanelController.Instance.ShowCurrentPanel();
                //角色模型的隐藏
                if (ToolBarController.Instance.TempModel != null)
                {
                    ToolBarController.Instance.TempModel.SetActive(false);
                }
                m_FirstPersonController.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            InventoryPanelShow = !InventoryPanelShow;
        }
        //按下整理背包按钮
        if (Input.GetKeyDown(GameConst.BagArrangeKey))
        {
            InventoryPanelController.Instance.BagArrange();
        }
        //按下背包材料ID相同的融合按钮
        if (Input.GetKeyDown(GameConst.BagIdFuseKey))
        {
            InventoryPanelController.Instance.IdFuse();
        }

    }



    /// <summary>
    /// 工具栏按钮检测
    /// </summary>
    private void ToolBarKey() {
        ToolBarCommonKey(GameConst.ToolBarKey1,0);
        ToolBarCommonKey(GameConst.ToolBarKey2,1);
        ToolBarCommonKey(GameConst.ToolBarKey3,2);
        ToolBarCommonKey(GameConst.ToolBarKey4,3);
        ToolBarCommonKey(GameConst.ToolBarKey5,4);
        ToolBarCommonKey(GameConst.ToolBarKey6,5);
        ToolBarCommonKey(GameConst.ToolBarKey7,6);
        ToolBarCommonKey(GameConst.ToolBarKey8,7);
    }


    /// <summary>
    /// 工具栏按钮检测按键通用
    /// </summary>
    /// <param name="keyNum"></param>
    private void ToolBarCommonKey(KeyCode keyNum,int indexOfSlotList) {
        if (Input.GetKeyDown(keyNum))
        {
            ToolBarController.Instance.ManagerSlotActiveOnKey(indexOfSlotList);
        }
    }


    /// <summary>
    /// 检测建造按钮
    /// </summary>
    private void BuildKey()
    {
        //打开建造UI
        if (Input.GetKeyDown(GameConst.OpenBuildUI))
        {
            BuildPanelController.Instance.ShowOrCloseWheelBG();
        }
    }

}
