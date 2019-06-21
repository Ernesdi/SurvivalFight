using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public delegate void PlayerDeathDel();

/// <summary>
/// 玩家控制
/// </summary>
public class PlayerController : MonoBehaviour {

    private Transform m_Transform;
    private FirstPersonController FPC;
    private int vit=100;
    private int hp=1000;
    private float vitB;             //存储一开始赋值的VIT速度值
    private float hpB;              //存储一开始赋值的HP生命值
    private int index;   //索引（延时一下然后削减体力） 每30帧削减一下体力
    private float runSpeed;
    private float walkSpeed;
    private float alpha;            //血屏的透明通道值
    private Image vit_Image;        //速度UI
    private Image hp_Image;         //HP UI
    private Image bs_Panel;         //血屏UI

    private AudioSource PlayerBreathingHeavy;  //玩家的呼吸声
    private bool isPlayerBreathingHeavy;       //是否正在播放玩家的呼吸声
    private bool isDeath;
    public event PlayerDeathDel playerDeathDel;

    public bool IsDeath
    {
        get { return isDeath; }
        set { isDeath = value; }
    }

    public int VIT
    {
        get { return vit; }
        set { vit = value;
            if (vit <= 0)
                vit = 0;
            FPC.M_RunSpeed = runSpeed * (vit * 1/ vitB);
            FPC.M_WalkSpeed = walkSpeed * (vit * 1 / vitB);
            vit_Image.fillAmount = vit * 1 / vitB;
            //如果小于一半的体力值
            if (vit <= vitB / 2)
            {
                if (!isPlayerBreathingHeavy)
                {
                    PlayerBreathingHeavy.playOnAwake = true;
                    PlayerBreathingHeavy.Play();
                    isPlayerBreathingHeavy = true;
                    Debug.Log("开始呼吸");
                }
                //根据体力值的多少呼吸声就有多大~当然这是介于有呼吸声的时候
                PlayerBreathingHeavy.volume = (vitB / 2 - vit) / (vitB / 2);
                //根据体力值的多少呼吸声就有多快~当然这是介于有呼吸声的时候
                PlayerBreathingHeavy.pitch = 1+((vitB / 2 - vit) / (vitB / 2)/3);

            }
            else
            {
                if (isPlayerBreathingHeavy)
                {
                    PlayerBreathingHeavy.Stop();
                    isPlayerBreathingHeavy = false;
                    Debug.Log("停止呼吸");
                }
            }

        }
    }
    public int HP
    {
        get { return hp; }
        set { hp = value;
           
            if (hp >= hpB / 2)
            {
                hp_Image.color = new Color32((byte)((hpB - hp)*(255/(hpB / 2))),255, 0, 255);
            }
            else
            {
                hp_Image.color = new Color32(255, (byte)(255-(hpB - hp) * (255 / (hpB / 2))), 0, 255);
            }
            hp_Image.fillAmount = hp * 1 / hpB;
            //不是Color 而是 Color32~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //bs_Panel.color = new Color(255, 255, 255, 255 * (1 - hp * 0.001f));
            //float temp = 255 * (1 - hp * 0.001f);
            //byte tempB = (byte)temp;
            bs_Panel.color = new Color32(255, 255, 255, (byte)(255 * (1 - hp * 1 / hpB)));
            if (hp <= 0)
                bs_Panel.color = new Color32(255, 255, 255, 255);
            //Debug.Log((byte)(255 * (1 - hp * 1 / hpB)));
            if (hp <= 0 && isDeath == false)
                Death();
        }
    }

    void Start()
    {
        vitB = vit;
        hpB = hp;
        m_Transform = gameObject.GetComponent<Transform>();
        FPC = gameObject.GetComponent<FirstPersonController>();
        StartCoroutine("ReStoreVit",5);                         //恢复百分比体力值
        StartCoroutine("ReStoreHp",30);                         //每秒固定回血30HP
        runSpeed = FPC.M_RunSpeed;
        walkSpeed = FPC.M_WalkSpeed;
        vit_Image = GameObject.Find("Canvas/MainPanel/VIT/Bar/Load").GetComponent<Image>();
        hp_Image = GameObject.Find("Canvas/MainPanel/HP/Bar/Load").GetComponent<Image>();
        bs_Panel = GameObject.Find("Canvas/MainPanel/BlooadScreenPanel").GetComponent<Image>();
        PlayerBreathingHeavy = AudioManager.Instance.PlayAudioClipByComponent(gameObject, ClipName.PlayerBreathingHeavy, false, true);
    }

