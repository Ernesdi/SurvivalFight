using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 枪械模块V层抽象类（父类）
/// </summary>
public abstract class GunViewBase : MonoBehaviour {

    //三个基础属性
    private Transform m_Transform;
    private Animator m_Animator;
    private Camera m_EnvCamera;

    //优化开镜动作
    private Vector3 startPos;
    private Vector3 startRot;
    private Vector3 endPos;
    private Vector3 endRot;

    //准心位置
    private Transform gunStar;
    //用于实例化的准心
    private GameObject prefab_GunStar;
    //枪口
    private Transform fireEffect_Transform;
   

    //封装
    public Transform M_Transform { get { return m_Transform; } }
    public Animator M_Animator { get { return m_Animator; } }
    public Camera M_EnvCamera { get { return m_EnvCamera; } }

    public Vector3 StartPos { get { return startPos; } set { startPos = value; } }
    public Vector3 StartRot { get { return startRot; } set { startRot = value; } }
    public Vector3 EndPos { get { return endPos; } set { endPos = value; } }
    public Vector3 EndRot { get { return endRot; } set { endRot = value; } }

    public Transform GunStar { get { return gunStar; } }
    public Transform FireEffect_Transform { get { return fireEffect_Transform; } set { fireEffect_Transform = value; } }

    //公开Awake方法用于子类AWAKE进行时父类已经调用了
    protected void Awake()
    {
        //基础属性的查找
        m_Transform = gameObject.GetComponent<Transform>();
        m_Animator = gameObject.GetComponent<Animator>();
        m_EnvCamera = GameObject.Find("EnvCamera").GetComponent<Camera>();

        prefab_GunStar = Resources.Load<GameObject>("Prefabs/Gun/GunStar");
        //实例化
        gunStar = Instantiate<GameObject>(prefab_GunStar, GameObject.Find("MainPanel").GetComponent<Transform>()).GetComponent<Transform>();

        //在父类中的AWAKE中调用子类就不用管在什么时候调用重写的方法了  只限V层查找
        InitHoldPostPos();
        FindGunPointTransform();
        Init();
    }

    private void OnEnable()
    {
        Debug.Log("执行OnEnable");
        //显示准心
        gunStar.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        Debug.Log("执行OnDisable");
        //如果准心能找的到 就隐藏  因为退出程序的那一刻会调用这个方法 
        //但是gunstar在内存中的信息又被消除了 所有会发生空引用异常
        gunStar.gameObject.SetActive(false);
    }


    /// <summary>
    /// 进入到开镜状态
    /// </summary>
    public void EnterHoldPose(float time=0.2f,int fov=40)
    {
        //正对屏幕
        M_Transform.DOLocalMove(EndPos, time);
        M_Transform.DOLocalRotate(EndRot, time);
        //放大环境射线机
        M_EnvCamera.DOFieldOfView(fov, time);
    }

    /// <summary>
    /// 退出到开镜射击状态
    /// </summary>
    public void ExitHoldPose(float time = 0.2f, int fov = 60)
    {
        //摆回正常状态
        M_Transform.DOLocalMove(StartPos, time);
        M_Transform.DOLocalRotate(StartRot, time);
        //环境摄像机缩小
        M_EnvCamera.DOFieldOfView(fov, time);

    }


    //子类需要实现的初始化方法
    protected abstract void Init();
    //定义枪口位置的初始化抽象方法
    protected abstract void FindGunPointTransform();
    //定义瞄准的初始化抽象方法
    protected abstract void InitHoldPostPos();
}
