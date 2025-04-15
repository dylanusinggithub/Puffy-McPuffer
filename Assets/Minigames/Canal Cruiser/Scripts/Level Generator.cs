using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] bool PressMeToSetLevelIndex;
    [SerializeField] int levelIndex;

    [SerializeField] LevelSettings[] Levels;
    List<GameObject> CreatesLocation = new List<GameObject>();

    float LevelWidth = 0;

    [SerializeField] GameObject FinishLine;

    ScoreScript SS;
    GameObject Puffy;

    float startSpeed;
    Material ScrollingBackground;
    bool PlayAnimation, GauntletMode; 

    Slider timeSlider;
    RawImage timerWater;

    [SerializeField] GameObject SinglePlay, LevelBTN;

    [SerializeField] GameObject GauntletText;

    public bool gaunttrue = false;

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
        Physics2D.gravity = new Vector2(-9.81f, 0); // Left (behind player)

        Puffy = GameObject.Find("Moving Thing");
        SS = GameObject.Find("Game Manager").GetComponent<ScoreScript>();
        ScrollingBackground = GameObject.Find("background").GetComponent<SpriteRenderer>().material;
        timerWater = GameObject.Find("Timer").transform.GetChild(0).GetChild(1).GetComponent<RawImage>();

        // sets the level index to minigameIndex which is provided by level desginer in menu screen
        levelIndex = PlayerPrefs.GetInt("difficulty", 0);

        SS.scoreWin = Levels[levelIndex].CreateCompletion;

        timeSlider = GameObject.Find("Timer").GetComponent<Slider>();
        timeSlider.maxValue = Levels[levelIndex].GameplayTime;
        timeSlider.value = Levels[levelIndex].GameplayTime;

        startSpeed = Levels[levelIndex].LevelSpeed;

        // 2 is the speed it is pegged at so if the levelSpeed is 1.75f then it'll play at 75% speed
        GameObject.Find("Water Swiggles").GetComponent<Animator>().SetFloat("Speed", Levels[levelIndex].LevelSpeed / 2); 

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

        GameObject finish = Instantiate(FinishLine);

        float dist = 4.63f * Levels[levelIndex].LevelSpeed;
        dist *= Levels[levelIndex].GameplayTime;

        finish.transform.position = new Vector3(dist, 0);
        GameObject.Find("UnionChaseMain").GetComponent<UnionController>().chaseDistance = dist - 4;

        // Grabs all creates and disables them to later reenable a specific ones
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                Transform OBJ = transform.GetChild(i).GetChild(j);
                if (OBJ.name.Contains("CratePrefab"))
                {
                    if(!OBJ.GetComponent<ObjectScript>().isHardmode) CreatesLocation.Add(OBJ.gameObject);
                    OBJ.gameObject.SetActive(false);
                }
                else if (OBJ.GetComponent<ObjectScript>().isHardmode) OBJ.gameObject.SetActive(false);
            }
        }

        if (!Levels[levelIndex].GauntletMode)
        {
            if (CreatesLocation.Count < Levels[levelIndex].CreateCompletion)
                Debug.LogError("Level is not Possible! Create Completion: " + Levels[levelIndex].CreateCompletion + " But only " + CreatesLocation.Count + " Valid Spots");
            else if (CreatesLocation.Count < Levels[levelIndex].CreateCompletion + Levels[levelIndex].ExtraCreates)
                Debug.Log("Level does not have enough space of extra crates! " + (CreatesLocation.Count - Levels[levelIndex].CreateCompletion) + "/" + Levels[levelIndex].ExtraCreates + " spawned");
        }

        // Goes through all create locations and chooses a random one to reenable "Spawn"
        int initalCreateCount = CreatesLocation.Count;
        for (int i = 0; i < Levels[levelIndex].CreateCompletion + Levels[levelIndex].ExtraCreates && i < initalCreateCount; i++)
        {
            int randomIndex = Random.Range(0, CreatesLocation.Count);
            CreatesLocation[randomIndex].SetActive(true);
            CreatesLocation.RemoveAt(randomIndex);
        }

        StartCoroutine(PlayTutorial());

        GauntletMode = Levels[levelIndex].GauntletMode;
        if (GauntletMode)
        {
            SS.score = Levels[levelIndex].CreateCompletion + Levels[levelIndex].ExtraCreates;
            SS.Gauntlet = GauntletMode;
            gaunttrue = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                for (int j = 0; j < transform.GetChild(i).childCount; j++)
                {
                    Transform OBJ = transform.GetChild(i).GetChild(j);
                    if (OBJ.name.ToUpper().Contains("CRATE")) OBJ.gameObject.SetActive(false);
                    else if (OBJ.GetComponent<ObjectScript>().isHardmode) OBJ.gameObject.SetActive(true);
                }
            }
        }

        StartCoroutine(GauntletAppear());

        if (LevelDesigner.SinglePlay)
        {
            SinglePlay.SetActive(true);
            LevelBTN.GetComponent<RectTransform>().anchoredPosition = new Vector2(318, -377);
        }
    }

    IEnumerator GauntletAppear()
    {
        Time.timeScale = 0;
        GauntletText.SetActive(true);

        // Positions Cargo Correctly
        Transform Anchor = GameObject.Find("Cargo Objects").transform;
        foreach (Transform Cargo in Anchor.GetComponentsInChildren<Transform>())
            Cargo.localPosition = new Vector3(-20, 0);

        yield return new WaitForSecondsRealtime(GauntletText.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Time.timeScale = 1;
        GauntletText.SetActive(false);
    }

    IEnumerator PlayTutorial()
    {
        PlayAnimation = true;

        GameObject.Find("UnionChaseMain").GetComponent<UnionController>().enabled = false;
        GameObject Timer = GameObject.Find("Timer Holder");
        Timer.SetActive(false);

        float Delay;
        if (PlayerPrefs.GetString("showTutorial", "False") == "True")
        {
            Delay = Puffy.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
            Puffy.GetComponent<Animator>().Play("Crusier Tutorial", 0, 0);
        }
        else
        {
            GameObject.Find("Tutorial Assets").SetActive(false);
            GameObject.Find("Tutorial Text").SetActive(false);
            Delay = Puffy.GetComponent<Animator>().runtimeAnimatorController.animationClips[1].length;
            Puffy.GetComponent<Animator>().Play("Crusier Opening", 0, 0);
        }

        yield return new WaitForSeconds(Delay);
        Puffy.GetComponent<Animator>().enabled = false;


        PlayAnimation = false;

        GameObject.Find("Player").GetComponent<PlayerScript>().enabled = true;
        GameObject.Find("Water Swiggles").GetComponent<Animator>().enabled = true;
        GameObject.Find("UnionChaseMain").GetComponent<UnionController>().enabled = true;

        GameObject.Find("Cinematic Bars").GetComponent<Animation>().Play();

        yield return new WaitForSeconds(0.5f);

        Timer.SetActive(true);
    }

    float HardmodeCheckTime = 1;
    void FixedUpdate()
    {
        if (!PlayAnimation)
        {
            if (timeSlider.value > 0)
            {
                timeSlider.value -= Time.deltaTime;
                timerWater.uvRect = new Rect(timerWater.uvRect.position + new Vector2(0.1f, 0) * Time.deltaTime, timerWater.uvRect.size);
            }
            else GameObject.Find("Game Manager").GetComponent<ScoreScript>().Die();

            Puffy.transform.Translate(new Vector3(startSpeed / 10, 0, 0));
            ScrollingBackground.mainTextureOffset += new Vector2(startSpeed / 310, 0);

            if (!GauntletMode)
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

    public bool GauntletMode;

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