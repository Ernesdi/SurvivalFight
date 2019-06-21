using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 枪械模块V层抽象类（父类）
/// </summary>
public abstract class GunControllerBase : MonoBehaviour {

  
    //枪械的数值（字段）
    [SerializeField] private int id;
    [SerializeField] private int demage;
    [SerializeField] private int durable;

    //耐久值固定值
    protected float durable_2;
    [SerializeField] private WeapenType gunType;
    //血条UI
    private GameObject gun_UI;


    //枪械的资源
    private AudioClip fireClip;
    private GameObject fireEffect;

    //C层父类持有V层父类
    protected GunViewBase m_GunViewBase;

    //射线相关
    private Ray ray;
    private RaycastHit hit;

    //是否可以设计
    protected bool canShoot=true;

    //是否是狙击枪
    protected bool isSniper;

    #region  属性封装


    //枪械数值属性
    public int Id { get { return id; } set { id = value; } }
    public int Demage {get { return demage; }set { demage = value; } }
    public WeapenType GunType { get { return gunType; } set { gunType = value; } }

    public GameObject Gun_UI { get { return gun_UI; }set { gun_UI = value; } }
    //耐久值归零时
    public int Durable
    {
        get { return durable; }
        set
        {
            durable = value;
            if (durable <= 0)
            {
                //销毁当前物体
                Destroy(gameObject);
                //销毁准心
                Destroy(m_GunViewBase.GunStar.gameObject);
                //隐藏狙击镜
                ToolBarController.Instance.ChangeSniperAimState(false);
                //退出瞄准模式
                m_GunViewBase.ExitHoldPose();
            }
        }
    }

    public AudioClip FireClip { get { return fireClip; } set { fireClip = value; } }
    public GameObject FireEffect { get { return fireEffect; } set { fireEffect = value; } }

    public GunViewBase M_GunViewBase { get { return m_GunViewBase; }set { m_GunViewBase = value; } }

    public Ray MyRay { get { return ray; }set { ray = value; } }
    public RaycastHit Hit { get { return hit; }set { hit = value; } }

    #endregion


    protected virtual void Start()
    {
        Debug.Log(durable);
        durable_2 = Durable;
        m_GunViewBase = gameObject.GetComponent<GunViewBase>();
        Init();
        InitFireClip();
    }

    private void FixedUpdate()
    {
        ShootReady();
        Controller();
    }

    /// <summary>
    /// 更新枪械血条UI
    /// </summary>
    private void UpDataUI() {
        gun_UI.GetComponent<InventoryItemManager>().UpdataUI(Durable/durable_2);
    }


    /// <summary>
    /// 放下武器
    /// </summary>
    public void Holster()
    {
        m_GunViewBase.M_Animator.SetTrigger("holster");
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    protected void PlayClip()
    {
        AudioSource.PlayClipAtPoint(FireClip, m_GunViewBase.FireEffect_Transform.position);
    }

    /// <summary>
    /// 是否可以射击 用于fire动作事件
    /// </summary>
    protected void CanShoot(int state) {
        if (state==1)
        {
            canShoot = true;
        }
        else
            canShoot = false;
    }

    /// <summary>
    /// 射击准备
    /// </summary>
    protected void ShootReady()
    {
        //Debug.DrawRay(m_AssaultRifleView.FireEffect_Transform.position, m_AssaultRifleView.FireEffect_Transform.forward,Color.green);
        ray = new Ray(m_GunViewBase.FireEffect_Transform.position, m_GunViewBase.FireEffect_Transform.forward);
        Debug.DrawLine(m_GunViewBase.FireEffect_Transform.position, m_GunViewBase.FireEffect_Transform.forward * 500, Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            //世界点转换为屏幕点
            Vector2 tempPos = RectTransformUtility.WorldToScreenPoint(m_GunViewBase.M_EnvCamera, hit.point);
            
            //更新准心
            m_GunViewBase.GunStar.position = tempPos;
        }
        else
        {
            hit.point = Vector3.zero;
            //Debug.Log("没有碰撞");
        }
    }

    /// <summary>
    /// 协程，让对象回归到对象池中
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Delay(ObjectPool pool, GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        pool.AddObject(go);
    }


    /// <summary>
    /// 控制
    /// </summary>
    protected virtual void Controller()
    {

        //开镜  持续按下右键
        if (Input.GetMouseButton(1))
        {
            //动作
            m_GunViewBase.M_Animator.SetBool("holdPose", true);
            if (isSniper)
            {
                ToolBarController.Instance.ChangeSniperAimState(true);
                m_GunViewBase.EnterHoldPose(0.2f, 20);
            }
            else
            m_GunViewBase.EnterHoldPose();
            ////准心隐藏
            //m_GunViewBase.GunStar.gameObject.SetActive(false);
        }
        //关镜 松开右键
        if (Input.GetMouseButtonUp(1))
        {
            //动作
            m_GunViewBase.M_Animator.SetBool("holdPose", false);
            if (isSniper)
            {
                ToolBarController.Instance.ChangeSniperAimState(false);
            }
            m_GunViewBase.ExitHoldPose();
            //准心显示
            m_GunViewBase.GunStar.gameObject.SetActive(true);
        }
      
    }



    protected virtual void MouseLeftDown() {
        //设置动画
        m_GunViewBase.M_Animator.SetTrigger("fire");
        Shoot();
        PlayClip();
        Durable--;
        //更新枪械血条UI
        UpDataUI();
    }

    //子类需要实现的初始化方法
    protected abstract void Init();
    //子类需要实现的初始化的抽象方法
    protected abstract void InitFireClip();


    //子类需要实现的特有的抽象方法
    protected abstract void Shoot();
  

}
