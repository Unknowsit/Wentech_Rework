using UnityEngine;

public class Delay : MonoBehaviour
{
    public GameObject obj,obj2,obj3;

    public void OnButtonClick()
    {
        Invoke("MyFunction", 1f); 
    }

    void MyFunction()
    {
        obj.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(true);
    }
}