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
    [SerializeField] int minigameIndex;
    [SerializeField] LevelClass[] Levels;

    [SerializeField] Sprite ErrorComic;


    private void OnValidate()
    {
        if (PressMeToSaveLevelIndex)
        {
            PlayerPrefs.SetInt("levelIndex", levelIndex);
            minigameIndex = -Levels[levelIndex].StartScreen.Count;
            PlayerPrefs.SetInt("minigameIndex", minigameIndex);

            PressMeToSaveLevelIndex = false;
        }
    }

    void Start()
    {
        levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        minigameIndex = PlayerPrefs.GetInt("minigameIndex", -Levels[levelIndex].StartScreen.Count);
    }

    public void StartLevel()
    {
        if (minigameIndex < 0) // Displays the Starting comic before entering the level
        {
            CreateComicBackground(Levels[levelIndex].StartScreen[minigameIndex + Levels[levelIndex].StartScreen.Count]);
            minigameIndex++;
            return;
        }

        // Loads specific level given
        PlayerPrefs.SetInt("difficulty", Levels[levelIndex].Sequence[minigameIndex].difficulty);
        PlayerPrefs.SetString("showTutorial", Levels[levelIndex].Sequence[minigameIndex].showTutorial.ToString()); // theres no SetBool??

        minigameIndex++;
        PlayerPrefs.SetInt("minigameIndex", minigameIndex);

        print("Difficulty: " + Levels[levelIndex].Sequence[minigameIndex - 1].difficulty + ", Sequence: " + ( minigameIndex - 1));

        switch (Levels[levelIndex].Sequence[minigameIndex - 1].minigames) // Minus 1 as it incremenets before entering the minigame
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

    // Creates the background, I originally did gameobjects enabling and disabling but i wanted to make this as easy for desginers as possible
    void CreateComicBackground(object Comic)
    {
        // Removes previous Comic
        for (int i = 0; i < this.transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);

        print(Comic.GetType());

        GameObject GOComic = new GameObject("Start GOComic");
        GOComic.transform.SetParent(this.transform);

        // Checks comic's type and tries to display it accordingly
        if (Comic is Texture2D)
        {
            // Make image centred and fill screen
            GOComic.AddComponent<RectTransform>();
            GOComic.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            GOComic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            GOComic.GetComponent<RectTransform>().anchorMax = Vector2.one;
            GOComic.GetComponent<RectTransform>().anchorMin = Vector2.zero;


            GOComic.AddComponent<Image>();
            GOComic.GetComponent<Image>().sprite = Sprite.Create((Texture2D)Comic, GOComic.GetComponent<Rect>(), Vector2.zero);

            // Makes GOComic advance whenever clicked
            GOComic.AddComponent<Button>();
            GOComic.GetComponent<Button>().onClick.AddListener(StartLevel);
            GOComic.GetComponent<Button>().transition = Selectable.Transition.None;
        }
        else if(Comic.Equals(typeof(VideoClip)))
        {
            GOComic.AddComponent<VideoPlayer>();
            GOComic.GetComponent<VideoPlayer>().clip = (VideoClip)Comic;
            GOComic.GetComponent<VideoPlayer>().isLooping = true;

            // Makes GOComic advance whenever clicked
            GOComic.AddComponent<Button>();
            GOComic.GetComponent<Button>().onClick.AddListener(StartLevel);
            GOComic.GetComponent<Button>().transition = Selectable.Transition.None;
        }

    }
}



// Same as Canal Cruiser's Level Generator thingy, the original link is in there
[System.Serializable]
class LevelClass
{
    public List<Object> StartScreen;
    public MinigameSettings[] Sequence;
}

[System.Serializable]
class MinigameSettings
{
    public enum Minigames { LockBalancing, CanalCrusier, PipePuzzle }
    public Minigames minigames;

    [Range(1, 10)] public int difficulty;
    public bool showTutorial;

    public List<Object> Cutscene;
}