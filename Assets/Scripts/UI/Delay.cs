using System.Collections;
using UnityEngine;

public class Delay : MonoBehaviour
{
    [SerializeField] private GameObject[] obj = new GameObject[3];

    private IEnumerator ActivateObjects()
    {
        yield return new WaitForSecondsRealtime(1);

        for (int i = 0; i < 3; i++)
        {
            obj[i].SetActive(true);
        }
    }

    public void OnButtonClick()
    {
        StartCoroutine(ActivateObjects());
    }
}