using UnityEngine;

public class Delay : MonoBehaviour
{
    public GameObject obj;

    public void OnButtonClick()
    {
        Invoke("MyFunction", 1f); 
    }

    void MyFunction()
    {
        obj.SetActive(true);
    }
}