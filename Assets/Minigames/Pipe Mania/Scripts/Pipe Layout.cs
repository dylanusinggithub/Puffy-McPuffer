using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] GameObject GameOverScreen, WinScren;

    [Header("Pipe Settings")] 
    [SerializeField] GameObject ClickParticle;
    [SerializeField] GameObject BrokenPrefab;
    [SerializeField] AudioClip RegularPipe, BrokenPipe;
    Slider Timer;

    [SerializeField] LevelSettings[] Levels;


    void Awake()
    {
        int LevelIndex = PlayerPrefs.GetInt("difficulty", 0);
        Instantiate(Levels[LevelIndex].Layouts[Random.Range(0, Levels[LevelIndex].Layouts.Length)], transform);

        List<GameObject> Pipes = new List<GameObject>();

        // Ignores first 2 (Start & End pipes)
        for (int i = 2; i < transform.GetChild(0).childCount; i++) 
        {
            GameObject Pipe = transform.GetChild(0).GetChild(i).gameObject;
            Pipe.AddComponent<PipeController>();
            Pipe.AddComponent<BoxCollider2D>();

            Pipe.AddComponent<AudioSource>();
            Pipe.GetComponent<PipeController>().RegularPipe = RegularPipe;
            Pipe.GetComponent<PipeController>().BrokePipe = BrokenPipe;

            Pipe.GetComponent<PipeController>().ClickParticle = ClickParticle;

            Pipes.Add(transform.GetChild(0).GetChild(i).gameObject);
        }

        // Goes through all pupes and chooses a random one to be broken
        int initalPipeCount = Pipes.Count;
        for (int i = 0; i < Levels[LevelIndex].BrokenPipesCount && i < initalPipeCount; i++)
        {
            int randomIndex = Random.Range(0, Pipes.Count);
            Pipes[randomIndex].GetComponent<PipeController>().broken = 3;
            Pipes[randomIndex].GetComponent<PipeController>().BrokenPrefab = BrokenPrefab;
            Pipes.RemoveAt(randomIndex);
        }

        Timer = GameObject.Find("Timer").GetComponent<Slider>();
        Timer.maxValue = Levels[LevelIndex].Timer;
    }

    private void FixedUpdate()
    {
        if (Timer.value < Timer.maxValue)
        {
            Timer.value += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 0;
            GameOverScreen.SetActive(true);
            GameObject.Find("Pause Icon").SetActive(false);
            this.enabled = false;
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
        GameObject.Find("Pause Icon").SetActive(false);
        this.enabled = false;
    }

    [System.Serializable]
    class LevelSettings
    {
        [Range(5, 30)] public int Timer;
        [Range(0, 10)] public int BrokenPipesCount;
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
