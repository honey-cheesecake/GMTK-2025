using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] float timeRemaining;
    int score = 0;

    [SerializeField] CatManager catManager;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI timeRemainingText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] AudioSource audioSource;

    // /[Header("Sound Script")]
    //[SerializeField] soundsScript soundScript;

    //Sound variables
    private Dictionary<AudioClip, float> lastPlayTime = new Dictionary<AudioClip, float>();
    private float minTimeBetweenSameSounds = 0.2f;

    bool isGameRunning = true;

    void Start()
    {
        UpdateTimeRemainingText();
        UpdateScoreText();
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (isGameRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingText();

            if (timeRemaining <= 0 || catManager.getCatchableCats().Count == 0)
            {
                isGameRunning = false;

                timeRemaining = 0;
                UpdateTimeRemainingText();
                
                score = GetFinalScore();
                UpdateScoreText();
                gameOverScreen.SetActive(true);
            }
        }  
    }

    public void AddToScore(int change)
    {
        if (!isGameRunning)
        {
            return;
        }
        Debug.Assert(change >= 0);
        score += change;
        UpdateScoreText();
        //PlayCatSound(clip);
    }

    int GetFinalScore()
    {
        if (timeRemaining >= 0)
        {
            return score + ((int)timeRemaining * 5);
        }
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
    
    public void PlayCatSound(AudioClip clip)
    {
        if (lastPlayTime.ContainsKey(clip) && 
            Time.time - lastPlayTime[clip] < minTimeBetweenSameSounds)
        {
            return; // Don't play if too recent
        }
        
        audioSource.PlayOneShot(clip);
        lastPlayTime[clip] = Time.time;
    }


}
