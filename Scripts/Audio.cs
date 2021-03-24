using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{

    private static Audio _instance;

    public static Audio Instance { get { return _instance; } }

    public AudioSource[] audioSources;

    public AudioClip Run, Attack, Jump, Hit, launch, swoosh, BG1, BG2, typeWriter, drawSword,
        win;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
    }

    public void playerSound(AudioClip clip)
    {
        audioSources[0].PlayOneShot(clip);
    }

    public void enemySound(AudioClip audioClip)
    {
        audioSources[1].Stop();
        audioSources[1].clip = audioClip;
        audioSources[1].Play();
    }

    public void itemSound(AudioClip audioClip)
    {
        audioSources[2].Stop();
        audioSources[2].clip = audioClip;
        audioSources[2].Play();
    }

    public void UISound(AudioClip audioClip)
    {
        audioSources[3].Stop();
        audioSources[3].clip = audioClip;
        audioSources[3].Play();
    }

    public void BGsound(AudioClip audioClip)
    {
        audioSources[4].Stop();
        audioSources[4].clip = audioClip;
        audioSources[4].Play();
    }

}
