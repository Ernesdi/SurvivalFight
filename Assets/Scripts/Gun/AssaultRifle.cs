using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 步枪武器C层
/// </summary>
public class AssaultRifle : WeapenControllerBase
{
    //V层查找
    private AssaultRifleView m_AssaultRifleView;
    //子弹预制体
    private GameObject shellEffect;

    //对象池脚本数组
    private ObjectPool[] pools;

   

    protected override void Init()
    {
        //每个弹夹的弹数
        cartridgeNum = 30;
        //向下转型 里氏转换原则  这样C层的子类就不需要重新继承V层子类 因为C层父类继续了V层父类
        m_AssaultRifleView = (AssaultRifleView)M_GunViewBase;

        shellEffect = Resources.Load<GameObject>("Prefabs/Gun/Shell");

        pools = gameObject.GetComponents<ObjectPool>();
    }

    /// <summary>
    /// 播放特效(子类特有)弹壳特效和枪口特效
    /// </summary>
    protected override void PlayEffect()
    {
        GunPointEffect();
        ShellEffect();
    }

    /// <summary>
    /// 枪口特效
    /// </summary>
    private void GunPointEffect() {
        GameObject gunPoint = null;
        //如果对象池1中有数据
        if (pools[0].Data())
        {
            gunPoint = pools[0].GetObject();
            //重置一些属性
            gunPoint.GetComponent<Transform>().position = m_AssaultRifleView.FireEffect_Transform.position;
        }
        else
        {
            //实例化枪口特效 没有勾选自动播放所以需要手动play
            gunPoint = Instantiate<GameObject>(FireEffect, m_AssaultRifleView.FireEffect_Transform.position, Quaternion.identity, m_AssaultRifleView.GunPointEffectParent);
            gunPoint.name = "GunPointEffect";
        }
        gunPoint.GetComponent<ParticleSystem>().Play();
        StartCoroutine(Delay(pools[0], gunPoint, 1));
    }

    /// <summary>
    /// 弹壳弹出特效
    /// </summary>
    private void ShellEffect() {
        GameObject shell = null;
        if (pools[1].Data())
        {
            shell = pools[1].GetObject();
            //重置一些属性  isKinematic的意思是是否使用动力学？受到力的影响 
            //因为对象池中的带有Rigidbody的对象有特殊性 所以重置对象的位置需要把使用动力学√上 使用完之后关掉再送进对象池
            shell.GetComponent<Rigidbody>().isKinematic = true;
            shell.GetComponent<Transform>().position = m_AssaultRifleView.BulletEffect_Transform.position;
            shell.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            //实例化弹壳特效  
            shell = Instantiate<GameObject>(shellEffect, m_AssaultRifleView.BulletEffect_Transform.position, Quaternion.identity, m_AssaultRifleView.BulletParentTransform);
            shell.name = "Bullet";
        }
        shell.GetComponent<Rigidbody>().AddForce(m_AssaultRifleView.BulletEffect_Transform.up * 50);
        StartCoroutine(Delay(pools[1], shell, 3));
    }




    /// <summary>
    /// 射击(子类特有)  射击判断（弓箭啊  枪械啊  命中判断不一样 实例化的子弹也不一样）
    /// </summary>
    protected override void Shoot() {
           
            //如果ShootReady返回的点不是0
            if (Hit.point!= Vector3.zero)
            {
                //如果有BulletMarks这个组件 
                if (Hit.collider.GetComponent<BulletMarks>()!=null)
                {//调用里面的方法
                    Hit.collider.GetComponent<BulletMarks>().CreateBulletMark(Hit);
                    Hit.collider.GetComponent<BulletMarks>().Hp -= Demage;
                }
                //与AI进行交互
                if (Hit.collider.GetComponentInParent<AI>()!=null)
                {
                    //父类的Hit
                    Hit.collider.GetComponentInParent<AI>().PlayFleshEffect(Hit);
                    if (Hit.collider.gameObject.name == "Head")
                    {
                        Hit.collider.GetComponentInParent<AI>().HeadHit(Demage*2);
                    }
                    else
                    {
                        Hit.collider.GetComponentInParent<AI>().NormalHit(Demage);
                    }
                    //实例化子弹在射线碰撞的位置
                    //GameObject go = Instantiate<GameObject>(m_AssaultRifleView.Prefab_bullet, Hit.point, Quaternion.identity);
                    //go.transform.SetParent(Hit.collider.GetComponent<Transform>());
                }
                //既没有与AI交互也没有与障碍物交互
                if (Hit.collider.GetComponentInParent<AI>() == null && Hit.collider.GetComponent<BulletMarks>() == null)
                {
                    //实例化子弹在射线碰撞的位置
                    Instantiate<GameObject>(m_AssaultRifleView.Prefab_bullet, Hit.point, Quaternion.identity);
                }
        }
      
    }

    protected override void InitFireClip()
    {
        FireClip = Resources.Load<AudioClip>("Soures/AssaultRifle_Fire");
    }

    protected override void InitFireEffect()
    {
        FireEffect = Resources.Load<GameObject>("Effects/Gun/AssaultRifle_GunPoint_Effect");
    }

    protected override void ReloadBullet()
    {
            m_AssaultRifleView.M_Animator.SetTrigger("reloadBullet");
            Debug.Log("没有换弹动作所以会报警告,触发换弹");
    }

    protected override void SwitchFireModel()
    {
        //播放一个声音
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Soures/SwitchGunModel"), m_AssaultRifleView.BulletEffect_Transform.position);
        Debug.Log("切换开火模式");
        //读取动作 因为缺少动作所以会报警告
        m_AssaultRifleView.M_Animator.SetTrigger("switchFireModel");
    }
}
