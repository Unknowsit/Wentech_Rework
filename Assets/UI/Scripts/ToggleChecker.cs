using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleChecker : MonoBehaviour
{
    public Toggle[] toggles;       
    public GameObject objectA;      
    public GameObject objectB;    
    public GameObject objectC;
    public float ObjectDelayTime = 1.0f;
    public GameObject objectDelay;


    public void ToggleObjectA()
    {
        if (objectA != null)
            objectA.SetActive(!objectA.activeSelf);
    }

   
    public void DeleteObjectB()
    {
        if (objectB != null)
        {
            Destroy(objectB);
            objectB = null;  // กัน error ใน Update
        }
    }
    public void DeleteObjectC()
    {
        if (objectC != null)
        {
            Destroy(objectC);
            objectC = null;  // กัน error ใน Update
        }
    }

    private void Start()
    {
        StartCoroutine(ObjectDelays(objectDelay));
    }

    void Update()
    {
        int count = 0;
        foreach (var t in toggles)
        {
            if (t != null && t.isOn)
                count++;
        }

        if (objectB != null)
            objectB.SetActive(count > 1);

        if (objectC != null)
            objectC.SetActive(count == 1);
    }

    IEnumerator ObjectDelays(GameObject target)
    {
        yield return new WaitForSeconds(ObjectDelayTime);
        objectDelay.SetActive(true);
    }
}
