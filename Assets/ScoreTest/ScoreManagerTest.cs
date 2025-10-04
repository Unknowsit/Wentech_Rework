using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ScoreManagerTest : MonoBehaviour
{
    //นำGameObj Text มาลากใส่
    [SerializeField] public TextMeshProUGUI scoreTextP1;
    [SerializeField] public TextMeshProUGUI scoreTextP2;
    [SerializeField] public TextMeshProUGUI winText;
    [SerializeField] public TextMeshProUGUI loseText;
    [SerializeField] public TextMeshProUGUI drawText;

    [SerializeField] GameObject resultPanel;

    private int scoreP1 = 0;
    private int scoreP2 = 0;

    private bool countingP1 = false;
    private bool countingP2 = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreTextP1.text = "" + scoreP1;
        scoreTextP1.text = "" + scoreP2;
        if (resultPanel) resultPanel.SetActive(false);
    }

    public void Show()
    {

    }

    //ส่วนนี้เอาใว้เรียกใช้CountScore
    public void AddScoreP1(bool isP1)
    {
        if (isP1)
        {
            if (!countingP1) StartCoroutine(CountScore(true));
        }
        else
        {
            if (!countingP2) StartCoroutine(CountScore(false));
        }
    }


    //ใว้ทำการเพิ่มคะแนนในplayer
    IEnumerator CountScore(bool isP1)
    {
        if (isP1)
            countingP1 = true;
        else
            countingP2 = true;

        int current;
        if (isP1) current = scoreP1;
        else current = scoreP2;

        int target = current + 1000;//เลขตั้งใหม่ได้

        //Loopให้คะแนนเป็นตัวเลขค่อยๆเพิ่ม
        while (current < target)
        {
            //current++;

            int diff = target - current;
            int step = Mathf.Max(1, Mathf.CeilToInt(diff * 0.5f)); // บวกที่ล่ะ 50% (ปรับความเร็วได้)

            current += step;
            if (current > target) current = target;

            if (isP1)
            {
                scoreP1 = current;
                scoreTextP1.text = scoreP1.ToString();
            }
            else
            {
                scoreP2 = current;
                scoreTextP2.text = scoreP2.ToString();
            }

            yield return new WaitForSeconds(0.05f);
        }

        if (isP1)
            countingP1 = false;
        else
            countingP2 = false;
    }

    public void ShowWinnerPanel()
    {
        if (!resultPanel) return;

        resultPanel.SetActive(true);

        if (scoreP1 > scoreP2)
        {
            winText.text = $"Player 1 : " + scoreP1.ToString();
            drawText.gameObject.SetActive(false);
            loseText.text = $"Player 2 : " + scoreP2.ToString();
        }
        else if (scoreP2 > scoreP1)
        {
            winText.text = $"Player 2 : " + scoreP2.ToString();
            drawText.gameObject.SetActive(false);
            loseText.text = $"Player 1 : " + scoreP1.ToString();
        }
        else
        {
            winText.text = $"Player 1 : " + scoreP1.ToString();
            drawText.gameObject.SetActive(true);
            loseText.text = $"Player 2 : " + scoreP2.ToString();
        }
    }

    public void CloseWinnerPanel()
    {
        if (resultPanel) resultPanel.SetActive(false);
    }
}
