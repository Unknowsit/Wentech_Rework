using UnityEngine;

public class DelayDisable : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;
    public GameObject obj5;
    public GameObject obj6;

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
        
        if (obj3 != null) obj3.SetActive(true);
    }

    public void DisableObj3WithDelay()
    {
        Invoke("DisableObj3", 1f);
    }

    void DisableObj3()
    {
        if (obj3 != null) obj3.SetActive(false);
        if (obj4 != null) obj4.SetActive(true);
    }

    public void DisableObj4WithDelay()
    {
        Invoke("DisableObj4", 1f);
    }

    void DisableObj4()
    {
        if (obj4 != null) obj4.SetActive(false);
        if (obj3 != null) obj3.SetActive(true);
    }

    /// <summary>
    /// //////
    /// </summary>
    public void DisableObj5WithDelay()
    {
        Invoke("DisableObj5", 1f);
    }

    void DisableObj5()
    {
        if (obj1 != null) obj1.SetActive(true);
        if (obj2 != null) obj2.SetActive(false);
    }
    /// <summary>
    /// /////
    /// </summary>
    /// 
    public void DisableObj6WithDelay()
    {
        Invoke("DisableObj6", 1f);
    }

    void DisableObj6()
    {
        if (obj3 != null) obj3.SetActive(false);
        if (obj2 != null) obj2.SetActive(true);
    }
}

