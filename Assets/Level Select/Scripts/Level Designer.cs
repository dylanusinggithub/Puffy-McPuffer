using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            minigameIndex = -Levels[levelIndex].StartScreen.Length;
            PlayerPrefs.SetInt("minigameIndex", minigameIndex);

            PressMeToSaveLevelIndex = false;
        }
    }

    void Start()
    {
        levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        minigameIndex = PlayerPrefs.GetInt("minigameIndex", -Levels[levelIndex].StartScreen.Length);
    }

    public void StartLevel()
    {
        if (minigameIndex < 0) // Displays the Starting comic before entering the level
        {
            CreateComicBackground(Levels[levelIndex].StartScreen[minigameIndex + Levels[levelIndex].StartScreen.Length]);
            minigameIndex++;
            return;
        }
        else if (minigameIndex >= Levels[levelIndex].Sequence.Length)
        {
            // Go through each endscren untill you've seen them all, then reset the minigameIndex
            // to the number of startscreen the next level has 
            if (minigameIndex < Levels[levelIndex].Sequence.Length + Levels[levelIndex].EndScreen.Length)
            {
                CreateComicBackground(Levels[levelIndex].EndScreen[minigameIndex - (Levels[levelIndex].Sequence.Length)]);
                minigameIndex++;
                return;
            }

            levelIndex++;
            if (levelIndex == Levels.Length)
            {
                CreateComicBackground(ErrorComic);
                Debug.LogWarning("You have reached the last level");
            }
            else minigameIndex = -Levels[levelIndex].StartScreen.Length;

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
    void CreateComicBackground(Sprite image)
    {
        // Removes previous Comic
        for (int i = 0; i < this.transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);

        GameObject Comic = new GameObject("Start Comic");
        Comic.transform.SetParent(this.transform);

        // Make image centred and fill screen
        Comic.AddComponent<RectTransform>();
        Comic.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        Comic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Comic.GetComponent<RectTransform>().anchorMax = Vector2.one;
        Comic.GetComponent<RectTransform>().anchorMin = Vector2.zero;
  

        Comic.AddComponent<Image>();
        Comic.GetComponent<Image>().sprite = image;

        // Makes comic advance whenever clicked
        Comic.AddComponent<Button>();
        Comic.GetComponent<Button>().onClick.AddListener(StartLevel);
        Comic.GetComponent<Button>().transition = Selectable.Transition.None;
    }
}



// Same as Canal Cruiser's Level Generator thingy, the original link is in there
[System.Serializable]
class LevelClass
{
    public Sprite[] StartScreen;

    public MinigameSettings[] Sequence;

    public Sprite[] EndScreen;
}

[System.Serializable]
class MinigameSettings
{
    public enum Minigames { LockBalancing, CanalCrusier, PipePuzzle }
    [SerializeField] public Minigames minigames;

    [Range(1, 10)] public int difficulty;
    [SerializeField] public bool showTutorial;
}