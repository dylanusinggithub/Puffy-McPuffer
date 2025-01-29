using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDesigner : MonoBehaviour
{
    [SerializeField] int levelIndex = 0;
    [SerializeField] public LevelClass[] Levels;

    public void StartLevel1()
    {
        // Sets the diffculty level to the specific level's diffculty
        PlayerPrefs.SetInt("Diffculty", Levels[0].Sequence[levelIndex].diffculty);

        // Loads specific level given
        levelIndex++;
        switch (Levels[0].Sequence[levelIndex - 1].minigames)
        {
            case MinigameSettings.Minigames.LockBalancing:
                SceneManager.LoadScene("New Lock Balancing");
                break;

            case MinigameSettings.Minigames.CanalCrusier:
                SceneManager.LoadScene("Canal Crusier");
                break;

            case MinigameSettings.Minigames.PipePuzzle:
                SceneManager.LoadScene("Pipelike");
                break;
        }
    }
}

// Same as Canal Cruiser's Level Generator thingy, the original link is in there
[System.Serializable]
public class LevelClass
{
    public GameObject StartScreen;

    public MinigameSettings[] Sequence;

    public GameObject EndScreen;
}

[System.Serializable]
public class MinigameSettings
{
    public enum Minigames { LockBalancing, CanalCrusier, PipePuzzle }
    [SerializeField] public Minigames minigames;

    [Range(1, 10)] public int diffculty;
}