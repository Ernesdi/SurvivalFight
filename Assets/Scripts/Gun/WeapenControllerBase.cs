using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模式：观察者模式  此类是被观察者(主播)    粉丝关注的是主播 粉丝是观察者
public delegate void ShootNumDel(int shootNum);
public delegate void DurableNumDel(WeapenControllerBase wcb);
public delegate void SingleFireDel(bool singleFire);

/// <summary>
/// 枪械武器C层
/// </summary>
public abstract class WeapenControllerBase : GunControllerBase
{
    //连续射击
    private bool continueShoot = true;
    //单发射击
    private bool singleFire;

    public static WeapenControllerBase WeapenInstance;
    //事件
    public event ShootNumDel UpdateShootNum;
    public event DurableNumDel UpdateDurableNum;
    public event SingleFireDel UpdateSingleFire;



    private int shootNum;         //射击数
    [SerializeField]
    protected int cartridgeNum;     //弹夹数

    private void Awake()
    {
        WeapenInstance = this;
    }

    public int ShootNum
    {
        get { return shootNum; }
        set { shootNum = value;
            //发送消息给粉丝
            UpdateShootNum(shootNum);
        }
    }
    public bool SingleFire
    {
        get { return singleFire; }
        set
        {
            singleFire = value;
            UpdateSingleFire(singleFire);
        }
    }


    protected override void Start()
    {
        base.Start();
        InitFireEffect();
    }

    protected override void MouseLeftDown()
    {
        base.MouseLeftDown();
        ShootNum++;
        PlayEffect();
        CheckBulletNum();
    }

    protected override void Controller()
    {
        base.Controller();
        if (singleFire)
        {
            //单发射击
            if (Input.GetMouseButtonDown(0) && canShoot)
            {
                MouseLeftDown();
            }
        }
        else
        {
            //持续射击
            if (Input.GetMouseButton(0) && canShoot)
            {
                if (continueShoot)
                {
                    MouseLeftDown();
                    continueShoot = false;
                    StartCoroutine("DelayShoot");
                }

            }
        }
        //手动换弹
        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {
            if (GunUIPanel.Instance.BackBullet==0)
                return;
            ReloadBullet();
            UpdateDurableNum(this);
            ShootNum = 0;
        }
        //切换开火模式
        if (Input.GetKeyDown(KeyCode.V))
        {
            bool temp = SingleFire;
            //切换开关
            SingleFire = !SingleFire;
            if (SingleFire == temp)
                return;
            SwitchFireModel();
        }
    }

    /// <summary>
    /// 检测子弹数
    /// </summary>
    private void CheckBulletNum()
    {
        if (ShootNum == cartridgeNum)
        {
            ReloadBullet();
            UpdateDurableNum(this);
            ShootNum = 0;
        }
    }

    public void ChangeCanShoot(bool isTrue) {
        canShoot = isTrue;
    }

    /// <summary>
    /// 每次射击的间隔
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayShoot()
    {
        yield return new WaitForSeconds(0.06f);
        continueShoot = true;
    }


    protected abstract void InitFireEffect();
    protected abstract void PlayEffect();
  
    //换弹
    protected abstract void ReloadBullet();

    //切换开火模式
    protected abstract void SwitchFireModel();
}
