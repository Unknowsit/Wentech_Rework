using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour
{
    [Header("Lock UI position")]
    public RectTransform child;        
    public GameObject targetObject;  

    [Header("Link with volume slider")]
    public Slider volumeSlider;        
    public Image volumeImage;         

    Vector2 fixedPos;
    bool canLock = false;

    void Awake()
    {
        if (child != null)
            fixedPos = child.anchoredPosition;
    }

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
            OnSliderChanged(volumeSlider.value); 
        }
    }

    void LateUpdate()
    {

        if (canLock && child != null)
        {
            child.anchoredPosition = fixedPos;  
        }
    }

    public void OnButtonPressed()
    {
        StartCoroutine(DelayLock());
    }

    IEnumerator DelayLock()
    {
        yield return new WaitForSeconds(0.5f); 
        canLock = true;                     

        yield return new WaitForSeconds(0.2f);
        targetObject.SetActive(true);
    }

    public void UnlockPosition()
    {
        canLock = false;       
    }

    void OnSliderChanged(float value)
    {
        if (volumeImage != null)
        {
            volumeImage.fillAmount = Mathf.Clamp01(value);
        }
    }
}
