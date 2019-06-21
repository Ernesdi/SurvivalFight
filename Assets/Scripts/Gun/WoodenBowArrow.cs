using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 弓箭的箭自身的管理器
/// </summary>
public class WoodenBowArrow : BulletBase
{

    private BoxCollider m_BoxCollider;
    private Transform pivot_Transform;
    private RaycastHit hit;

    public override void Init()
    {
        m_BoxCollider = gameObject.GetComponent<BoxCollider>();
        pivot_Transform = M_Transform.Find("Pivot").GetComponent<Transform>();
    }

    public override void Shoot(Vector3 pos, int force, int demage ,RaycastHit hit)
    {
        M_Rigidbody.AddForce(pos * force);
        //箭头子弹的伤害值
        this.Demage = demage;
        this.hit = hit;
    }

    public override void CollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag!="Arrow")
        {
            //箭头停止运动
            M_Rigidbody.Sleep();
            //如果碰到了障碍物
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Env"))
            {//计算伤害
                collision.collider.GetComponent<BulletMarks>().Hp -= Demage;
                collision.collider.GetComponent<BulletMarks>().PlayAudio(hit);
                //移除刚体
                Destroy(M_Rigidbody);
                //移除碰撞器
                Destroy(m_BoxCollider);
                //设置父物体跟随
                M_Transform.SetParent(collision.gameObject.transform);

                StartCoroutine("TileAnimation", pivot_Transform);
            }
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("AI"))
            {//计算伤害
                collision.collider.GetComponentInParent<AI>().HP -= Demage;
                collision.collider.GetComponentInParent<AI>().PlayFleshEffect(hit);
                //移除刚体
                Destroy(M_Rigidbody);
                //移除碰撞器
                Destroy(m_BoxCollider);
                //设置父物体跟随
                M_Transform.SetParent(collision.gameObject.transform);

                StartCoroutine("TileAnimation", pivot_Transform);
            }
        }
       
    }
}
