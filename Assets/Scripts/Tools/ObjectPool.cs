using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池工具
/// </summary>
public class ObjectPool : MonoBehaviour {
    //声明一个队列用作对象池存储
    private Queue<GameObject> pool=null;
	
	void Awake () {
        pool = new Queue<GameObject>();
    }


    /// <summary>
    /// 添加对象到对象池中去
    /// </summary>
    public void AddObject(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }

    /// <summary>
    /// 在对象池里读取对象
    /// </summary>
    public GameObject GetObject()
    {
        GameObject go=null;
        if (pool.Count>0)
        {
            go = pool.Dequeue();
            go.SetActive(true);
        }
        return go;
    }

    /// <summary>
    /// 对象池中是否有数据？
    /// </summary>
    public bool Data() {
        if (pool.Count>0)
            return true;
        else
            return false;
    }
}
