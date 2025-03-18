using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CoalHaulLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel, YouWinPanel;
    [SerializeField] private GameObject[] layouts;
    [SerializeField] private PuffinController puffinController;

    private int currentLevel = 0;
    private Vector3 lastPuffinPosition;

    void Start()
    {
        for (int i = 0; i < layouts.Length; i++)
        {
            layouts[i].SetActive(i == 0);
        }

        if (YouWinPanel != null)
        {
            YouWinPanel.SetActive(false);
        }

        lastPuffinPosition = puffinController.transform.position;
    }

    public void NextLevel()
    {
        if (currentLevel < layouts.Length - 1)
        {
            layouts[currentLevel].SetActive(false);
            currentLevel++;
            layouts[currentLevel].SetActive(true);
        }

        YouWinPanel.SetActive(false);
        Time.timeScale = 1f;

        if (puffinController != null)
        {
            puffinController.ResetPuffin(lastPuffinPosition);
        }
    }

    public void SavePuffinPosition(Vector3 position)
    {
        lastPuffinPosition = position;
    }

    public void RetryLevel()
    {

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(false);
        }

        layouts[currentLevel].SetActive(false);
        layouts[currentLevel].SetActive(true);
        if (puffinController != null)
        {
            puffinController.ResetPuffin(lastPuffinPosition);
        }
        Time.timeScale = 1f;
    }
}
