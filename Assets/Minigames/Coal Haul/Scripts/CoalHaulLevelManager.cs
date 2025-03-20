using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoalHaulLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel, YouWinPanel, FinalYouWinPanel; //Game Over panel, You Win panel (Level 1 - 4), Final You Win panel (Level)
    [SerializeField] private GameObject[] layouts; //Level layout array
    [SerializeField] private PuffinController puffinController; //Puffin Controller script reference

    private int currentLevel = 0; //Player startes in first level layout (Level 1 = 0, Level 2 = 1, Level 3 = 2, etc.)
    private Vector3 lastPuffinPosition;

    void Start()
    {
        for (int i = 0; i < layouts.Length; i++)
        {
            layouts[i].SetActive(i == 0);
        }

        if (YouWinPanel != null)
        {
            YouWinPanel.SetActive(false); //Disable You Win panel
        }

        if (FinalYouWinPanel != null)
        {
            FinalYouWinPanel.SetActive(false); //Disable Final You Win panel
        }

        lastPuffinPosition = puffinController.transform.position;
    }

    //Enable Next Level Function
    public void NextLevel()
    {
        if (currentLevel < layouts.Length - 1)
        {
            layouts[currentLevel].SetActive(false);
            currentLevel++;
            layouts[currentLevel].SetActive(true);
        }

        YouWinPanel.SetActive(false);
        FinalYouWinPanel.SetActive(false);

        Time.timeScale = 1f;

        if (puffinController != null)
        {
            puffinController.ResetPuffin(lastPuffinPosition);
        }
    }

    //Save Puffin Position
    public void SavePuffinPosition(Vector3 position)
    {
        lastPuffinPosition = position;
    }


    //Enable Retry Level Function
    public void RetryLevel()
    {

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(false); //Disable Game Over panel
        }

        layouts[currentLevel].SetActive(false);
        layouts[currentLevel].SetActive(true);
        if (puffinController != null)
        {
            puffinController.ResetPuffin(lastPuffinPosition); //Puffin character is repositioned at start of whatever level layout the plauyer's on
        }
        Time.timeScale = 1f; //Game resumes
    }

    //Win Game Functions
    public void WinGame()
    {
        Time.timeScale = 0f; //Pause game

        if (currentLevel == layouts.Length - 1) // If player is on the final level
        {
            FinalYouWinPanel.SetActive(true); //Show Final You Win panel
        }

        else
        {
            YouWinPanel.SetActive(true); // Show regular You Win panel
        }
    }

    //Clicking "Level Select" Button
    public void GoToLevelSelect()
    {
        Time.timeScale = 1f; //Game resumes
        SceneManager.LoadScene("Level Select Map"); //Player is transferred back to Level Select Menu
    }
}
