using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; //Pause Panel object
    private bool isPaused = false; //Paused bool is initially set to false

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPaused) //If Space Bar is pressed
        {
            PauseGame(); //Pause Game function is called
        }
    }

    //Pause Game Function
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    //Resume Game Function
    public void ResumeGame()
    {
        UnityEngine.Debug.Log("ResumeGame() called!");
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    //Quit Function
    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
