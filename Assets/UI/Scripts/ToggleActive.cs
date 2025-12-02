using UnityEngine;
using UnityEngine.UI;

public class ToggleActive : MonoBehaviour
{
    public Toggle toggle;          
    public GameObject targetObject; 

    void Start()
    {
        
        toggle.onValueChanged.AddListener(OnToggleChanged);

        
        OnToggleChanged(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        if (targetObject != null)
            targetObject.SetActive(isOn);  
    }
}
