using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    IDLE,
    WALK,
    ENTERRUN,
    EXITRUN,
    ENTERATTACK,
    EXITATTACK,
    DEATH,
    NULL
}

/// <summary>
/// 单个AI自身的控制器
/// </summary>
public class AI : MonoBehaviour {

    private Transform m_Transform;
    private Vector3 dir;
    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;
    [SerializeField]
    private AIState aiState = AIState.IDLE;
    private GameObject fx_Flesh;
    private int isDeath;   //已经死亡标志位
    private Transform player_Transform;
    private PlayerController m_PlayerController;

    private AIManagerType m_AIManagerType = AIManagerType.NULL;
    
    public AIState AiState
    {
        get { return aiState; }
        set { aiState = value; }
    }
    public AIManagerType M_AIManagerType
    {
        get { return m_AIManagerType; }
        set { m_AIManagerType = value; }
    }
    [SerializeField]
    private int hp;                 //AI的生命值
    [SerializeField]
    private int attack;             //AI的攻击力

    public int HP
    {
        get { return hp; }
        set { hp = value;
            if (hp <= 0)
            {
                SwitchState(AIState.DEATH);
            } 
        }
    }
    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    //声明初始化要在一起。不然程序运行不了，因为AIManager要给这个List集合进行赋值 
    private List<Vector3> pos_List = new List<Vector3>();

