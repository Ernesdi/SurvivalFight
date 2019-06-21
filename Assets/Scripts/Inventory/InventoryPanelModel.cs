using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// 背包模块M层  数据读取层
/// </summary>
public class InventoryPanelModel : MonoBehaviour {
    
    void Awake () {
		
	}

    /// <summary>
    /// 使用路径读取json返回一个List对象
    /// </summary>
    /// <param name="fileName">文件名</param>
    public List<InventoryItem> GetJsonByName(string fileName) {
        List<InventoryItem> itemList =  JsonTools.GetFirstJsonList<InventoryItem>(fileName);
        return itemList;
    }


}
