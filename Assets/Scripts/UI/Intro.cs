using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] private string nextSceneName = null;
    [SerializeField] private float loadDelayTime = 5f;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        IntroGame();
    }

    private void IntroGame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(LoadSceneAfterDelay(loadDelayTime));
        }
    }

    private IEnumerator LoadSceneAfterDelay(float delayTime)
    {
        //yield return new WaitForSeconds(delayTime);
        StartCoroutine(audioManager.FadeOut(audioManager.bgmSource, delayTime));
        StartCoroutine(audioManager.FadeOut(audioManager.ambSource, delayTime));
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync(nextSceneName); // Transition Screen to Main Menu
        AudioManager.instance.PlayBGM("BGM01");
        AudioManager.instance.PlayAmbient("SFX03");
    }

    /*private IEnumerator FadeOutBGM(float duration)
    {
        AudioSource bgm = AudioManager.instance.bgmSource;
        float startVolume = bgm.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgm.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
    }*/
}