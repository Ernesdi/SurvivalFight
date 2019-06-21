using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 合成面板图谱实体类
/// </summary>
public class CraftingMapItem  {

    private int mapId;
    private string[] mapContents;
    private int mapMaterialCount;
    private string mapName;

 
    public int MapId { get { return mapId; }set { mapId = value; } }
    public string[] MapContents { get { return mapContents; }set { mapContents = value; } }
    public int MapMaterialCount { get { return mapMaterialCount; }set { mapMaterialCount = value; } }
    public string MapName { get { return mapName; }set { mapName = value; } }

    public CraftingMapItem(int mapId,string[] maoContents,int mapMaterialCount,string mapName)
    {
        this.MapId = mapId;
        this.mapContents = maoContents;
        this.mapMaterialCount = mapMaterialCount;
        this.mapName = mapName;
    }

    public override string ToString()
    {
        return string.Format("图谱的ID是{0}，图谱的位置是{1}，图谱合成需要的材料数是{2}，图谱的名字是{3}",mapId,mapContents.Length,mapMaterialCount,mapName);
    }

}
