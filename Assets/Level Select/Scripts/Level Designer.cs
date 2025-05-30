using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDesigner : MonoBehaviour
{
    [SerializeField] UnityEngine.Object[] IntroComic;

    [SerializeField] int levelIndex;
    [SerializeField] int minigameIndex;
    int comicIndex = 0;

    [SerializeField] public LevelClass[] Levels;


    static public LevelDesigner Instance;
    static public bool AdvanceToNextLevel = false;
    static public bool SinglePlay = false;

    static public bool FreePlay;
    bool JustLoaded = true;

    AudioSource BTNAS;
    float BTNVol;

    public void BTN_FreePlay()
    {
        if (JustLoaded) return;

        FreePlay = !FreePlay;
        if (!FreePlay && transform.childCount > 0)
        {
            Animator anim = transform.GetChild(0).GetComponentInChildren<Animator>();

            anim.SetFloat("Speed", -1);
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("Appear", 0, 1); // Replays Animation only after it completes

            anim.transform.GetChild(0).gameObject.SetActive(false);

            // Removes previous LevelPreview
            float clipLength = anim.runtimeAnimatorController.animationClips[0].length;
            float PlaybackPercent = Mathf.Clamp01(anim.GetCurrentAnimatorStateInfo(0).normalizedTime); // Only care if it's midway
            float DestoryTime = clipLength * PlaybackPercent;
            Destroy(transform.GetChild(0).gameObject, DestoryTime);
        }

        GameObject LevelBTNs = GameObject.Find("Canvas");
        foreach (Transform t in LevelBTNs.GetComponentInChildren<Transform>())
        {
            LevelSelector BTN = t.GetComponentInChildren<LevelSelector>();
            if (BTN != null) BTN.EnableOrDisableLevel();
        }
    }

    public void BTN_Exit()
    {
        StartCoroutine(FadeLoadScene("Menu"));
    }
    public void BTN_ComicViewer()
    {
        StartCoroutine(FadeLoadScene("Cinematic Viewer"));
    }

    IEnumerator FadeLoadScene(string Scene)
    {
        BTNAS.volume = BTNVol * PlayerPrefs.GetFloat("Volume", 1);
        BTNAS.Play();

        Animator FadeTransition = GameObject.Find("Fadetransition").GetComponent<Animator>();

        FadeTransition.SetFloat("Speed", 1);
        FadeTransition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;
        SceneManager.LoadScene(Scene);
    }

    void Awake()
    {
        Instance = this; // To Be accessed with Cinimatic Viewer
        
        BTNAS = GetComponent<AudioSource>();
        BTNVol = BTNAS.volume;

        GameObject.Find("CLOUDS").GetComponent<AudioSource>().volume *= PlayerPrefs.GetFloat("Volume", 1);
        GameObject.Find("FreePlay").GetComponent<Toggle>().isOn = FreePlay;

        // Sets Levels Unlocked to 0 if you're playing for the first time
        if (!PlayerPrefs.HasKey("Levels Unlocked") || PlayerPrefs.GetInt("Levels Unlocked") < 0)
        {
            GameObject.Find("Clouds Canvas").GetComponent<Canvas>().sortingOrder = 2;
            GameObject.Find("Menu Buttons").transform.SetParent(GameObject.Find("Canvas").transform);

            PlayerPrefs.SetInt("Levels Unlocked", -1);
            StartLevel(-1);
        }


        levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        minigameIndex = PlayerPrefs.GetInt("minigameIndex", -1);

        if (AdvanceToNextLevel) 
        {
            if (Levels[levelIndex].Sequence.Length - 1 == minigameIndex)
            {
                PlayerPrefs.SetInt("minigameIndex", -1);
                PlayerPrefs.SetString("advanceToNextLevel", "False");

                AdvanceToNextLevel = false;

                print("Level Complete");

                return;
            }
            
            if (!SinglePlay)
            {
                minigameIndex++;
                PlayerPrefs.SetInt("minigameIndex", minigameIndex);
            }

            NextLevel();
        }
        else SinglePlay = false;

        JustLoaded = false;
    }

    void NextLevel()
    {
        // I would advance directly but then i would skip every comic after the 1st
        PlayerPrefs.SetString("advanceToNextLevel", "True"); 
        StartLevel();
    }

    public void StartMinigame(int MinigameIndex, int LevelIndex)
    {
        if (LevelIndex >= Levels.Length) Debug.LogError("Invalid Level Index! This Level Does Not Exist");
        else if (MinigameIndex >= Levels[LevelIndex].Sequence.Length) Debug.LogError("Invalid Minigame Index!");
        else
        {
            PlayerPrefs.SetInt("minigameIndex", MinigameIndex);
            PlayerPrefs.SetInt("levelIndex", LevelIndex);

            minigameIndex = MinigameIndex;
            levelIndex = LevelIndex;

            LoadLevel();
        }
    }

    public void StartLevel(int LevelNumber)
    {
        if (LevelNumber >= Levels.Length) Debug.LogError("Invalid Level Index! This Level Does Not Exist");
        else
        {
            // if you start a new level and resets
            if (levelIndex != LevelNumber) PlayerPrefs.SetInt("minigameIndex", -1);

            GameObject LevelBTNs = GameObject.Find("Canvas");
            foreach (Transform t in LevelBTNs.GetComponentInChildren<Transform>())
            {
                LevelSelector BTN = t.GetComponentInChildren<LevelSelector>();
                if (BTN != null) BTN.GetComponent<Button>().enabled = false;
            }

            GameObject ExtraBTN = GameObject.Find("Menu Buttons");
            
            // Disables Comic Viewer and Exit
            foreach(Button BTN in ExtraBTN.GetComponentsInChildren<Button>()) BTN.interactable = false;

            PlayerPrefs.SetInt("levelIndex", LevelNumber);

            levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
            minigameIndex = PlayerPrefs.GetInt("minigameIndex", -1);

            StartLevel();

        }
    }

    public void StartLevel()
    {
        // Displays the Starting comic before entering the level
        if (minigameIndex == -1)
        {
            if (levelIndex < 0)
            {
                if (IntroComic.Length > comicIndex)
                {
                    CreateComicBackground(IntroComic[comicIndex]);
                    comicIndex++;
                }
                else
                {
                    PlayerPrefs.SetInt("Levels Unlocked", 0);

                    GameObject.Find("Clouds Canvas").GetComponent<Canvas>().sortingOrder = 1;
                    GameObject.Find("Menu Buttons").transform.SetParent(GameObject.Find("CLOUDS").transform);

                    // Renables the animators in case they were disabled before hand with a comics
                    GameObject LevelBTNs = GameObject.Find("Canvas");
                    foreach (Transform t in LevelBTNs.GetComponentInChildren<Transform>())
                    {
                        LevelSelector BTN = t.GetComponentInChildren<LevelSelector>();
                        if (BTN != null) BTN.EnableOrDisableLevel();
                    }

                    // Disables Comic Viewer and Exit
                    GameObject ExtraBTN = GameObject.Find("Menu Buttons");
                    foreach (Button BTN in ExtraBTN.GetComponentsInChildren<Button>()) BTN.interactable = true;

                    GetComponent<Canvas>().sortingOrder = 11;

                    Destroy(transform.GetChild(0).gameObject);
                    comicIndex = 0;
                    return;
                }
            }
            else
            {
                if (Levels[levelIndex].StartScreen.Count > comicIndex)
                {
                    CreateComicBackground(Levels[levelIndex].StartScreen[comicIndex]);
                    comicIndex++;
                }
                else
                {
                    comicIndex = 0;
                    PlayerPrefs.SetString("advanceToNextLevel", "True");
                    LoadLevel();
                }
            }
        }
        else 
        {
            // Play Previous Minigame's Victory Cutscenes
            // Checks to see if there is any comics or you've reached the last one
            if (Levels[levelIndex].Sequence[minigameIndex].Cutscene.Count > comicIndex && Levels[levelIndex].Sequence[minigameIndex].Cutscene.Count > 0)
            {
                CreateComicBackground(Levels[levelIndex].Sequence[minigameIndex].Cutscene[comicIndex]);
                comicIndex++;
            }
            else
            {
                if (Levels[levelIndex].Sequence.Length - 1 == minigameIndex)
                {
                    print("Level complete!");
                    PlayerPrefs.SetInt("minigameIndex", -1);

                    if (PlayerPrefs.GetInt("Levels Unlocked") <= levelIndex) PlayerPrefs.SetInt("Levels Unlocked", levelIndex + 1);

                    PlayerPrefs.SetString("advanceToNextLevel", "False");
                    AdvanceToNextLevel = false;
                    SinglePlay = false;

                    // Destroys last comic
                    for (int i = 0; i < this.transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);

                    // Renables the animators in case they were disabled before hand with a comics
                    GameObject LevelBTNs = GameObject.Find("Canvas");
                    foreach (Transform t in LevelBTNs.GetComponentInChildren<Transform>())
                    {
                        LevelSelector BTN = t.GetComponentInChildren<LevelSelector>();
                        if (BTN != null) BTN.EnableOrDisableLevel();    
                    }

                    GameObject ExtraBTN = GameObject.Find("Menu Buttons");
                    foreach (Button BTN in ExtraBTN.GetComponentsInChildren<Button>()) BTN.interactable = true;
                }
                else
                {
                    comicIndex = 0;
                    LoadLevel();
                }
            }
        }
    }

    void LoadLevel()
    {
        // Loads specific level given
        if (PlayerPrefs.GetString("advanceToNextLevel", "False") == "True")
        {
            // This is saved beforehand to not progress levels once quiting
            PlayerPrefs.SetInt("minigameIndex", minigameIndex);
            minigameIndex++;

            PlayerPrefs.SetString("advanceToNextLevel", "False");
            AdvanceToNextLevel = false;
        }

        PlayerPrefs.SetInt("difficulty", Levels[levelIndex].Sequence[minigameIndex].difficulty);
        PlayerPrefs.SetString("showTutorial", Levels[levelIndex].Sequence[minigameIndex].showTutorial.ToString()); // theres no SetBool??


        print("Level: " + levelIndex + ", Difficulty: " + Levels[levelIndex].Sequence[minigameIndex].difficulty + ", Sequence: " + minigameIndex);

        switch (Levels[levelIndex].Sequence[minigameIndex].minigames)
        {
            case MinigameSettings.Minigames.LockBalancing:
                SceneManager.LoadScene("New Lock Balancing");
                break;
            case MinigameSettings.Minigames.CanalCrusier:
                SceneManager.LoadScene("Canal Cruiser");
                break;
            case MinigameSettings.Minigames.PipeMania:
                SceneManager.LoadScene("Pipe Mania");
                break;
            case MinigameSettings.Minigames.CanalSnap:
                SceneManager.LoadScene("CanalSnap");
                break;
            case MinigameSettings.Minigames.CoalHaul:
                SceneManager.LoadScene("CoalHaul");
                break;
        }
    }

    void CreateComicBackground(object Comic)
    {
        // Removes previous Comic
        for (int i = 0; i < this.transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);

        GameObject GOComic = new GameObject("Start GOComic");
        GOComic.transform.SetParent(this.transform);

        // Finds the Comic's Type by finding the last instance of "." (plus 1 as it actually gives the location of the ".")
        // Basically: UnityEngine.Video.VideoClip > .VideoClip > VideoClip
        String ObjectType = Comic.GetType().ToString();
        ObjectType = ObjectType.Substring(ObjectType.LastIndexOf(".") + 1);


        switch (ObjectType)
        {
            case "Sprite":
            case "Texture2D":
                {
                    // Make image centred and fill screen
                    GOComic.AddComponent<RectTransform>();
                    GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
                    GOComic.GetComponent<RectTransform>().localScale = Vector2.one;

                    // The images keep flip flopping between the 2 and i have no idea why
                    if (ObjectType == "Sprite")
                    {
                        GOComic.AddComponent<Image>();
                        GOComic.GetComponent<Image>().sprite = (Sprite)Comic;
                    }
                    else
                    {
                        GOComic.AddComponent<RawImage>();
                        GOComic.GetComponent<RawImage>().texture = (Texture2D)Comic;
                    }

                    // Makes GOComic advance whenever clicked
                    GOComic.AddComponent<Button>();
                    GOComic.GetComponent<Button>().onClick.AddListener(NextLevel);
                    GOComic.GetComponent<Button>().transition = Selectable.Transition.None;
                }
                break;

            case "VideoClip":
                {
                    Debug.LogError("Video is no longer supported </3");
                }
                break;

            case "GameObject":
                {
                    Destroy(GOComic);

                    GOComic = Instantiate((GameObject)Comic);
                    GOComic.transform.parent = transform;

                    GOComic.AddComponent<PopupController>();
                    StartCoroutine(AssignButtons());

                    if (GOComic.GetComponent<RectTransform>() != null)
                    {
                        GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        GOComic.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
                        GOComic.GetComponent<RectTransform>().localScale = Vector2.one;
                    }
                }
                break;

            default:
                {
                    Destroy(GOComic);
                    Debug.LogError("Invalid Type! " + ObjectType);
                }
                break;
        }
    }

    IEnumerator AssignButtons()
    {
        yield return new WaitForEndOfFrame();

        GameObject.Find("Next").GetComponent<Button>().onClick.AddListener(StartLevel);
        GameObject.Find("Skip").GetComponent<Button>().onClick.AddListener(SkipToLevel);

        if (comicIndex > 1) GameObject.Find("Back").GetComponent<Button>().onClick.AddListener(GoBack);
        else Destroy(GameObject.Find("Back"));
    }

    public void SkipToLevel()
    {
        // If intro
        if (levelIndex < 0)
        {
            comicIndex = 1000; // if we ever make more than 1K comics i'll be impressed
            StartLevel();
        }
        else
        {
            // Displays the very last comic, the how to play
            comicIndex = Levels[levelIndex].StartScreen.Count - 1;
            StartLevel();
        }

    }

    public void GoBack()
    {
        if (comicIndex > 1) 
        {
            comicIndex -= 2; // Gets incremented later
            StartLevel();
        }
    }
}

// Same as Canal Cruiser's Level Generator thingy, the original link is in there
[System.Serializable]
public class LevelClass
{
    public List<UnityEngine.Object> StartScreen;
    public MinigameSettings[] Sequence;
}

[System.Serializable]
public class MinigameSettings
{
    public enum Minigames { LockBalancing, CanalCrusier, PipeMania, CanalSnap, CoalHaul }
    public Minigames minigames;

    [Range(0, 10)] public int difficulty;
    public bool showTutorial;

    public List<UnityEngine.Object> Cutscene;
}