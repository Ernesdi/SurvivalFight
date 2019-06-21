using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 是管理某一类的枚举  
/// </summary>
public enum AIManagerType
{
    BOAR,
    CANNIBAL,
    NULL
}

/// <summary>
/// AI管理器
/// </summary>
public class AIManager : MonoBehaviour {

    private Transform m_Transform;
    //野猪
    private GameObject prefab_Boar;
    //Zombie
    private GameObject prefab_Cannibal;
    //AI集合
    private List<GameObject> AIList;
    //AI要移动到的目标点
    private Transform[] pos_Transform;

    //AI移动到目标点的队列
    //private Queue<Vector3> pos_Queue;
    private List<Vector3> pos_List;
    //默认类型
    private AIManagerType AIManagertype = AIManagerType.NULL;
    //重新生成的AI计数
    private int index;

    private PlayerController m_PlayerController;

    private AIRogdoll m_AIRogdoll = null;

    public AIManagerType AIManagerType
    {
        get { return AIManagertype; }
        set { AIManagertype = value; }
    }

    void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        prefab_Boar = Resources.Load<GameObject>("AI/Boar");
        prefab_Cannibal = Resources.Load<GameObject>("AI/Cannibal");
        m_PlayerController = GameObject.Find("FPSController").GetComponent<PlayerController>();
        m_PlayerController.playerDeathDel += PlayerDeath;

        AIList = new List<GameObject>();
        //pos_Queue = new Queue<Vector3>();
        pos_List = new List<Vector3>();
        //填写true能够查找到隐藏的物体
        pos_Transform = gameObject.GetComponentsInChildren<Transform>(true);
        for (int i = 1; i < pos_Transform.Length; i++)
        {
            //pos_Queue.Enqueue(pos_Transform[i].position);
            pos_List.Add(pos_Transform[i].position);
        }
        CreateAIByEnum();
    }

    private void M_PlayerController_playerDeathDel()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 通过枚举来创建AI
    /// </summary>
    private void CreateAIByEnum()
    {
        if (AIManagertype == AIManagerType.BOAR)
        {
            CreateAI(prefab_Boar);
        }
        else if (AIManagertype == AIManagerType.CANNIBAL)
        {
            CreateAI(prefab_Cannibal);
        }
    }

    /// <summary>
    /// 循环创建
    /// </summary>
    /// <param name="prefab"></param>
    private void CreateAI(GameObject prefab)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(prefab, m_Transform.position, Quaternion.identity, m_Transform);
            //go.GetComponent<AI>().Dir = pos_Queue.Dequeue();
            go.GetComponent<AI>().Dir = pos_List[i];
            go.GetComponent<AI>().Pos_List = pos_List;
            if (AIManagertype == AIManagerType.BOAR)
            {
                go.GetComponent<AI>().M_AIManagerType = AIManagerType.BOAR;
                go.GetComponent<AI>().HP = 200;
                go.GetComponent<AI>().Attack = 50;
            }
            else if(AIManagertype == AIManagerType.CANNIBAL)
            {
                go.GetComponent<AI>().M_AIManagerType = AIManagerType.CANNIBAL;
                go.GetComponent<AI>().HP = 100;
                go.GetComponent<AI>().Attack = 100;
            }
            AIList.Add(go);
        }
    }


    /// <summary>
    /// AI死亡
    /// </summary>
    private void AIDeath(GameObject AI)
    {
        AIList.Remove(AI);
        StartCoroutine("ReCreateOneAI", 3);
    }

    /// <summary>
    /// 重新创建一个AI
    /// </summary>
    /// <param name="time">重新生成的时间</param>
    /// <returns></returns>
    private IEnumerator ReCreateOneAI(int time)
    {
        GameObject go = null;
        yield return new WaitForSeconds(time);
        if (AIManagertype == AIManagerType.BOAR)
        {
            go = GameObject.Instantiate<GameObject>(prefab_Boar, m_Transform.position, Quaternion.identity, m_Transform);
            go.GetComponent<AI>().M_AIManagerType = AIManagerType.BOAR;
            go.GetComponent<AI>().HP = 200;
            go.GetComponent<AI>().Attack = 50;
        }
        else if (AIManagertype == AIManagerType.CANNIBAL)
        {
            go = GameObject.Instantiate<GameObject>(prefab_Cannibal, m_Transform.position, Quaternion.identity, m_Transform);
            go.GetComponent<AI>().M_AIManagerType = AIManagerType.CANNIBAL;
            go.GetComponent<AI>().HP = 100;
            go.GetComponent<AI>().Attack = 100;
        }
        index++;
        int temp = index % pos_List.Count;
        go.GetComponent<AI>().Dir = pos_List[temp];
        go.GetComponent<AI>().Pos_List = pos_List;
        AIList.Add(go);
    }


    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void PlayerDeath()
    {
        for (int i = 0; i < AIList.Count; i++)
        {
            AIList[i].GetComponent<Animator>().SetBool("PlayerDeath",true);
            AIList[i].GetComponent<AI>().AiState = AIState.IDLE;
        }
    }

}
