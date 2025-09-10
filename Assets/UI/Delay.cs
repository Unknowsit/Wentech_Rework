using UnityEngine;

public class Delay : MonoBehaviour
{
    public GameObject obj,obj2,obj3,obj4;

    public void OnButtonClick()
    {
        Invoke("MyFunction", 1f);
        Invoke("NewO", 3f);
    }

    void MyFunction()
    {
        obj.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(true);
        obj4.SetActive(false);
    }

    void NewO()
    {
        
    }
}