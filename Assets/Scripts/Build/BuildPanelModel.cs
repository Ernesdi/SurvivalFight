using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelModel : MonoBehaviour {

    private string[] names = new string[] { "", "【其他】", "【屋顶】", "【楼梯】", "【窗户】", "【门】", "【墙】", "【地板】", "【地基】" };
    private List<Sprite> itemSpriteList = new List<Sprite>();                   //图集集合
    private List<Sprite[]> itemIconList = new List<Sprite[]>();                 //单个种类所拥有的子类    里面没有Null元素
    private List<List<GameObject>> materialsModelList = new List<List<GameObject>>();   //材料模型集合    里面也没有Null元素
    private List<string[]> materialsName = new List<string[]>();                //材料名字集合
    public string[] Names
    {
        get { return names; }
    }
    public List<Sprite> ItemSpriteList
    {
        get { return itemSpriteList; }
    }
    public List<Sprite[]> ItemIconList
    {
        get { return itemIconList; }
    }
    public List<List<GameObject>> MaterialsModelList
    {
        get { return materialsModelList; }
    }
    public List<string[]> MaterialsName
    {
        get { return materialsName; }
    }

    void Awake () {
        LoadSprite();
        LoadItemIcon();
        LoadMaterialModel();
        AddNameToMaterialsName();
    }
    /// <summary>
    /// 加载图片
    /// </summary>
    private void LoadSprite()
    {
        //添加九大图标
        itemSpriteList.Add(null);
        itemSpriteList.Add(LoadSpriteByBuildFiled("Question Mark", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Roof_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Stairs_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Window_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Door_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Wall_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Floor_Category", "Icon"));
        itemSpriteList.Add(LoadSpriteByBuildFiled("Foundation_Category", "Icon"));
    }

    /// <summary>
    /// 在建造文件夹中读取Sprite资源
    /// </summary>
    /// <param name="filedName">二级文件夹</param>
    /// <param name="spriteName">资源名</param>
    /// <returns></returns>
    private Sprite LoadSpriteByBuildFiled(string spriteName, string filedName = "ItemIcon")
    {
        return Resources.Load<Sprite>("Build/" + filedName + "/" + spriteName);
    }


    /// <summary>
    /// 读取单个Item拥有的所有图标
    /// </summary>
    private void LoadItemIcon()
    {
        itemIconList.Add(null);
        itemIconList.Add(new Sprite[] {
            LoadSpriteByBuildFiled("Ceiling Light"),
            LoadSpriteByBuildFiled("Pillar_Wood"),
            LoadSpriteByBuildFiled("Wooden Ladder")
        });
        itemIconList.Add(new Sprite[] {
            null,
            LoadSpriteByBuildFiled("Roof_Metal"),
            null
        });
        itemIconList.Add(new Sprite[] {
            LoadSpriteByBuildFiled("Stairs_Wood"),
            LoadSpriteByBuildFiled("L Shaped Stairs_Wood"),
            null
        });
        itemIconList.Add(new Sprite[] {
            null,
            LoadSpriteByBuildFiled("Window_Wood"),
            null
        });
        itemIconList.Add(new Sprite[] {
            null,
            LoadSpriteByBuildFiled("Wooden Door"),
            null
        });
        itemIconList.Add(new Sprite[] {
            LoadSpriteByBuildFiled("Wall_Wood"),
            LoadSpriteByBuildFiled("Doorway_Wood"),
            LoadSpriteByBuildFiled("Window Frame_Wood")
        });
        itemIconList.Add(new Sprite[] {
            null,
            LoadSpriteByBuildFiled("Floor_Wood"),
            null
        });
        itemIconList.Add(new Sprite[] {
            null,
            LoadSpriteByBuildFiled("Platform_Wood"),
            null
        });
    }



    /// <summary>
    /// 读取单个materialModel  UI图片和模型的名字一样
    /// </summary>
    private void LoadMaterialModel()
    {
        for (int i = 0; i < itemIconList.Count; i++)
        {
            if (itemIconList[i] != null)
            {
                List<GameObject> temp = new List<GameObject>();
                for (int j = 0; j < itemIconList[i].Length; j++)
                {
                    if (itemIconList[i][j] != null)
                    {
                        temp.Add(LoadModelByBuildFiled(itemIconList[i][j].name));
                    }
                }
                materialsModelList.Add(temp);
                Debug.Log("单个类的模型个数为：" + temp.Count);
            }
            else
            {
                materialsModelList.Add(null);
            }
        }
        Debug.Log("长度为：" + materialsModelList.Count);
    }

    /// <summary>
    /// 在建造文件夹中读取Model资源
    /// </summary>
    /// <param name="modelName">资源名</param>
    /// <returns></returns>
    private GameObject LoadModelByBuildFiled(string modelName)
    {
        return Resources.Load<GameObject>("Build/PrefabsModel/" + modelName);
    }

    /// <summary>
    /// 添加数据源，材料的名字
    /// </summary>
    private void AddNameToMaterialsName()
    {
        materialsName.Add(null);
        materialsName.Add(new string[] { "吊灯", "木柱", "梯子" });
        materialsName.Add(new string[] { "屋顶" });
        materialsName.Add(new string[] { "直梯", "L型梯" });
        materialsName.Add(new string[] { "窗户" });
        materialsName.Add(new string[] { "门" });
        materialsName.Add(new string[] { "普通墙壁", "门型墙壁", "窗型墙壁" });
        materialsName.Add(new string[] { "地板" });
        materialsName.Add(new string[] { "地基" });

    }

}
