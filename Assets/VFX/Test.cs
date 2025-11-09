using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Test : MonoBehaviour
{

    [SerializeField] private string nextSceneName = null;
    [SerializeField] private float loadDelayTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartIntro()
    {
        StartCoroutine(LoadSceneAfterDelay(loadDelayTime));
    }

    private IEnumerator LoadSceneAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync(nextSceneName); // Transition Screen to Main Menu
    }
}
