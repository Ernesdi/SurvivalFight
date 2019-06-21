using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingRifle : WeapenControllerBase
{

    //V层查找
    private HuntingRifleView m_HuntingRifleView;

    protected override void Init()
    {
        //每个弹夹的弹数
        cartridgeNum = 5;
        //向下转型 里氏转换原则  这样C层的子类就不需要重新继承V层子类 因为C层父类继续了V层父类
        m_HuntingRifleView = (HuntingRifleView)M_GunViewBase;
        SingleFire = true;
        isSniper = true;
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
        GameObject tempEffect = Instantiate<GameObject>(FireEffect, M_GunViewBase.FireEffect_Transform.position, Quaternion.identity);
        tempEffect.GetComponent<ParticleSystem>().Play();
        StartCoroutine(Delay(tempEffect, 3));

        //弹壳弹出特效
        GameObject tempShell = Instantiate<GameObject>(m_HuntingRifleView.BulletEffect_Shell, m_HuntingRifleView.BulletEffect_Transform.position, Quaternion.identity);
        tempShell.GetComponent<Rigidbody>().AddForce(m_HuntingRifleView.BulletEffect_Transform.up * 70);
        StartCoroutine(Delay(tempShell, 3));
    }

    protected override void ReloadBullet()
    {
        m_HuntingRifleView.M_Animator.SetTrigger("reloadBullet");
        Debug.Log("触发狙击枪换弹");
    }

    protected override void Shoot()
    {
        GameObject tempBullet = Instantiate<GameObject>(m_HuntingRifleView.Prefab_bullet, m_HuntingRifleView.FireEffect_Transform.position, Quaternion.identity);
        tempBullet.GetComponent<ShotgunBullet>().Shoot(m_HuntingRifleView.FireEffect_Transform.forward, 5000, Demage, Hit);
    }

    protected override void SwitchFireModel()
    {
        
    }

    /// <summary>
    /// 不使用对象池 直接销毁当前物体
    /// </summary>
    IEnumerator Delay(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }

    /// <summary>
    /// 狙击枪开一枪换弹特效
    /// </summary>
    private void ResetBulletEffectClip()
    {
        AudioSource.PlayClipAtPoint(m_HuntingRifleView.BulletEffect_Clip, m_HuntingRifleView.BulletEffect_Transform.position);
    }
}
