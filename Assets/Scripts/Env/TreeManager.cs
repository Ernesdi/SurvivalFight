using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石头管理脚本
/// </summary>
public class TreeManager : EnvManagerBase {

    //种类
    private GameObject prefab_Tree2;

    protected override void FindInit()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_CreatePoint = m_Transform.Find("TreesPoint").GetComponentsInChildren<Transform>();
        create_Transform = m_Transform.Find("Trees").GetComponent<Transform>();

        prefab_Tree2 = Resources.Load<GameObject>("Env/Conifer");
     
    }

    protected override void CreateEnv()
    {
        for (int i = 1; i < m_CreatePoint.Length; i++)
        {
            //隐藏生成点的Mesh
            m_CreatePoint[i].GetComponent<MeshRenderer>().enabled = false;

            GameObject prefab = prefab_Tree2;

            //缩放随机
            float scale = Random.Range(0.5f, 1.0f);
            //旋转随机
            Vector3 rot = new Vector3(0, Random.Range(0, 360), 0);
            Transform temp = Instantiate<GameObject>(prefab, m_CreatePoint[i].localPosition, Quaternion.identity, create_Transform).GetComponent<Transform>();
            temp.localScale = temp.localScale * scale;
            temp.localRotation = Quaternion.Euler(rot);

            temp.gameObject.name = "Tree";
        }

    }
}
