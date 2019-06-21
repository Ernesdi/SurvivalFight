using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// 合成面板M层
/// </summary>
public class CraftingPanelModel : MonoBehaviour {


    private Dictionary<int, CraftingMapItem> dictionary;


    private void Awake()
    {
        //一启动就解析 数据保存在dictionary中
        dictionary = GetMapJsonByName("CraftingMapJsonData");
    }

    /// <summary>
    /// 根据名字返回一个两层list数据列表(json两层解析)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<List<CraftingContentItem>> GetContentJsonByName(string fileName)
    {
        List<List<CraftingContentItem>> nameList = new List<List<CraftingContentItem>>();

        string jsonText = Resources.Load<TextAsset>("JsonData/" + fileName).text;

        JsonData jsonData = JsonMapper.ToObject(jsonText);
        for (int i = 0; i < jsonData.Count; i++)
        {
            JsonData jd = jsonData[i]["Type"];
            List<CraftingContentItem> tempList = new List<CraftingContentItem>();

            for (int j = 0; j < jd.Count; j++)
            {
                tempList.Add(JsonMapper.ToObject<CraftingContentItem>(JsonMapper.ToJson(jd[j])));

            }
            nameList.Add(tempList);
        }
        return nameList;

    }


    /// <summary>
    /// 根据名字返回一个合成图谱对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Dictionary<int, CraftingMapItem> GetMapJsonByName(string fileName) {
        Dictionary<int, CraftingMapItem> dictionary = new Dictionary<int, CraftingMapItem>();

        string jsonText = Resources.Load<TextAsset>("JsonData/" + fileName).text;
        JsonData jsonData = JsonMapper.ToObject(jsonText);
        for (int i = 0; i < jsonData.Count; i++)
        {
            int mapId = int.Parse(jsonData[i]["MapId"].ToString());
            string tempText = jsonData[i]["MapContents"].ToString();
            string[] mapContents = tempText.Split(',');
            int mapMaterialCount = int.Parse(jsonData[i]["MapMaterialCount"].ToString());

            string mapName = jsonData[i]["MapName"].ToString();
            CraftingMapItem cmi = new CraftingMapItem(mapId,mapContents,mapMaterialCount,mapName);
            dictionary.Add(mapId, cmi);
        }
        return dictionary;
    }

    /// <summary>
    /// 通过mapId返回一个图谱对象
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public CraftingMapItem GetCraftingMapItemByMapId(int mapId) {
        CraftingMapItem cmi = null;
        dictionary.TryGetValue(mapId, out cmi);
        return cmi;
    }
}
