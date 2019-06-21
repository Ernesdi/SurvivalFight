using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;
    private Transform m_Transform;
    private AudioClip[] audioClip;
    private Dictionary<string, AudioClip> aiduoClipDic;

    void Awake () {
        Instance = this;
        m_Transform = gameObject.GetComponent<Transform>();
        audioClip = Resources.LoadAll<AudioClip>("Soures/All/");
        aiduoClipDic = new Dictionary<string, AudioClip>();
        for (int i = 0; i < audioClip.Length; i++)
        {
            
            aiduoClipDic.Add(audioClip[i].name, audioClip[i]);
        }
    }

    /// <summary>
    /// 通过名字来获取AudioClip
    /// </summary>
    private AudioClip GetClipAudioByName(string clipName)
    {
        AudioClip temp;
        aiduoClipDic.TryGetValue(clipName, out temp);
        return temp;
    }

    /// <summary>
    /// 通过PlayClipAtPoint来播放一个声音
    /// </summary>
    /// <param name="audioType">声音的枚举</param>
    /// <param name="pos">声音播放的位置</param>
    public void PlayAudioClipByName(ClipName audioType,Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(GetClipAudioByName(audioType.ToString()), pos);
    }

    /// <summary>
    /// 通过添加组件的方式来播放声音
    /// </summary>
    /// <param name="go">要将AudioSource添加到的物体</param>
    /// <param name="audioType">声音的类型</param>
    /// <param name="isPlayOnWake">是否一开始就播放</param>
    /// <param name="isLoop">是否循环</param>
    public AudioSource PlayAudioClipByComponent(GameObject go, ClipName audioType, bool isPlayOnWake=true, bool isLoop=true)
    {
        AudioSource tempAudioSource = go.AddComponent<AudioSource>();
        tempAudioSource.clip = GetClipAudioByName(audioType.ToString());
        tempAudioSource.playOnAwake= isPlayOnWake;
        //一开始就播放（因为虽然设置了但是需要我们手动播放一下）
        if (isPlayOnWake)
            tempAudioSource.Play();
        tempAudioSource.loop= isLoop;
        return tempAudioSource;
    }


}
