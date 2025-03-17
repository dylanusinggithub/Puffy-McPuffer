using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CoalHaulLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel, YouWinPanel;

    [SerializeField] private CoalHaulLevelSettings[] Levels; 
    private int currentLevelIndex;

    private GameObject endZone, wallParent;
    private GameObject activeLayout;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("difficulty", 0);

        endZone = GameObject.Find("EndZone");
        wallParent = GameObject.Find("WallParent");

        LoadLevelSettings();
    }

    private void LoadLevelSettings()
    {
        if (Levels == null || Levels.Length == 0)
        {
            UnityEngine.Debug.LogError("Levels array is null or empty in CoalHaulLevelManager!");
            return;
        }

        if (currentLevelIndex >= Levels.Length) return;

        CoalHaulLevelSettings level = Levels[currentLevelIndex];

        if (level == null)
        {
            UnityEngine.Debug.LogError("Level settings for index " + currentLevelIndex + " is null!");
            return;
        }

        endZone.transform.position = level.endZonePosition;

        SetWallPositions(level.wallPositions);
        ActivateLevelLayouts(level.levelLayouts);
    }

    void SetWallPositions(Vector3[] positions)
    {
        Transform[] walls = wallParent.GetComponentsInChildren<Transform>();
        for (int i = 1; i < walls.Length; i++)
        {
            if (i - 1 < positions.Length)
                walls[i].position = positions[i - 1];
            else
                walls[i].gameObject.SetActive(false);
        }
    }

    void ActivateLevelLayouts(GameObject[] layouts)
    {
        if (activeLayout != null)
            activeLayout.SetActive(false);

        if (layouts.Length > currentLevelIndex)
        {
            activeLayout = layouts[currentLevelIndex];
            activeLayout.SetActive(true);
        }
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < Levels.Length)
        {
            PlayerPrefs.SetInt("difficulty", currentLevelIndex);
            LoadLevelSettings();
            YouWinPanel.SetActive(false);
        }

        else
        {
            UnityEngine.Debug.Log("All Levels Completed!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            YouWinPanel.SetActive(true);
        }
    }
}

[System.Serializable]
class CoalHaulLevelSettings
{
    [Header("Level Difficulty")]
    public int levelIndex;

    [Header("Layout Settings")]
    public GameObject[] levelLayouts;
    public GameObject activeLayout;

    [Header("Position Settings")]
    public Vector3 startZonePosition;
    public Vector3 endZonePosition;
    public Vector3[] wallPositions;
}
