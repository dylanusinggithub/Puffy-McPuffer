using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] bool PressMeToSetLevelIndex;
    [SerializeField] int levelIndex;

    [SerializeField] LevelSettings[] Levels;
    List<GameObject> CreatesLocation = new List<GameObject>();

    float LevelWidth = 0;

    [SerializeField] Sprite FinishLine;

    ScoreScript SS;
    GameObject Puffy;

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
        Puffy = GameObject.Find("Moving Thing");
        SS = GameObject.Find("Game Manager").GetComponent<ScoreScript>();

        // sets the level index to minigameIndex which is provided by level desginer in menu screen
        levelIndex = PlayerPrefs.GetInt("difficulty", 0);

        GameObject.Find("Game Manager").GetComponent<ScoreScript>().timeWin = Levels[levelIndex].GameplayTime;
        GameObject.Find("Player").GetComponent<PlayerScript>().startSpeed = Levels[levelIndex].LevelSpeed;
        GameObject.Find("Game Manager").GetComponent<ScoreScript>().scoreWin = Levels[levelIndex].CreateCompletion;

        // 2 is the speed it is pegged at so if the levelSpeed is 1.75f then it'll play at 75% speed
        GameObject.Find("Water Swiggles").GetComponent<Animator>().SetFloat("Speed", Levels[levelIndex].LevelSpeed / 2); 

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

            LevelWidth += Levels[levelIndex].LayoutSeperation;
        }

        GameObject finish = new GameObject("FinishLine");

        float dist = 4.6f * Levels[levelIndex].LevelSpeed;
        finish.transform.position = new Vector3(dist * Levels[levelIndex].GameplayTime, 0);
        finish.transform.localScale = Vector3.one * 0.9f;

        finish.AddComponent<SpriteRenderer>().sprite = FinishLine;

        // Grabs all creates and disables them to later reenable a specific ones
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                Transform OBJ = transform.GetChild(i).GetChild(j);
                if (OBJ.name.Contains("CratePrefab"))
                {
                    CreatesLocation.Add(OBJ.gameObject);
                    OBJ.gameObject.SetActive(false);
                }
                else if(OBJ.GetComponent<ObjectScript>().isHardmode) OBJ.gameObject.SetActive(false);
            }
        }

        // Goes through all create locations and chooses a random one to reenable "Spawn"
        int initalCreateCount = CreatesLocation.Count;
        for (int i = 0; i < Levels[levelIndex].CreateCompletion + Levels[levelIndex].ExtraCreates && i < initalCreateCount; i++)
        {
            int randomIndex = Random.Range(0, CreatesLocation.Count);
            CreatesLocation[randomIndex].SetActive(true);
            CreatesLocation.RemoveAt(randomIndex);
        }

    }

    float HardmodeCheckTime = 1;
    void FixedUpdate()
    {
        if (HardmodeCheckTime > 0) HardmodeCheckTime -= Time.deltaTime;
        else
        {
            HardmodeCheckTime = 1f;

            if (SS.score < Levels[levelIndex].CreateCompletion) return;

            float closestLayout = 1000;
            Transform closestLayoutObject = transform.GetChild(0);
            foreach (Transform layout in GetComponentInChildren<Transform>())
            {
                float dist = (layout.position - Puffy.transform.position).magnitude;
                if (dist < closestLayout)
                {
                    closestLayout = dist;
                    closestLayoutObject = layout;
                }
            }

            if (closestLayoutObject.GetSiblingIndex() < transform.childCount - 1)
            {
                GameObject NextLayout = transform.GetChild(closestLayoutObject.GetSiblingIndex() + 1).gameObject;

                foreach (Transform layout in NextLayout.GetComponentInChildren<Transform>())
                {
                    if (layout.GetComponent<ObjectScript>().isHardmode)
                    {
                        layout.gameObject.SetActive(true);
                    }
                }

            }
        }
    }
}



// I Wanted to have multi-dimentional array of gameobjects so that desginers can just drag and drop
// layouts to create new levels entirely by theirself, so thank you Rabwin
// https://discussions.unity.com/t/how-to-declare-a-multidimensional-array-of-string-in-c/21138

[System.Serializable]
class LevelSettings
{
    [Range(5, 50)] public int GameplayTime;
    [Range(0, 10)] public int CreateCompletion;
    [Range(0, 5)] public int ExtraCreates;

    [Range(0, 4)] public float LevelSpeed;

    public float LayoutSeperation;

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