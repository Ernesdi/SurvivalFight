using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;
using System.IO;
using System;

/// <summary>
/// 文件操作类
/// </summary>
public class FileOperation  {
    //存入Json文件
    public static void SaveToJsonFileRegex(JsonData jsonData, string path)
    {

        string str = jsonData.ToJson();
        //json存入中文后会被转义成编码，使用正则显示中文
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        string modifyString = reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        //文件流写入UTF8格式内容
        FileStream file = new FileStream(path, FileMode.Create);
        byte[] bts = System.Text.Encoding.UTF8.GetBytes(modifyString);
        file.Write(bts, 0, bts.Length);
        //关闭文件流
        if (file != null)
        {
            file.Close();
        }
    }

    //存入Json文件
    public static void SaveToJsonFileIO(JsonData jsonData, string path)
    {
        string jsonStr = JsonMapper.ToJson(jsonData);
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        StreamWriter sw = fileInfo.CreateText();
        sw.Write(jsonStr);
        sw.Flush();
        sw.Close();
        sw.Dispose();
    }

}
