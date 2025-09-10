using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioData[] bgm, sfx;
    public AudioSource bgmSource, sfxSource;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        PlayBGM("Savfk");
    }

    public void PlayBGM(string name)
    {
        AudioData audioData = Array.Find(bgm, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("Background source not found");
        }
        else
        {
            bgmSource.clip = audioData.clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        AudioData audioData = Array.Find(sfx, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("SFX source not found");
        }
        else
        {
            sfxSource.PlayOneShot(audioData.clip);
        }
    }

    public void ToggleBGM()
    {
        bgmSource.mute = !bgmSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void BGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}