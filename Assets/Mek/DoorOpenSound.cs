using UnityEngine;

public class DoorCloseSound : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }
    
    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.instance;

        if (audioManager != null)
        {
            audioManager.PlaySFX("SFX10");   // เล่นเสียงเปิดประตู
        }
        else
        {
            Debug.LogWarning("DoorOpenSound : AudioManager not found in scene.");
        }
    }
}
