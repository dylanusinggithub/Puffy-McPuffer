using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LevelDesigner : MonoBehaviour
{
    [Tooltip("I have to add a button to manually press as otherwise it'd be reset every single time")]
    [SerializeField] bool PressMeToSaveLevelIndex;
    [SerializeField] int levelIndex;

    [SerializeField] RenderTexture VideoCanvas;

    [SerializeField] int minigameIndex;
    int comicIndex = 0;

    [SerializeField] LevelClass[] Levels;

    [SerializeField] Sprite ErrorComic;

    private void OnValidate()
    {
        if (PressMeToSaveLevelIndex)
        {
            PlayerPrefs.SetInt("levelIndex", levelIndex);
            minigameIndex = -1;
            PlayerPrefs.SetInt("minigameIndex", minigameIndex);

            PressMeToSaveLevelIndex = false;
        }
    }

    void Awake()
    {

        levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        minigameIndex = PlayerPrefs.GetInt("minigameIndex", 0);

        if (PlayerPrefs.GetString("advanceToNextLevel", "False") == "True") StartLevel();
    }

    void NextLevel()
    {
        PlayerPrefs.SetString("advanceToNextLevel", "True");
        StartLevel();
    }

    public void StartLevel()
    {
        // Displays the Starting comic before entering the level
        if (minigameIndex == -1)
        {
            if (Levels[levelIndex].StartScreen.Count > comicIndex)
            {
                CreateComicBackground(Levels[levelIndex].StartScreen[comicIndex]);
                comicIndex++;
            }
            else
            {
                comicIndex = 0;
                LoadLevel();
            }
        }
        else if (Levels[levelIndex].Sequence.Length - 1 > minigameIndex)
        {
            // Play Previous Minigame's Victory Cutscenes
            //Checks to see if there is any comics or you've reached the last one
            if (Levels[levelIndex].Sequence[minigameIndex].Cutscene.Count > comicIndex && Levels[levelIndex].Sequence[minigameIndex].Cutscene.Count > 0)
            {
                CreateComicBackground(Levels[levelIndex].Sequence[minigameIndex].Cutscene[comicIndex]);
                comicIndex++;
            }
            else
            {
                comicIndex = 0;
                LoadLevel();
            }
        }
        else
        {
            levelIndex++;
            if (levelIndex == Levels.Length)
            {
                Debug.LogWarning("You have reached the last level!");
                CreateComicBackground(ErrorComic);
            }
            else
            {
                PlayerPrefs.SetInt("minigameIndex", -Levels[levelIndex].StartScreen.Count);
                PlayerPrefs.SetInt("levelIndex", levelIndex);
            }
        }
    }

    void LoadLevel()
    {
        // Loads specific level given
        if (PlayerPrefs.GetString("advanceToNextLevel", "False") == "True")
        {
            minigameIndex++;
            PlayerPrefs.SetString("advanceToNextLevel", "False");
        }

        PlayerPrefs.SetInt("difficulty", Levels[levelIndex].Sequence[minigameIndex].difficulty);
        PlayerPrefs.SetString("showTutorial", Levels[levelIndex].Sequence[minigameIndex].showTutorial.ToString()); // theres no SetBool??
        PlayerPrefs.SetInt("minigameIndex", minigameIndex);

        print("Difficulty: " + Levels[levelIndex].Sequence[minigameIndex].difficulty + ", Sequence: " + minigameIndex);

        switch (Levels[levelIndex].Sequence[minigameIndex].minigames)
        {
            case MinigameSettings.Minigames.LockBalancing:
                SceneManager.LoadScene("New Lock Balancing");
                break;

            case MinigameSettings.Minigames.CanalCrusier:
                SceneManager.LoadScene("Canal Cruiser");
                break;

            case MinigameSettings.Minigames.PipePuzzle:
                SceneManager.LoadScene("Pipelike");
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
            case "Texture2D":
                {
                    // Make image centred and fill screen
                    GOComic.AddComponent<RectTransform>();
                    GOComic.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().anchorMax = Vector2.one;
                    GOComic.GetComponent<RectTransform>().anchorMin = Vector2.zero;


                    GOComic.AddComponent<RawImage>();
                    GOComic.GetComponent<RawImage>().texture = (Texture2D)Comic;

                    // Makes GOComic advance whenever clicked
                    GOComic.AddComponent<Button>();
                    GOComic.GetComponent<Button>().onClick.AddListener(NextLevel);
                    GOComic.GetComponent<Button>().transition = Selectable.Transition.None;
                }
                break;

            case "VideoClip":
                {
                    // Create Video Player
                    GOComic.AddComponent<VideoPlayer>();
                    VideoPlayer VP = GOComic.GetComponent<VideoPlayer>();
                    VP.clip = (VideoClip)Comic;
                    VP.isLooping = true;

                    VP.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("Volume", 1));
                    VP.targetTexture = VideoCanvas;

                    // Create the image the video will be playing to
                    GOComic.AddComponent<RawImage>();
                    GOComic.GetComponent<RawImage>().texture = VideoCanvas;

                    GOComic.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    GOComic.GetComponent<RectTransform>().anchorMax = Vector2.one;
                    GOComic.GetComponent<RectTransform>().anchorMin = Vector2.zero;

                    // Makes GOComic advance whenever clicked
                    GOComic.AddComponent<Button>();
                    GOComic.GetComponent<Button>().onClick.AddListener(NextLevel);
                    GOComic.GetComponent<Button>().transition = Selectable.Transition.None;
                }
                break;

            case "GameObject":
                {
                    Destroy(GOComic);

                    GOComic = Instantiate((GameObject)Comic);
                    GOComic.transform.parent = transform;
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
}


// Same as Canal Cruiser's Level Generator thingy, the original link is in there
[System.Serializable]
class LevelClass
{
    public List<UnityEngine.Object> StartScreen;
    public MinigameSettings[] Sequence;
}

[System.Serializable]
class MinigameSettings
{
    public enum Minigames { LockBalancing, CanalCrusier, PipePuzzle }
    public Minigames minigames;

    [Range(0, 10)] public int difficulty;
    public bool showTutorial;

    public List<UnityEngine.Object> Cutscene;
}