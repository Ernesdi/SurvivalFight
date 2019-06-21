using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 重新加载场景
/// </summary>
public class ReSetScene : MonoBehaviour {

	
	void Update () {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
	}

}
