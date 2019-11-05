using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundmanager;
    private void Awake()
    {
        if (soundmanager == null)
        {
            soundmanager = this;
        }
        else if (soundmanager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public AudioSource sfx;

    public void SFXPlay(AudioClip a)
    {
        sfx.clip = a;
        sfx.Play();
    }
}
