using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石头管理脚本
/// </summary>
public class StoneManager : EnvManagerBase {

    private GameObject prefab_Stone1;
    private GameObject prefab_Stone2;

    protected override void FindInit()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_CreatePoint = m_Transform.Find("StonePoint").GetComponentsInChildren<Transform>();
        create_Transform = m_Transform.Find("Stones").GetComponent<Transform>();

        prefab_Stone1 = Resources.Load<GameObject>("Env/Rock_Normal");
        prefab_Stone2 = Resources.Load<GameObject>("Env/Rock_Metal");
    }

    protected override void CreateEnv()
    {
        for (int i = 1; i < m_CreatePoint.Length; i++)
        {
            m_CreatePoint[i].GetComponent<MeshRenderer>().enabled = false;
            GameObject prefab;
            //石头随机
            int stone = Random.Range(0, 2);
            if (stone == 0)
                prefab = prefab_Stone1;
            else
                prefab = prefab_Stone2;
            //缩放随机
            float scale = Random.Range(0.5f, 2.5f);
            //旋转随机
            Vector3 rot = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Transform temp = Instantiate<GameObject>(prefab, m_CreatePoint[i].localPosition, Quaternion.identity, create_Transform).GetComponent<Transform>();
            temp.localScale = temp.localScale * scale;
            temp.localRotation = Quaternion.Euler(rot);
        }
    }

   

}
