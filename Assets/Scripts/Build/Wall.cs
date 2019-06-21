using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BuildMaterialsBase {

    private int index = 1;
    List<Transform> tempTransform = new List<Transform>();
    Transform[] transformGroup;
    private bool isOnPlatform;                  //是否在边边上
    private bool isCollisionWithPlatform;
    public bool IsOnPlatform
    {
        get
        {
            return isOnPlatform;
        }

        set
        {
            isOnPlatform = value;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isCollisionWithPlatform = true;
            BuildPanelController.Instance.IsCanPut = true;
            transformGroup = collision.gameObject.GetComponent<Transform>().GetComponentsInChildren<Transform>();
            for (int i = 0; i < transformGroup.Length; i++)
            {
                Debug.Log(transformGroup[i].name);
            }
        }
        else if (collision.gameObject.tag == "Terrain")
        {
            isCollisionWithPlatform = false;
            BuildPanelController.Instance.IsCanPut = true;
        }
    }
    protected override void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isCollisionWithPlatform = true;
            BuildPanelController.Instance.IsCanPut = true;
            transformGroup = collision.gameObject.GetComponent<Transform>().GetComponentsInChildren<Transform>();
        }
        else if (collision.gameObject.tag == "Terrain")
        {
            isCollisionWithPlatform = false;
            BuildPanelController.Instance.IsCanPut = true;
        }
    }
    protected override void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isCollisionWithPlatform = false;
            transformGroup = null;
            IsAttach = false;
        }
        else if (collision.gameObject.tag == "Terrain")
        {
            isCollisionWithPlatform = false;
            BuildPanelController.Instance.IsCanPut = false;
        }
    }


    protected override void OnTriggerEnter(Collider other)
    {
      
    }

    protected override void OnTriggerExit(Collider other)
    {
       
    }

    /// <summary>
    /// 重写父类Updata 全部搬过来
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (transformGroup!=null)
            {
                
                //只有一个地基就不允许放置墙壁了
                if (transformGroup.Length == 1)
                {
                    BuildPanelController.Instance.IsCanPut = false;
                    return;
                }
                isOnPlatform = true;
                BuildPanelController.Instance.RayStop = true;
                index++;
                index = index % (transformGroup.Length - 1);
                if (index == 0)
                    index = transformGroup.Length - 1;
                M_Transform.position = transformGroup[index].position;
                M_Transform.rotation = transformGroup[index].rotation;
            }
        }
       
        if (BuildPanelController.Instance.IsCanPut)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 0, 100);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 100);
        }
        if (Input.GetKeyDown(KeyCode.E) && isCollisionWithPlatform == true)
        {
            isOnPlatform = false;
            BuildPanelController.Instance.RayStop = false;
        }
        //旋转
        if (Input.GetKeyDown(KeyCode.E) && isCollisionWithPlatform == false)
        {
            RotAngle++;
            M_Transform.rotation = Quaternion.Euler(new Vector3(0, 90 * RotAngle, 0));
        }

    }

    /// <summary>
    /// 因为已经存在东西所以销毁掉这个Transform
    /// </summary>
    public void DestroyOnTransform()
    {
        if (transformGroup!=null)
        {
            Destroy(transformGroup[index].gameObject);
        }
    }
}
