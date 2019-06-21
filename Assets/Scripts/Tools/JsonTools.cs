using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// json数据加载工具类 sealed密封的
/// </summary>
public sealed class JsonTools : MonoBehaviour {

    private static bool isLoad;
    private static AssetBundle  ab = null;
    /// <summary>
    /// 使用路径读取json返回一个List对象
    /// </summary>
    /// <param name="fileName"></param>
    public static List<T> GetFirstJsonList<T>(string fileName)
    {
        List<T> tempList = new List<T>();
        string jsonStr = Resources.Load<TextAsset>("JsonData/" + fileName).text;
        
        //if (isLoad ==false)
        //{
        //    ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/jsondata.assetbundle");
        //    isLoad = true;
        //}
        //Debug.Log(fileName);
        //string jsonStr = ab.LoadAsset<TextAsset>(fileName).text;

        JsonData jsonData = JsonMapper.ToObject(jsonStr);
        for (int i = 0; i < jsonData.Count; i++)
        {
            T tempItem = JsonMapper.ToObject<T>(jsonData[i].ToJson());
            tempList.Add(tempItem);
        }
        return tempList;
    }

    /// <summary>
    /// 更新Json文件
    /// </summary>
    public static void UpdateJsonFile<T>(List<T> list, string name)
    {

        JsonData tempJsonData = JsonMapper.ToObject(JsonMapper.ToJson(list));
        string path = Application.dataPath + "/Resources/JsonData/" + name + ".txt";
        //string path = Application.streamingAssetsPath + "/"+name+".txt";
        FileOperation.SaveToJsonFileIO(tempJsonData, path);
    }


}
