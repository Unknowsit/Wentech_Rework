using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool isCounting = true;

    private UIManager ui;
    private GameManager gameManager;

    private void Start()
    {
        ui = UIManager.instance;
        gameManager = GameManager.instance;

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
        }
        else if (ui.RemainingTime == 0)
        {
            ForceShoot();
        }
    }

    private void ForceShoot()
    {
        if (gameManager.cannonShooter.enabled)
        {
            gameManager.cannonShooter.Shoot();
        }
    }
}