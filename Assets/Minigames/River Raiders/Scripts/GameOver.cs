using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel; // Game Over panel
    public GameObject gameOverPanel2;

    // Clicking on the "Retry" button
    public void OnRetryButton() // If the player clicks the "Retry" button
    {
        SceneManager.LoadScene("River Raiders"); // Minigame is reloaded
        Time.timeScale = 1f; // Time scale is reset to 1 (gameplay starts again)
    }

    // Clicking on the "Quit" button
    public void OnQuitButton() // if the player clicks the "Quit" button
    {
        SceneManager.LoadScene("Level Select Map"); // Level Select screen is loaded
        Time.timeScale = 1f; // Time scale is reset to 1
    }
}
