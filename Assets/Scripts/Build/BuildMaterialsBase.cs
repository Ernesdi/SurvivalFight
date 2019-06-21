using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildMaterialsBase : MonoBehaviour {

    private Transform m_Transform;
    private bool isAttach = false;  //是否吸附
    private Material oldMaterial;   //原来的材质球
    private Material newMaterial;   //新的材质球
    private int rotAngle;

    public Transform M_Transform
    {
        get { return m_Transform; }
        set { m_Transform = value; }
    }

    public bool IsAttach
    {
        get { return isAttach; }
        set { isAttach = value; }
    }

    public int RotAngle
    {
        get { return rotAngle; }
        set{rotAngle = value; }
    }

    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
        newMaterial = Resources.Load<Material>("Build/Building Preview");
        gameObject.GetComponent<MeshRenderer>().material = newMaterial;
    }

    void Update()
    {
        if (BuildPanelController.Instance.IsCanPut)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 0, 100);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 100);
        }

        //旋转
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotAngle++;
            m_Transform.rotation = Quaternion.Euler(new Vector3(0, 90 * rotAngle, 0));
        }
    }

    /// <summary>
    /// 变回普通的颜色
    /// </summary>
    public void Normal()
    {
        gameObject.GetComponent<MeshRenderer>().material = oldMaterial;
        //gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
    }


    protected abstract void OnCollisionEnter(Collision collision);
    protected abstract void OnCollisionStay(Collision collision);
    protected abstract void OnCollisionExit(Collision collision);
    protected abstract void OnTriggerEnter(Collider other);
    protected abstract void OnTriggerExit(Collider other);
}
