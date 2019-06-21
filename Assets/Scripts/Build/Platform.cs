using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地板
/// </summary>
public class Platform : BuildMaterialsBase {

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain")
        {
            BuildPanelController.Instance.IsCanPut = false;
        }
    }
    protected override void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain")
        {
            BuildPanelController.Instance.IsCanPut = false;
        }
    }
    protected override void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain")
        {
            BuildPanelController.Instance.IsCanPut = true;
        }
    }

    /// <summary>
    /// 当触发开始 就让他有吸附属性
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Platform")
        {

            if (BuildPanelController.Instance.IsCanPut==false)
                return;
            Debug.Log("触发开始");
            IsAttach = true;
            Vector3 offsetPos = Vector3.zero;                                            //偏移位置        
            Vector3 fixModelPos = other.gameObject.GetComponent<Transform>().position;   //固定模型的位置
            Vector3 moveModelPos = M_Transform.position;                                 //移动模型的位置
            float x = moveModelPos.x - fixModelPos.x;
            float z = moveModelPos.z - fixModelPos.z;

            if (x > 0 && Mathf.Abs(z) <0.4f)  //右边
            {
                offsetPos = new Vector3(3.3f, 0, 0);
            }
            else if (x < 0 && Mathf.Abs(z) < 0.4f) //左边
            {
                offsetPos = new Vector3(-3.3f, 0, 0);
            }

            if (z > 0 && Mathf.Abs(x) < 0.4f)  //前面
            {
                offsetPos = new Vector3(0, 0, 3.3f);
            }
            else if (z < 0 && Mathf.Abs(x) < 0.4f) //后面
            {
                offsetPos = new Vector3(0, 0, -3.3f);
            }
            M_Transform.position = fixModelPos + offsetPos;
            //Debug.Log(IsAttach);
        }
    }

    /// <summary>
    /// 当触发结束
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Platform")
        {

        }
    }

}
