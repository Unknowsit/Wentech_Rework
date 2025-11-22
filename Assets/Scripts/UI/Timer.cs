using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool isCounting = true;
    private bool isWarningPlaying = false;

    [Header("Warning Settings")]
    [SerializeField] private float warningThreshold = 10f;

    private AudioManager audioManager;
    private GameManager gameManager;
    private UIManager ui;

    private void Start()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
        ui = UIManager.instance;

        ui._ProgressBar.maxValue = ui.RemainingTime;
        ui._ProgressBar.value = ui.RemainingTime;
    }

    private void Update()
    {
        if (!isCounting) return;

        if (ui.RemainingTime > 0)
        {
            ui.RemainingTime = Mathf.Max(0, ui.RemainingTime - Time.deltaTime);
            ui._ProgressBar.value = ui.RemainingTime;

            if (ui.RemainingTime <= warningThreshold && !isWarningPlaying)
            {
                PlayWarningMusic();
            }
        }
        else if (ui.RemainingTime == 0)
        {
            ForceShoot();
        }
    }

    private void PlayWarningMusic()
    {
        audioManager.PlaySFX("SFX07");
        isWarningPlaying = true;
    }

    public void StopWarningMusic()
    {
        audioManager.sfxSource.Stop();
        isWarningPlaying = false;
    }

    private void ForceShoot()
    {
        if (gameManager.cannonShooter.enabled)
        {
            gameManager.cannonShooter.Shoot();
        }
    }
}