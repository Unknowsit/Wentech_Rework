using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] private string nextSceneName = null;
    [SerializeField] private float loadDelayTime = 5f;

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
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync(nextSceneName); // Transition Screen to Main Menu
    }
}