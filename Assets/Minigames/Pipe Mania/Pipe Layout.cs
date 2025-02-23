using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] GameObject GameOverScreen, WinScren;
    Slider Timer;

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

        Timer = GameObject.Find("Timer").GetComponent<Slider>();
        Timer.maxValue = Levels[LevelIndex].Timer;
    }

    private void Update()
    {
        if (Timer.value < Timer.maxValue)
        {
            Timer.value += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 0;
            GameOverScreen.SetActive(true);
        }

    }

    public void CheckPipes()
    {
        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++)
        {
            if (!transform.GetChild(0).GetChild(i).GetComponent<PipeController>().solved) return;
        }

        Time.timeScale = 0;
        WinScren.SetActive(true);
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
