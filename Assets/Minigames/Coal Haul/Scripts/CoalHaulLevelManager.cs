using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class CoalHaulLevelManager : MonoBehaviour
{
    [SerializeField] GameObject GameOverPanel, YouWinPanel;

    [SerializeField] private CoalHaulLevelSettings[] Levels; 
    private int currentLevelIndex;

    GameObject startZone, endZone, wallParent;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("difficulty", 0);

        startZone = GameObject.Find("StartZone");
        endZone = GameObject.Find("EndZone");
        wallParent = GameObject.Find("WallsParent");

        LoadLevelSettings();
    }

    private void LoadLevelSettings()
    {
        if (currentLevelIndex >= Levels.Length) return;

        CoalHaulLevelSettings level = Levels[currentLevelIndex];

        startZone.transform.position = level.startZonePosition;
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
        foreach (GameObject layout in layouts)
            layout.SetActive(true);
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < Levels.Length)
        {
            PlayerPrefs.SetInt("difficulty", currentLevelIndex);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select");
        }
    }
}

[System.Serializable]
class CoalHaulLevelSettings
{
    [Header("Level Difficulty")]
    public int levelIndex;
    public float puffinSpeed;
    public float wallGapSize;

    [Header("Layout Settings")]
    public GameObject[] levelLayouts;

    [Header("Position Settings")]
    public Vector3 startZonePosition;
    public Vector3 endZonePosition;
    public Vector3[] wallPositions;
}