    public List<Vector3> Pos_List
    {
        get { return pos_List; }
        set { pos_List = value; }
    }
    public Vector3 Dir
    {
        get { return dir; }
        set { dir = value; }
    }

    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        m_Animator = gameObject.GetComponent<Animator>();
        player_Transform = GameObject.Find("FPSController").GetComponent<Transform>();
        m_PlayerController = player_Transform.GetComponent<PlayerController>();
        fx_Flesh = Resources.Load<GameObject>("Effects/Gun/Bullet Impact FX_Flesh");
        m_NavMeshAgent.SetDestination(dir);
    }

    void Update () {
        CalcDistanceChangeTarget();
        AIFollowPlayerByDestination();
        AIAttackPlayerByDestination();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchState(AIState.DEATH);
        }

    }

    /// <summary>
    /// 根据距离目标点的位置改变巡逻方向
    /// </summary>
    private void CalcDistanceChangeTarget()
    {
        if (aiState == AIState.WALK || aiState == AIState.IDLE)
        {
            if (Vector3.Distance(m_Transform.position, dir) <= 1)
            {
                int index = Random.Range(0, pos_List.Count);
                dir = pos_List[index];
                m_NavMeshAgent.SetDestination(dir);
                SwitchState(AIState.IDLE);
            }
            else
            {
                SwitchState(AIState.WALK);
            }
        }
    }

    /// <summary>
    /// AI跟随玩家
    /// </summary>
    private void AIFollowPlayerByDestination()
    {
        if (Vector3.Distance(m_Transform.position, player_Transform.position) <= 20 && m_PlayerController.IsDeath == false)
        {
            SwitchState(AIState.ENTERRUN);
        }
        else
        {
            SwitchState(AIState.EXITRUN);
        }
    }

    /// <summary>
    /// AI攻击玩家
    /// </summary>
    private void AIAttackPlayerByDestination()
    {
        if (aiState == AIState.ENTERRUN)
        {
            if (Vector3.Distance(m_Transform.position, player_Transform.position) <= 2 && m_PlayerController.IsDeath == false)
            {
                SwitchState(AIState.ENTERATTACK);
            }
            else
            {
                SwitchState(AIState.EXITATTACK);
            }
        }
    }




    /// <summary>
    /// 切换状态方法。
    /// </summary>
    private void SwitchState(AIState aIState)
    {
        switch (aIState)
        {
            case AIState.IDLE:
                IdleState();
                break;
            case AIState.WALK:
                WalkState();
                break;
            case AIState.ENTERRUN:
                EnterRun();
                break;
            case AIState.EXITRUN:
                ExitRun();
                break;
            case AIState.ENTERATTACK:
                EnterAttack();
                break;
            case AIState.EXITATTACK:
                ExitAttack();
                break;
            case AIState.DEATH:
                DeathState();
                break;
        }
    }

    /// <summary>
    /// 行走状态
    /// </summary>
    private void WalkState()
    {
        m_Animator.SetBool("Walk",true);
        aiState = AIState.WALK;
    }
    /// <summary>
    /// 站立状态
    /// </summary>
    private void IdleState()
    {
        m_Animator.SetBool("Walk", false);
        aiState = AIState.IDLE;
    }

    /// <summary>
    /// 进入奔跑状态
    /// </summary>
    private void EnterRun()
    {
        m_Animator.SetBool("Run", true);
        aiState = AIState.ENTERRUN;
        //安全校验
//       if (m_NavMeshAgent.enabled == false)
//          return;
        m_NavMeshAgent.enabled = true;
        m_NavMeshAgent.speed = 3.0f;
        m_NavMeshAgent.SetDestination(player_Transform.position);
    }

    /// <summary>
    /// 退出奔跑状态
    /// </summary>
    private void ExitRun()
    {
        m_Animator.SetBool("Run", false);
        SwitchState(AIState.WALK);
        m_NavMeshAgent.enabled = true;
        m_NavMeshAgent.speed = 0.8f;
        m_NavMeshAgent.SetDestination(dir);
    }

    /// <summary>
    /// 进入攻击状态
    /// </summary>
    private void EnterAttack()
    {
        m_Animator.SetBool("Attack", true);
        m_NavMeshAgent.enabled = false;
        aiState = AIState.ENTERATTACK;
    }


    /// <summary>
    /// 退出攻击状态
    /// </summary>
    private void ExitAttack()
    {
        m_Animator.SetBool("Attack", false);
        m_NavMeshAgent.enabled = true;
        SwitchState(AIState.ENTERRUN);
    }

    /// <summary>
    /// AI死亡状态
    /// </summary>
    private void DeathState()
    {
        if (isDeath == 1)
            return;
        if (m_AIManagerType == AIManagerType.BOAR)
        {
            m_Animator.SetTrigger("Death");
            AudioManager.Instance.PlayAudioClipByName(ClipName.BoarDeath, m_Transform.position);
        }
        else if (m_AIManagerType == AIManagerType.CANNIBAL)
        {
            m_Transform.GetComponent<AIRogdoll>().StartRagdoll();
            AudioManager.Instance.PlayAudioClipByName(ClipName.ZombieDeath, m_Transform.position);
        }
        //弃用也是没办法的呢  换一个吧
        //m_NavMeshAgent.Stop();
        if (m_NavMeshAgent.isActiveAndEnabled == true)
        {
            m_NavMeshAgent.isStopped = true;
        }
        StartCoroutine("Death");
        isDeath = 1;
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        SendMessageUpwards("AIDeath", gameObject);
    }

    /// <summary>
    /// 播放血液特效
    /// </summary>
    public void PlayFleshEffect(RaycastHit hit)
    {
        GameObject go = Instantiate<GameObject>(fx_Flesh, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(go,3);
    }

    /// <summary>
    /// 头部受到伤害
    /// </summary>
    public void HeadHit(int value)
    {
        if (isDeath == 1)
            return;
        //因为触发动画时间较短所以不用FSM 模式  finite state machine机器
        m_Animator.SetTrigger("HitHard");
        HP -= value;
        Debug.Log("命中头部，造成："+value);
        PlayHitSource();
    }

    /// <summary>
    /// 身体受到伤害
    /// </summary>
    public void NormalHit(int value)
    {
        if (isDeath == 1)
            return;
        m_Animator.SetTrigger("HitNormal");
        HP -= value;
        Debug.Log("命中身体，造成：" + value);
        PlayHitSource();
    }

    /// <summary>
    /// 播放受伤声音
    /// </summary>
    private void PlayHitSource()
    {
        if (m_AIManagerType == AIManagerType.BOAR)
        {
            AudioManager.Instance.PlayAudioClipByComponent(gameObject,ClipName.BoarInjured, true,false);
        }
        else if (m_AIManagerType == AIManagerType.CANNIBAL)
        {
            AudioManager.Instance.PlayAudioClipByComponent(gameObject, ClipName.ZombieAttack, true, false);
        }
    }


    /// <summary>
    /// 用于动画事件攻击玩家调用PlayerController中的生命值削减方法
    /// </summary>
    private void AttackPlayer(float AttackRange)
    {
        if (m_AIManagerType == AIManagerType.BOAR)
        {
            AudioManager.Instance.PlayAudioClipByName(ClipName.BoarAttack, m_Transform.position);
        }
        else if (m_AIManagerType == AIManagerType.CANNIBAL)
        {
            AudioManager.Instance.PlayAudioClipByName(ClipName.ZombieAttack, m_Transform.position);
        }
        //前方的距离小于攻击范围时
        if (m_Transform.position.z - player_Transform.position.z <= AttackRange)
        {
            m_PlayerController.HP-=Attack;
            //播放玩家受伤害的声音
            AudioManager.Instance.PlayAudioClipByName(ClipName.PlayerHurt, player_Transform.position);
        }
       
    }
}
