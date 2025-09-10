using UnityEngine;

public class Timer : MonoBehaviour
{
    private UIManager ui;

    private void Awake()
    {
        ui = UIManager.instance;
    }

    private void Start()
    {
        ui._ProgressBar.maxValue = ui.RemainingTime;
        ui._ProgressBar.value = ui.RemainingTime;
    }

    private void Update()
    {
        if (ui.RemainingTime > 0)
        {
            ui.RemainingTime = Mathf.Max(0, ui.RemainingTime - Time.deltaTime);
            ui._ProgressBar.value = ui.RemainingTime;
        }
    }
}