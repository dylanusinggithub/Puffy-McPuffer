using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Score  text element
    private int score = 0; // Player's score

    // Method to increase score
    public void AddScore(int points)
    {
        score += points; // Add 1 point to game score
        UpdateScoreText(); // Update the Score text
    }

    // Update the UI text
    private void UpdateScoreText()
    {
        scoreText.text = "Cargo Collected: " + score.ToString();
    }
}
