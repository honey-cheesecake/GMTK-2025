using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public float timeRemaining;
    public int score = 0;
    public float maxScore = 120;

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI timeRemainingText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI finalScoreText;


    void Start()
    {
        UpdateTimeRemainingText();
        UpdateScoreText();
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0 || score == 120)
        {
            GetScore();
            gameOverScreen.SetActive(true);
            Debug.Log("gameOver");
        }
        UpdateTimeRemainingText();
        UpdateScoreText();
    }


    public float GetScore()
    {
        score = score + ((int)timeRemaining * 5);
        return score;
    }

    private void UpdateTimeRemainingText()
    {
        var timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timeRemainingText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void UpdateScoreText()
    {
        //var timeSpan = TimeSpan.FromSeconds(timeRemaining);
        scoreText.text = string.Format(score + " points");
        finalScoreText.text = string.Format(score + " points");
    }


}
