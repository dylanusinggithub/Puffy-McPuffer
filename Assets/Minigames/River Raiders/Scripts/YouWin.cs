using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWin : MonoBehaviour
{
    public GameObject youWinPanel;

    // Clicking on the "Quit" button
    public void OnQuitButton() // if the player clicks the "Quit" button
    {
        SceneManager.LoadScene("Level Select Map"); // Level Select screen is loaded
        Time.timeScale = 1f; // Time scale is reset to 1
    }
}
