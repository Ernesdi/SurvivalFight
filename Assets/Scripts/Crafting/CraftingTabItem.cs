using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 种类的实体类
/// </summary>
public class CraftingTabItem  {

    private string category;
 
    public string Category { get { return category; }set { category = value; } }

    public CraftingTabItem(){}

    public CraftingTabItem(string category)
    {
        this.Category = category;  
    }

    public override string ToString()
    {
        return string.Format("种类名称为："+ category); 
    }

}
