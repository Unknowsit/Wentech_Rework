using UnityEngine;
using System.Collections;
public class ButtonDelay : MonoBehaviour
{
    public GameObject objectToActivate;

    
    public float delay = 5f;

    void Start()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    IEnumerator ActivateAfterDelay()
    {
        
        yield return new WaitForSeconds(delay);

        
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
