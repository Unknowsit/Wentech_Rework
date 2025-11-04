using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] private string nextSceneName = null;
    [SerializeField] private float loadDelayTime = 5f;
    [SerializeField] private AudioManager audioManager;

    private void Update()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                StartIntro();
            }
    #elif UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartIntro();
            }
    #endif
    }

    private void StartIntro()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioManager.PlaySFX("SFX01");
            StartCoroutine(LoadSceneAfterDelay(loadDelayTime));
        }
    }

    private IEnumerator LoadSceneAfterDelay(float delayTime)
    {
        StartCoroutine(audioManager.FadeOut(audioManager.bgmSource, delayTime));
        StartCoroutine(audioManager.FadeOut(audioManager.ambSource, delayTime));
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync(nextSceneName); // Transition Screen to Main Menu
    }
}