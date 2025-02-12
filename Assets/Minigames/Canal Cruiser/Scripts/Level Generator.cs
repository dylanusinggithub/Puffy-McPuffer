using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] bool PressMeToSetLevelIndex;
    [SerializeField] int levelIndex;

    [SerializeField] LevelSettings[] Levels;
    List<GameObject> CreatesLocation = new List<GameObject>();

    [SerializeField, Range(-5, 50)] float LayoutSeperation;
    float LevelWidth = 0;

    private void OnValidate()
    {
        if (PressMeToSetLevelIndex)
        {
            PressMeToSetLevelIndex = false;
            PlayerPrefs.SetInt("difficulty", levelIndex);
        }
    }

    private void Start()
    {
        // sets the level index to minigameIndex which is provided by level desginer in menu screen
        levelIndex = PlayerPrefs.GetInt("difficulty", 0);

        GameObject.Find("Game Manager").GetComponent<ScoreScript>().timeWin = Levels[levelIndex].GameplayTime;
        GameObject.Find("Player").GetComponent<PlayerScript>().startSpeed = Levels[levelIndex].LevelSpeed;
        GameObject.Find("Game Manager").GetComponent<ScoreScript>().scoreWin = Levels[levelIndex].CreateCompletion;

        TextMeshProUGUI scoreText = GameObject.Find("Cargo Counter").GetComponent<TextMeshProUGUI>();
        scoreText.text = "Cargo Collected: 0 / " + Levels[levelIndex].CreateCompletion;

        // Places each layout in order
        for (int i = 0; i < Levels[levelIndex].Length; i++)
        {
            // Finds the furthest away object in the chosen layout
            float furthestElement = 0;    
            for (int j = 0; j < Levels[levelIndex].Layouts[i].transform.childCount; j++)
            {
                if (Levels[levelIndex].Layouts[i].transform.GetChild(j).transform.localPosition.x > furthestElement)
                {
                    furthestElement = Levels[levelIndex].Layouts[i].transform.GetChild(j).transform.localPosition.x;
                }
            }

            // Because the layouts are centred at 0,0 the furthest element's distance mulitplied would give you the total width
            LevelWidth += furthestElement * 2; 

            Vector3 pos = new Vector3(LevelWidth, 0, 0);
            Instantiate(Levels[levelIndex][i], pos, Quaternion.identity, transform);

            LevelWidth += LayoutSeperation;
        }

        // Grabs all creates and disables them to later reenable a specific ones
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                if (transform.GetChild(i).GetChild(j).name.Contains("CratePrefab"))
                {
                    CreatesLocation.Add(transform.GetChild(i).GetChild(j).gameObject);
                    transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
                }
            }
        }

        // Goes through all create locations and chooses a random one to reenable "Spawn"
        for (int i = 0; i < Levels[levelIndex].CreateCompletion + Levels[levelIndex].ExtraCreates; i++)
        {
            int randomIndex = Random.Range(0, CreatesLocation.Count);
            CreatesLocation[randomIndex].SetActive(true);
            CreatesLocation.RemoveAt(randomIndex);
        }

    }
}


// I Wanted to have multi-dimentional array of gameobjects so that desginers can just drag and drop
// layouts to create new levels entirely by theirself, so thank you Rabwin
// https://discussions.unity.com/t/how-to-declare-a-multidimensional-array-of-string-in-c/21138

[System.Serializable]
class LevelSettings
{
    [Range(5, 30)] public int GameplayTime;
    [Range(0, 10)] public int CreateCompletion;
    [Range(0, 5)] public int ExtraCreates;

    [Range(0, 4)] public float LevelSpeed;

    public GameObject[] Layouts;

    public GameObject this[int index]
    {
        get
        {
            return Layouts[index];
        }

        set
        {
            Layouts[index] = value;
        }
    }

    public int Length
    {
        get
        {
            return Layouts.Length;
        }
    }
}