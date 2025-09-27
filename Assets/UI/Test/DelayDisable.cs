using UnityEngine;

public class DelayDisable : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;

    public void DisableObj1WithDelay()
    {
        Invoke("DisableObj1", 1f);
    }

    void DisableObj1()
    {
        if (obj1 != null) obj1.SetActive(false);
        if (obj2 != null) obj2.SetActive(true);

    }

    public void DisableObj2WithDelay()
    {
        Invoke("DisableObj2", 1f);
    }

    void DisableObj2()
    {
        if (obj2 != null) obj2.SetActive(false);
        if (obj1 != null) obj1.SetActive(true);
    }

    public void DisableObj3WithDelay()
    {
        Invoke("DisableObj3", 1f);
    }

    void DisableObj3()
    {
        if (obj3 != null) obj3.SetActive(false);
    }

    public void DisableObj4WithDelay()
    {
        Invoke("DisableObj4", 1f);
    }

    void DisableObj4()
    {
        if (obj4 != null) obj4.SetActive(false);
    }
}
