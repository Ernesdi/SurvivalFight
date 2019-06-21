using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManagers : MonoBehaviour {

    private Transform m_Transform;
    private Transform[] Points;
	
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        Points = m_Transform.GetComponentsInChildren<Transform>();

        CreateAllAI();
    }

    /// <summary>
    /// 创建所有AI
    /// </summary>
    private void CreateAllAI()
    {
        for (int i = 1; i < Points.Length; i++)
        {
            if (i % 2 == 0)
            {
                Points[i].gameObject.AddComponent<AIManager>().AIManagerType = AIManagerType.BOAR;
            }
            else
            {
                Points[i].gameObject.AddComponent<AIManager>().AIManagerType = AIManagerType.CANNIBAL;
            }
        }
    }

}
