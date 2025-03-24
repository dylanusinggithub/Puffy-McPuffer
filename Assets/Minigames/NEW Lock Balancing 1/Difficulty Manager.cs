using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] GameplaySettings[] Levels;
    int levelIndex;

    WaterController WC;
    NEWLockBalancing LB;
    ObjectDropper OD;

    [SerializeField] GameObject SinglePlay;

    private void Awake()
    {
        levelIndex = PlayerPrefs.GetInt("difficulty", 0);

        WC = GetComponent<WaterController>();
        LB = GetComponent<NEWLockBalancing>();
        OD = GetComponent<ObjectDropper>();

        WC.strengthX = Levels[levelIndex].strength;
        WC.perlinStepSizeX = Levels[levelIndex].perlinStepSize;

        LB.LockSize = Levels[levelIndex].LockSize;
        LB.createCompletion = Levels[levelIndex].CreateCompletion;
        GameObject.Find("CreateText").GetComponent<TextMeshProUGUI>().text = "0 / " + Levels[levelIndex].CreateCompletion;

        OD.burstDelay = Levels[levelIndex].burstDelay;
        OD.burstSeparationDelay = Levels[levelIndex].burstSeparationDelay;
        OD.burstMaxLimit = Levels[levelIndex].burstMaxLimit;
        OD.burstMin = Levels[levelIndex].burstMin;

        OD.Layouts.Clear();
        foreach (GameObject layout in Levels[levelIndex].Layouts) OD.Layouts.Add(layout);

        if (LevelDesigner.SinglePlay) SinglePlay.SetActive(true);
    }
}

[System.Serializable]
class GameplaySettings
{

    [Tooltip("This is how big the lock will be")]
    [Range(1, 2)] public float LockSize = 1;

    [Tooltip("Neccessary amount of creates needed to complete")]
    [Range(0, 10)] public int CreateCompletion;

    [Header("Item Dropping Settings")]
    [Tooltip("Delay between bursts")]
    [Range(0, 10)] public float burstDelay = 5;

    [Tooltip("The time between objects dropping")]
    [Range(0, 10)] public float burstSeparationDelay = 1;

    [Tooltip("Maximum of times it can spawn objects")]
    [Range(1, 8)] public int burstMaxLimit = 5;

    [Tooltip("Minimum of times it can spawn objects")]
    [Range(1, 3)] public int burstMin = 2;

    [Tooltip("The layouts to be dropped during gameplay")]
    public GameObject[] Layouts;


    [Header("Water Setting", order = 1)]
    [Tooltip("How quickly the water will change horizontally")]
    [Range(0f, 5f)] public float perlinStepSize = 1;

    [Tooltip("The water's strength horizontally")]
    [Range(0f, 100f)] public float strength = 6;

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
