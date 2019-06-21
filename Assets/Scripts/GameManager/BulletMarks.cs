using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹弹痕（贴图融合）
/// </summary>
//[RequireComponent(typeof(ObjectPool))]  //当弹痕脚本挂载到物体上自动添加一个对象池脚本
public class BulletMarks : MonoBehaviour {
    //主贴图 用于反融合弹痕
    private Texture2D m_Main_Texture;
    //主贴图备份1 用于生成弹痕
    private Texture2D m_Main_TextureBackUp_1;
    //主贴图备份2 用于修复预制体Bug  
    private Texture2D m_Main_TextureBackUp_2;
    //弹痕贴图
    private Texture2D m_BulletMark;

    //不同的弹痕材质  序列化后外部可以进行赋值 但又是不公开 
    [SerializeField]
    private MaterialType materialType;

    private Transform tempParent;   //生成的临时数据的父物体

    //子弹命中特效
    private GameObject play_Effect;

    //弹痕队列
    private Queue<Vector2> bulletMarkQueue;

    //对象池对象
    private ObjectPool pool;

    [SerializeField] private int hp;   //临时数据 HP

    public int Hp {
        get { return hp; }
        set { hp = value;
            if (hp<=0)
            {
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        switch (materialType)
        {
            case MaterialType.Metal:
                ResoucesLoad("Textures/Gun/BulletMarks/Bullet Decal_Metal", "Effects/Gun/Bullet Impact FX_Metal", "TempObject/MetalEffect");
                break;
            case MaterialType.Stone:
                ResoucesLoad("Textures/Gun/BulletMarks/Bullet Decal_Stone", "Effects/Gun/Bullet Impact FX_Stone", "TempObject/StoneEffect");
                break;
            case MaterialType.Wood:
                ResoucesLoad("Textures/Gun/BulletMarks/Bullet Decal_Wood", "Effects/Gun/Bullet Impact FX_Wood", "TempObject/WoodEffect");
                break;
        }
        if (gameObject.name=="Tree")
        {
            m_Main_Texture = (Texture2D)gameObject.GetComponent<MeshRenderer>().materials[2].mainTexture;
        }
        else
        {
            m_Main_Texture = (Texture2D)gameObject.GetComponent<MeshRenderer>().material.mainTexture;
        }
      
        //主贴图备份1
        m_Main_TextureBackUp_1 = GameObject.Instantiate<Texture2D>(m_Main_Texture);
        //主贴图备份2
        m_Main_TextureBackUp_2 = GameObject.Instantiate<Texture2D>(m_Main_Texture);

        //设置当前的贴图为主贴图备份1
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_Main_TextureBackUp_1;

        bulletMarkQueue = new Queue<Vector2>();
        //如果没有查到对象池组件 就加一个
        if (gameObject.GetComponent<ObjectPool>()==null)
            pool = gameObject.AddComponent<ObjectPool>();
        else
            pool = gameObject.GetComponent<ObjectPool>();

    }

    /// <summary>
    /// 资源加载类(重构swtich)
    /// </summary>
    private void ResoucesLoad(string bulletMark,string effect,string parent) {
        m_BulletMark = Resources.Load<Texture2D>(bulletMark);
        play_Effect = Resources.Load<GameObject>(effect);
        tempParent = GameObject.Find(parent).GetComponent<Transform>();
    }

    /// <summary>
    /// 生成弹痕
    /// </summary>
    public void CreateBulletMark(RaycastHit hit) {
        Debug.Log("CreateBulletMark");

        //获得hit的贴图UV坐标
        Vector2 uv = hit.textureCoord;
        Debug.Log(uv);

        bulletMarkQueue.Enqueue(uv);

        //for循环遍历弹痕贴图
        for (int i = 0; i < m_BulletMark.width; i++)
        {
            for (int j = 0; j < m_BulletMark.height; j++)
            {
                //uv.x*主贴图宽度 减去 弹痕贴图宽度/2 +i
                float x = uv.x * m_Main_TextureBackUp_1.width - m_BulletMark.width / 2 + i;

                //uv.x*主贴图高度 减去 弹痕贴图高度/2 +j
                float y = uv.y * m_Main_TextureBackUp_1.height - m_BulletMark.height / 2 + j;

                //获得弹痕贴图某个位置的像素
                Color color =  m_BulletMark.GetPixel(i,j);

                //如果透明度很低 就不融合了
                if (color.a>=0.2f)
                {
                    //主贴图融合弹痕贴图
                    m_Main_TextureBackUp_1.SetPixel((int)x, (int)y, color);
                }
               
            }
        }
        //应用贴图
        m_Main_TextureBackUp_1.Apply();
        //播放子弹击中特效
        PlayEffect(hit);
        //定时调用弹痕消失方法
        Invoke("RemoveBulletMark",5);
    }


    /// <summary>
    /// 移除弹痕
    /// </summary>
    private void RemoveBulletMark() {
        //获得碰撞的UV点
        Vector2 uv = bulletMarkQueue.Dequeue();
        //遍历 弹痕贴图的像素
        for (int i = 0; i < m_BulletMark.width; i++)
        {
            for (int j = 0; j < m_BulletMark.height; j++)
            {
                //如果说是在中心点生成弹痕 0.5* 主贴图的长度 减去 弹痕贴图的长度的的一半 再加上i
                float x = uv.x * m_Main_TextureBackUp_1.width - m_BulletMark.width / 2 + i;

                float y = uv.y * m_Main_TextureBackUp_1.height - m_BulletMark.height / 2 + j;

                //获得贴图备份的每个像素
                Color color = m_Main_TextureBackUp_2.GetPixel((int)x, (int)y);
                //设置主贴图
                m_Main_TextureBackUp_1.SetPixel((int)x, (int)y, color);
            }
        }
        //应用
        m_Main_TextureBackUp_1.Apply();

    }


    /// <summary>
    /// 播放特效
    /// </summary>
    private void PlayEffect(RaycastHit hit) {
        Debug.Log("PlayEffect");
        PlayAudio(hit);
        GameObject effect = null;
        //如果对象池中有对象
        if (pool.Data())
        {//出列
            effect =  pool.GetObject();
            //重置特效的生成点 旋转角度
            effect.GetComponent<Transform>().position = hit.point;
            effect.GetComponent<Transform>().rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            //生成点是碰撞点  特效根据碰撞点的法线进行旋转  法线(就是z轴？) 因为勾选了粒子特效组件的自动播放所以只要实例化就行了
            effect = Instantiate<GameObject>(play_Effect, hit.point, Quaternion.LookRotation(hit.normal), tempParent);
            effect.name = "Effect" + materialType;
        }
         
        StartCoroutine(Daley(effect, 1));
    }

    /// <summary>
    /// 播放子弹命中材质的声音
    /// </summary>
    public void PlayAudio(RaycastHit hit)
    {
        switch (materialType)
        {
            case MaterialType.Metal:
                AudioManager.Instance.PlayAudioClipByName(ClipName.BulletImpactMetal, hit.point);
                break;
            case MaterialType.Stone:
                AudioManager.Instance.PlayAudioClipByName(ClipName.BulletImpactStone, hit.point);
                break;
            case MaterialType.Wood:
                AudioManager.Instance.PlayAudioClipByName(ClipName.BulletImpactWood, hit.point);
                break;
        }
    }

    //等待特效播放完毕就入列
    IEnumerator Daley(GameObject go,float time) {
        yield return new WaitForSeconds(time);
        pool.AddObject(go);
    }

}
