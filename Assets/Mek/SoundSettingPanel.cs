using UnityEngine;

public class SoundSettingPanel : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        //audioManager = AudioManager.instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void SettingButton()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX("SFX04");
        }
        else
        {
            Debug.LogWarning("SoundSettingPanel: audioManager ยังเป็น null ตอนกดปุ่ม");
        }
    }

}
