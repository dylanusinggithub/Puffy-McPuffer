using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Score Text
    public TextMeshProUGUI timeText; // Score Text
    private int score = 0; // Player's score is set at 0
    public GameObject cargo;

    [SerializeField, Range(0f, 20f)]
    int scoreWin;

    [SerializeField, Range(0f, 30f)]
    int timeWin;
    float timeTillWin = 0;

    public GameObject youWin;

    public int GetScore()
    {
        return score; // Provide access to the score
    }

    void Start()
    {
        timeTillWin = timeWin;
    }

    void FixedUpdate()
    {
        if(timeTillWin > 0)
        {
            timeTillWin -= Time.deltaTime;
            timeText.text = "Time Remaining: " + timeTillWin.ToString("F2");
        }
        else
        {
            if (scoreWin == score)
            {
                Time.timeScale = 0;
                youWin.SetActive(true);
            }
        }
    }

    // Add Score Function
    public void AddScore(int points)
    {
        score += points; // Add 1 point to game score
        score = Mathf.Max(score, 0); // Ensure score doesn't go below 0
        UpdateScoreText(); // Update Score Text function is 
        cargo.SetActive(true);
    }

    // Update Score Text Function
    private void UpdateScoreText()
    {
        scoreText.text = "Cargo Collected: " + score.ToString();
    }
}
