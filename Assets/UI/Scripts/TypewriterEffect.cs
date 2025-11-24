using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    private AudioManager audioManager;

    public TMP_Text uiText;
    public string fullText = "";
    public float delay = 0.05f;

    void Start()
    {
        audioManager = AudioManager.instance;
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        uiText.text = "";

        audioManager.PlaySFX("KeyboardTyping");

        foreach (char c in fullText)
        {
            uiText.text += c;
            yield return new WaitForSeconds(delay);
        }

        audioManager.sfxSource.Stop();
    }
    void OnDisable()
    {
        if (audioManager != null && audioManager.sfxSource.isPlaying)
            audioManager.sfxSource.Stop();
    }
}
