using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹父类 
/// </summary>

public abstract class BulletBase : MonoBehaviour
{
    //基础字段
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;
    private int demage;

    //属性封装
    public Transform M_Transform { get { return m_Transform; } }
    public Rigidbody M_Rigidbody { get { return m_Rigidbody; } }
    public int Demage { get { return demage; } set { demage = value; } }

    //生命周期函数
    private void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        Init();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEnter(collision);
    }

    //自杀方法
    public void KillSelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 尾巴颤动动画
    /// </summary>
    /// <returns></returns>
    public IEnumerator TileAnimation(Transform pivot_Transform)
    {
        //定义停止时间
        float stopTime = Time.time + 1.0f;
        //定义开始时位置尾巴的旋转角度
        Quaternion startRot = Quaternion.Euler(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0));
        //定义旋转范围
        float range = 1.0f;
        //无用ref参数 用于SmoothDamp第三个参数
        float vel = 0;
        //如国当前时间小于停止时间就执行
        while (Time.time < stopTime)
        {
            //定义旋转角度
            Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0)) * startRot;
            //设置旋转角度
            pivot_Transform.localRotation = rot;
            //平滑阻尼
            range = Mathf.SmoothDamp(range, 0, ref vel, stopTime - Time.time);
            yield return null;
        }
    }

    //抽象初始化方法
    public abstract void Init();
    //抽象射击方法
    public abstract void Shoot(Vector3 pos, int force, int demage,RaycastHit hit);
    //抽象碰撞方法
    public abstract void CollisionEnter(Collision collision);
}
