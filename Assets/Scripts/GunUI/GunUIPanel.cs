using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 枪械UI  C层
/// </summary>
public class GunUIPanel : MonoBehaviour {

    public static GunUIPanel Instance;

    private GunUIPanelView gunUIPanelView;

    private int currentBullet;          //当前子弹数
    private int cartridgeNum;           //一个弹夹的数量
    private int durable;                //枪械耐久度(子弹总数)
    private int shootNum;               //当前射击数
    private int backBullet;             //后备弹夹数

    private float loadAmount = 0;          //加载的进度条的数值


    public int BackBullet
    {
        get
        {
            return backBullet;
        }
        set
        {
            backBullet = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start() {
        gunUIPanelView = gameObject.GetComponent<GunUIPanelView>();
        gunUIPanelView.M_Transform.gameObject.SetActive(false);
    }




    /// <summary>
    /// 初始化UI
    /// </summary>
    public void InitGunUI(GameObject item, int currentBullet, int cartridgeNum, int durable, bool isWeapenGun) {
        gunUIPanelView.M_Transform.gameObject.SetActive(true);
        gunUIPanelView.ReloadBullet.gameObject.SetActive(false);
        this.currentBullet = currentBullet;
        this.cartridgeNum = cartridgeNum;
        this.durable = durable;
        if (gunUIPanelView.Item_Transform.Find("InventoryItem") != null)
        {
            Destroy(gunUIPanelView.Item_Transform.Find("InventoryItem").gameObject);
        }
        GameObject go = Instantiate(item, gunUIPanelView.Item_Transform);
        go.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
        go.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //除自己以外全部隐藏
        Transform[] tempTransform = go.GetComponentsInChildren<Transform>();
        for (int i = 1; i < tempTransform.Length; i++)
        {
            tempTransform[i].gameObject.SetActive(false);
        }

        if (isWeapenGun)
        {
            gunUIPanelView.CurrentBullet.text = currentBullet.ToString();
            backBullet = durable - cartridgeNum;
            gunUIPanelView.CartridgeNum.text = "/" + backBullet;
            gunUIPanelView.FireModel.text = "全自动";

            //初始化完毕开始自动观察
            WeapenControllerBase.WeapenInstance.UpdateShootNum += Fens;
            WeapenControllerBase.WeapenInstance.UpdateDurableNum += Fens2;
            WeapenControllerBase.WeapenInstance.UpdateSingleFire += Fens3;
        }

    }

    //加到委托链中自动更新面板  当前弹夹的弹数
    private void Fens(int shootNum) {
        this.shootNum = shootNum;
        int tempNum = cartridgeNum - shootNum;
        gunUIPanelView.CurrentBullet.text = tempNum.ToString();
    }

    //加到委托链中自动更新面板  换弹
    private void Fens2(WeapenControllerBase wcb) {
        CalcBackBullet(wcb);
    }

    /// <summary>
    /// 计算后备弹
    /// </summary>
    private void CalcBackBullet(WeapenControllerBase wcb)
    {
        if (backBullet == 0)
             return;
        gunUIPanelView.ReloadBullet.gameObject.SetActive(true);

        if (backBullet <= cartridgeNum)
        {
            int tempNum = backBullet + currentBullet;
            if (tempNum > cartridgeNum)
            {
                Debug.Log("后备弹夹+当前弹夹剩余子弹数为：" + tempNum);
                cartridgeNum = currentBullet;
                Debug.Log("cartridgeNum：" + cartridgeNum + "backBullet:" + backBullet);
                backBullet = tempNum - currentBullet - shootNum;
                Debug.Log("backBullet：" + backBullet);
                if (backBullet < 0)
                {
                    cartridgeNum += backBullet;
                    backBullet = 0;
                }
            }
            else
            {
                cartridgeNum = backBullet;
                backBullet = 0;
            }
        }
        else
        {
            backBullet -= shootNum;
        }
        gunUIPanelView.CartridgeNum.text = "/" + backBullet;
        Debug.Log("当前备弹：" + backBullet);
       
        StartCoroutine("loadBullet", wcb);
    }


    /// <summary>
    /// 读取子弹
    /// </summary>
    IEnumerator loadBullet(WeapenControllerBase wcb) {
        wcb.ChangeCanShoot(false);
        while (loadAmount < 0.95f)
        {
            loadAmount += Random.Range(0.01f, 0.1f);
            gunUIPanelView.ReloadBullet.fillAmount = loadAmount;
            yield return new WaitForSeconds(0.1f);
        }
        gunUIPanelView.ReloadBullet.fillAmount = 1;
        loadAmount = 0;
        gunUIPanelView.ReloadBullet.gameObject.SetActive(false);
        wcb.ChangeCanShoot(true);
    }


    //加到委托链中自动更新面板  切换开火模式
    private void Fens3(bool isSingleFire)
    {
        if (isSingleFire)
        {
            gunUIPanelView.FireModel.text = "单发";
        }
        else
        {
            gunUIPanelView.FireModel.text = "全自动";
        }
       
    }
}
