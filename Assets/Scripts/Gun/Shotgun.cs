using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 散弹枪C层
/// </summary>
public class Shotgun : WeapenControllerBase
{

    //V层查找
    private ShotgunView m_ShotgunView;

    protected override void Init()
    {
        //每个弹夹的弹数
        cartridgeNum = 5;
        m_ShotgunView = (ShotgunView)M_GunViewBase;
    }

    protected override void InitFireClip()
    {
        FireClip = Resources.Load<AudioClip>("Soures/Shotgun_Fire");
    }

    protected override void InitFireEffect()
    {
        FireEffect = Resources.Load<GameObject>("Effects/Gun/Shotgun_GunPoint_Effect");
    }

    protected override void PlayEffect()
    {
        //枪口特效
        GameObject tempEffect = Instantiate<GameObject>(FireEffect,M_GunViewBase.FireEffect_Transform.position,Quaternion.identity);
        tempEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(Delay(tempEffect, 3));

        //弹壳弹出特效
        GameObject tempShell = Instantiate<GameObject>(m_ShotgunView.BulletEffect_Shell, m_ShotgunView.BulletEffect_Transform.position, Quaternion.identity);
        Debug.Log("散弹弹壳弹出特效"+tempShell);
        tempShell.GetComponent<Rigidbody>().AddForce(m_ShotgunView.BulletEffect_Transform.up*70);
        StartCoroutine(Delay(tempShell, 3));
    }

    protected override void ReloadBullet()
    {
        m_ShotgunView.M_Animator.SetTrigger("reloadBullet");
        Debug.Log("触发散弹枪换弹");
    }

    protected override void Shoot()
    {
        StartCoroutine("CreateBullet");
        
    }

    protected override void SwitchFireModel()
    {
        
    }

    /// <summary>
    /// 定义协程生成散弹子弹 为了一开始不碰撞在一起触发睡眠
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateBullet() {
        for (int i = 0; i < 5; i++)
        {
            //偏移量   散弹枪发射子弹
            Vector3 offset = new Vector3(Random.Range(-0.05f,0.05f), Random.Range(-0.05f, 0.05f),0);
            GameObject tempBullet = Instantiate<GameObject>(m_ShotgunView.Prefab_bullet, m_ShotgunView.FireEffect_Transform.position, Quaternion.identity);
            tempBullet.GetComponent<ShotgunBullet>().Shoot(m_ShotgunView.FireEffect_Transform.forward + offset, 3000,Demage/5, Hit);
            //tempBullet.GetComponent<ShotgunBullet>().Shoot(m_ShotgunView.FireEffect_Transform.forward , 3000);
            yield return new WaitForSeconds(0.03f);
        }
       
    }

    /// <summary>
    /// 不使用对象池 直接销毁当前物体
    /// </summary>
    IEnumerator Delay(GameObject go,float time) {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }


    /// <summary>
    /// 散弹枪开一枪换弹特效
    /// </summary>
    private void ResetBulletEffectClip() {
        AudioSource.PlayClipAtPoint(m_ShotgunView.BulletEffect_Clip, m_ShotgunView.BulletEffect_Transform.position);
    }
}
