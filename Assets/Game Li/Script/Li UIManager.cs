using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiUIManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI scoreText; // 拖拽UI上的Text组件到这里
    private int score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
