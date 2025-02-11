using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; //Pause Panel object
    public GameObject optionsPanel; // Options Panel object
    private bool isPaused = false; //Paused bool is initially set to false

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //If Space Bar is pressed
        {
            TogglePause(); //Pause Game function is called
        }
    }

    //Pause Game Function
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    //Resume Game Function
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Open Options Panel from Pause Panel
    public void OpenOptions()
    {
        pausePanel.SetActive(false);  
        optionsPanel.SetActive(true); 
        Time.timeScale = 0;
    }

    // Close Options Panel back to Pause Panel
    public void CloseOptions()
    {
        optionsPanel.SetActive(false); 
        pausePanel.SetActive(true); 
        Time.timeScale = 0;
    }

    //Quit Function
    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
