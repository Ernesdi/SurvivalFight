using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	
	void Update () {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AudioManager.Instance.PlayAudioClipByName(ClipName.BoarAttack, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AudioManager.Instance.PlayAudioClipByComponent(gameObject, ClipName.PlayerBreathingHeavy);
        }
    }
}
