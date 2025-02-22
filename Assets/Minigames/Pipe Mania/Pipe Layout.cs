using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] LevelSettings[] Levels;

    void Awake()
    {
        int LevelIndex = PlayerPrefs.GetInt("difficulty", 0);
        Instantiate(Levels[LevelIndex].Layouts[Random.Range(0, Levels[LevelIndex].Layouts.Length)], transform);

        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++) 
        {
            transform.GetChild(0).GetChild(i).gameObject.AddComponent<PipeController>();
            transform.GetChild(0).GetChild(i).gameObject.AddComponent<BoxCollider2D>();
        }
    }

    public void CheckPipes()
    {
        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++)
        {
            if (!transform.GetChild(0).GetChild(i).GetComponent<PipeController>().solved) return;
        }

        print("Yipee");
    }

    [System.Serializable]
    class LevelSettings
    {
        [Range(5, 30)] public int Timer;
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
            get { return Layouts.Length; }
        }
    }
}
