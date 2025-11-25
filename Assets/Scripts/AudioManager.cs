using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public float savedBGMVolume = 1f;
    [HideInInspector] public float savedSFXVolume = 1f;

    public AudioData[] bgmData, sfxData, ambData;
    public AudioSource bgmSource, sfxSource, ambSource;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettingsOnStart();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Intro")
        {
            PlayBGM("BGM03");
            PlayAmbient("ABG1");
        }
        else if (scene.name == "Mainmenu")
        {
            StartCoroutine(FadeIn(bgmSource, 3f));
            PlayBGM("BGM02");

            if (ambSource != null) ambSource.Stop();
        }
        else if (scene.name == "Operators")
        {
            // Code Here
        }
        else if (scene.name == "Gameplay")
        {
            // Code Here
            StartCoroutine(FadeIn(bgmSource, 3f));
            PlayBGM("BGM01");
        }
    }

    private void LoadAudioSettingsOnStart()
    {
        savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        bgmSource.volume = savedBGMVolume;
        sfxSource.volume = savedSFXVolume;
        ambSource.volume = 1.0f;

        bool bgmMute = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        bool sfxMute = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        bgmSource.mute = bgmMute;
        sfxSource.mute = sfxMute;

        Debug.Log($"Audio settings loaded! BGM Vol: {savedBGMVolume}, SFX Vol: {savedSFXVolume}, BGM Mute: {bgmMute}, SFX Mute: {sfxMute}");
    }

    public void PlayBGM(string name)
    {
        AudioData audioData = Array.Find(bgmData, element => element.audioName == name);

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
        AudioData audioData = Array.Find(sfxData, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("SFX source not found");
        }
        else
        {
            sfxSource.PlayOneShot(audioData.clip);
        }
    }

    public void PlayAmbient(string name)
    {
        AudioData audioData = Array.Find(ambData, element => element.audioName == name);

        if (audioData == null)
        {
            Debug.Log("Ambient source not found");
        }
        else
        {
            ambSource.clip = audioData.clip;
            ambSource.volume = 1.0f;
            ambSource.Play();
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
        savedBGMVolume = volume;
        bgmSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        savedSFXVolume = volume;
        sfxSource.volume = volume;
    }

    public IEnumerator FadeIn(AudioSource source, float duration)
    {
        if (source == ambSource) yield break;

        float targetVolume = (source == bgmSource) ? savedBGMVolume : savedSFXVolume;

        source.volume = 0f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public IEnumerator FadeOut(AudioSource source, float duration)
    {
        if (source == ambSource) yield break;

        float startVolume = source.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;

        float targetVolume = (source == bgmSource) ? savedBGMVolume : savedSFXVolume;
        source.volume = targetVolume;
    }

    /*
    public IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0f;
        float startVolume = 0f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 1f, t / duration);
            yield return null;
        }

        source.volume = 1f;
    }

    public IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 1f;
    }
    */
}