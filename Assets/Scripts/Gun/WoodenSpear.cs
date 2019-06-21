using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 长矛C层
/// </summary>
public class WoodenSpear : ThrowerControllerBase
{

    private WoodenSpearView m_WoodenSpearView;

    protected override void Init()
    {
        m_WoodenSpearView = (WoodenSpearView)M_GunViewBase;
        CanShoot(0);
    }

    protected override void InitFireClip()
    {
        FireClip = Resources.Load<AudioClip>("Soures/Arrow Release");
    }

    protected override void Shoot()
    {
        GameObject go = Instantiate<GameObject>(m_WoodenSpearView.Prefab_Spear, m_WoodenSpearView.FireEffect_Transform.position, m_WoodenSpearView.FireEffect_Transform.rotation);
        go.GetComponent<WoodenBowArrow>().Shoot(m_WoodenSpearView.FireEffect_Transform.forward, 2000, Demage, Hit);
    }
}
