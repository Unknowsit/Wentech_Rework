using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickDeselect : MonoBehaviour, IPointerClickHandler
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OperatorBalloonClickHandler selected = OperatorBalloonClickHandler.GetCurrentlySelected();

        if (selected != null)
        {
            selected.Deselect();
            audioManager.PlaySFX("SFX02");
        }
    }
}