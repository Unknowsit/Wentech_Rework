using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioData[] backgroundList, sfxList;
    public AudioSource backgroundSource, sfxSource;

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
        PlayBackground("Savfk");
    }

    public void PlayBackground(string name)
    {
        AudioData audioData = Array.Find(backgroundList, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("Background source not found");
        }
        else
        {
            backgroundSource.clip = audioData.clip;
            backgroundSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        AudioData audioData = Array.Find(sfxList, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("SFX source not found");
        }
        else
        {
            sfxSource.PlayOneShot(audioData.clip);
        }
    }
}