using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRogdoll : MonoBehaviour {

    private Transform m_Transform;
    private Animator m_Animator;
    private BoxCollider body_BoxCollider;
    private BoxCollider foot_BoxCollider;


    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Animator = gameObject.GetComponent<Animator>();
        body_BoxCollider = m_Transform.Find("Armature/Hips/Middle_Spine").GetComponent<BoxCollider>();
        foot_BoxCollider = m_Transform.Find("Armature").GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 启动布娃娃系统
    /// </summary>
    public void StartRagdoll()
    {
        //禁用Animator组件，把控制权交给布娃娃系统。
        m_Animator.enabled = false;
        body_BoxCollider.enabled = false;
        foot_BoxCollider.enabled = false;
    }
}
