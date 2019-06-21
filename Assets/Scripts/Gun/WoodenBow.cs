using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 弓箭C层
/// </summary>
public class WoodenBow : ThrowerControllerBase
{

    private WoodenBowView m_WoodenBowView;

    protected override void Init()
    {
        m_WoodenBowView = (WoodenBowView)M_GunViewBase;
        //不允许射击
        CanShoot(0);
    }

    protected override void InitFireClip()
    {
        FireClip = Resources.Load<AudioClip>("Soures/Arrow Release");
    }

    protected override void Shoot()
    {
        GameObject go = Instantiate<GameObject>(m_WoodenBowView.Prefab_Arrow, m_WoodenBowView.FireEffect_Transform.position, m_WoodenBowView.FireEffect_Transform.rotation);
        go.GetComponent<WoodenBowArrow>().Shoot(m_WoodenBowView.FireEffect_Transform.forward, 3000, Demage , Hit);
    }
}
