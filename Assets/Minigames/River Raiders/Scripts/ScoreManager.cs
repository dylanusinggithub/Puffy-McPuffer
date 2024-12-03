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
    public GameObject cargo2;
    public GameObject cargo3;

    [SerializeField, Range(0f, 20f)]
    int scoreWin;

    [SerializeField, Range(0f, 50f)]
    int timeWin;
    public float timeTillWin = 0;

    public GameObject youWin, gameOver, gameOver2;

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
        if (Time.timeScale == 0) return; // Skip if the game is already stopped

        if (timeTillWin > 0)
        {
            timeTillWin -= Time.deltaTime;
            timeText.text = "Time Remaining: " + timeTillWin.ToString("F2");
        }
        else
        {
            if (scoreWin <= score) youWin.SetActive(true);
            else gameOver.SetActive(true);

            Time.timeScale = 0;
        }

        if (score == 1)
        {
            cargo.SetActive(true);
            cargo2.SetActive(false);
            cargo3.SetActive(false);
        }
        else if (score == 2)
        {
            cargo2.SetActive(true);
            cargo3.SetActive(false);
        }
        else if (score == 3)
        {
            cargo3.SetActive(true);
        }
    }

    // Add Score Function
    public void AddScore(int points)
    {
        score += points; // Add 1 point to game score
        score = Mathf.Max(score, 0); // Ensure score doesn't go below 0
        UpdateScoreText(); // Update Score Text function is 

        // Check if score is 0, indicating all cargo is lost
        if (score == 0)
        {
            gameOver2.SetActive(true); // Show the "Game Over 2" panel
            Time.timeScale = 0; // Stop the game
        }
    }

    // Update Score Text Function
    private void UpdateScoreText()
    {
        scoreText.text = "Cargo Collected: " + score.ToString() + "/ " + scoreWin.ToString();
    }
}
