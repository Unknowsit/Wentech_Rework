using UnityEngine;

public class Timer : MonoBehaviour
{
    private UIManager ui;

    private void Start()
    {
        ui = UIManager.instance;
    }

    private void Update()
    {
        if (ui.RemainingTime > 0)
        {
            ui.RemainingTime -= Time.deltaTime;
        }
        else if (UIManager.instance.RemainingTime < 0)
        {
            ui.RemainingTime = 0;
            ui.TimerText.color = Color.red;
        }

        int minutes = Mathf.FloorToInt(ui.RemainingTime / 60);
        int seconds = Mathf.FloorToInt(ui.RemainingTime % 60);
        ui.TimerText.text = $"{minutes:00}:{seconds:00}";
    }
}