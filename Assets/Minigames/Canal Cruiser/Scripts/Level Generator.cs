using System;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] MultidimensionalGameObject[] Levels;
    int levelIndex;

    [SerializeField, Range(-5, 50)] float LayoutSeperation;
    float LevelWidth = 0;

    private void Start()
    {
        levelIndex = PlayerPrefs.GetInt("LevelIndex", 0);

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
    }
}


// I Wanted to have multi-dimentional array of gameobjects so that desginers can just drag and drop
// layouts to create new levels entirely by theirself, so thank you Rabwin
// https://discussions.unity.com/t/how-to-declare-a-multidimensional-array-of-string-in-c/21138

[System.Serializable]
public class MultidimensionalGameObject
{
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