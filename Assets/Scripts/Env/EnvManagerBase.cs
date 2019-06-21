using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 环境埋点父类
/// </summary>
public abstract class EnvManagerBase : MonoBehaviour {

    protected Transform m_Transform;
    //查找生成点
    protected Transform[] m_CreatePoint;
    //需要生成在那个父物体
    protected Transform create_Transform;

    private void Start()
    {
        FindInit();
        CreateEnv();
    }

    protected abstract void FindInit();

    protected abstract void CreateEnv();

}
