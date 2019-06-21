using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 散弹枪子弹类
/// </summary>
public class ShotgunBullet : BulletBase {
    private Ray ray;
    private RaycastHit hit;
   
    public override void Init()
    {
        Invoke("KillSelf", 3);
    }

    /// <summary>
    /// 散弹枪子弹射击
    /// </summary>
    public override void Shoot(Vector3 pos, int force, int demage ,RaycastHit hit)
    {
        M_Rigidbody.AddForce(pos * force);
        //每个子弹的伤害值
        this.Demage = demage;
        //从当前子弹的位置往前发射一条射线 用于获得弹痕生成UV流
        ray = new Ray(M_Transform.position, pos);
        //接受hit碰撞信息  1000米最远射线距离，所能接受到的层是 11层 Env层
        //LayerMask.LayerToName(11);
        
    }

    /// <summary>
    /// 当子弹碰撞到物体，停止运动
    /// </summary>
    public override void CollisionEnter(Collision collision)
    {
            Debug.Log("Stop");
            //子弹停止运动
            M_Rigidbody.Sleep();
            //生成弹痕
            if (collision.collider.GetComponent<BulletMarks>() != null)
            {
                if (Physics.Raycast(ray, out hit, 1000, 1 << 11)) { }
                //调用里面的方法
                collision.collider.GetComponent<BulletMarks>().CreateBulletMark(hit);
                collision.collider.GetComponent<BulletMarks>().Hp -= Demage;
            }
            if (collision.collider.GetComponentInParent<AI>() != null)
            {
                if (Physics.Raycast(ray, out hit, 1000, 1 << 12)) { }
                //调用里面的方法
                //collision.collider.GetComponentInParent<AI>().HP -= Demage;
                collision.collider.GetComponentInParent<AI>().PlayFleshEffect(hit);
                if (collision.collider.gameObject.name == "Head")
                {
                    collision.collider.GetComponentInParent<AI>().HeadHit(Demage * 2);
                }
                else
                {
                    collision.collider.GetComponentInParent<AI>().NormalHit(Demage);
                }
            }
        Destroy(gameObject);
    }
}
