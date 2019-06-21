using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyMenu
{

    [MenuItem("MyMenu/BuildDataOfWindow")]
    public static void BuildDataOfWindow()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
