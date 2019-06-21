using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源工具类
/// </summary>
public sealed class ResourcesTools{

    /// <summary>
    /// 读取指定文件夹中的图标到内存中
    /// </summary>
    public static Dictionary<string, Sprite> ByNameLoadFile(string name, Dictionary<string, Sprite> dic)
    {
        Sprite[] tempSprite = Resources.LoadAll<Sprite>(name);
        //添加到字典集合中去
        for (int i = 0; i < tempSprite.Length; i++)
        {
            dic.Add(tempSprite[i].name, tempSprite[i]);
        }
        return dic;
    }


    /// <summary>
    /// 通过名字获取sprite图片
    /// </summary>
    public static Sprite ByNameGetSprite(string name,Dictionary<string, Sprite> dic)
    {
        Sprite temp = null;
        dic.TryGetValue(name, out temp);
        return temp;
    }

}