    void Update()
    {
        //Debug.Log(FPC.M_PlayerState);
        ReduceVit();
        //Debug.Log("玩家HP："+hp);
        //Debug.Log("玩家体力：" + vit);
    }


    /// <summary>
    /// 不动的时候恢复体力值
    /// </summary>
    private IEnumerator ReStoreVit(float value)
    {
        Vector3 temp;
        while (true)
        {
            temp = m_Transform.position;
            //每一秒恢复value%的体力值
            yield return new WaitForSeconds(1);
            if (temp == m_Transform.position && VIT <= vitB*(1- 1 / vitB))
            {
                //Debug.Log("触发恢复value%的体力值"+ vitB * (5 / vitB));
                //恢复体力值
                VIT += (int)(vitB*(value / vitB));
                if (VIT>=vitB)
                    VIT = (int)vitB;
            }
        }
    }

    /// <summary>
    /// 没有受到伤害的时候恢复生命值
    /// </summary>
    private IEnumerator ReStoreHp(float value)
    {
        int temp;
        while (true)
        {
            temp = HP;
            //每一秒恢复3%的生命值
            yield return new WaitForSeconds(1);
            //没有扣血并且血量低于初始生命值
            if (temp == HP && HP <= hpB)
            {
                //恢复生命值
                HP += (int)value;
                if (HP >= hpB)
                    HP = (int)hpB;
                //Debug.Log("触发恢复3%的生命值"+ (int)(3 * (1 / hpB)));
            }
        }
    }

    /// <summary>
    /// 减少体力值
    /// </summary>
    public void ReduceVit()
    {
        if (FPC.M_PlayerState == PlayerState.WALK)
        {
            index++;
            if (index>=10)
            {
                VIT -= 1;
                index = 0;
            }
           
        }
        if (FPC.M_PlayerState == PlayerState.RUN)
        {
            index++;
            if (index >= 30)
            {
                VIT -= 2;
                index = 0;
            }
        }
    }


    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void Death()
    {
        StopAllCoroutines();
        GameObject.Find("GameController").GetComponent<GameController>().enabled = false;
        m_Transform.GetComponent<FirstPersonController>().enabled = false;
        AudioManager.Instance.PlayAudioClipByName(ClipName.PlayerDeath,m_Transform.position);
        isDeath = true;
        playerDeathDel();
        StartCoroutine("DealyReSetScene");
    }

    private IEnumerator DealyReSetScene ()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("ReSetScene");
    }

    //private IEnumerator LeftOrRight()
    //{
    //    yield return null;
    //    int index = 0;
    //    bool temp = true;
    //    while (true)
    //    {
    //        if (index==0 && temp == true)
    //        {
    //            index++;
    //            temp = !temp;
    //        }
    //        else if (index >= 100)
    //        {
    //            index--;
    //        }
    //        else if (index<=-100)
    //        {
    //            index++;
    //        }
    //        PlayerBreathingHeavy.panStereo = index / 100;
    //    }
        
    //}


    //序列化字段 让字段能够在属性面板显示 但是又是private的[SerializeField]
    //private Transform m_Transform;
    //private GameObject m_BuildingPlan;
    //private GameObject m_WoodenSpear;

    //当前手中的物品
    //private GameObject currentHandState;
    //要切换到的目标物品
    //private GameObject targetHandState;

    //void Start()
    //{
    //    m_Transform = gameObject.GetComponent<Transform>();
    //    m_BuildingPlan = m_Transform.Find("ModelCamera/Building Plan").gameObject;
    //    m_WoodenSpear = m_Transform.Find("ModelCamera/Wooden Spear").gameObject;

    //    设置其他手臂隐藏 留下地图手臂
    //    m_WoodenSpear.SetActive(false);

    //    默认手臂是地图
    //    currentHandState = m_BuildingPlan;
    //    targetHandState = null;
    //}


    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        targetHandState = m_BuildingPlan;
    //        SwitchHandState();
    //    }
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        targetHandState = m_WoodenSpear;
    //        SwitchHandState();
    //    }
    //}

    /// <summary>
    /// 切换手中持有的物品
    /// </summary>
    //private void SwitchHandState()
    //{
    //    currentHandState.GetComponent<Animator>().SetTrigger("holster");
    //    StartCoroutine("DelayTime");
    //}


    /// <summary>
    /// 延时
    /// </summary>
    //IEnumerator DelayTime()
    //{
    //    等待一秒让动画播放完毕
    //    yield return new WaitForSeconds(1);
    //    当前手臂隐藏
    //    currentHandState.SetActive(false);
    //    目标手臂显示
    //    targetHandState.SetActive(true);
    //    标志位更新
    //    currentHandState = targetHandState;
    //}

}
