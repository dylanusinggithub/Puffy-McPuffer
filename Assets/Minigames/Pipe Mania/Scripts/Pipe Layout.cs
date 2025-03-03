using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] GameObject GameOverScreen, WinScren;

    [Header("Pipe Settings")] 
    [SerializeField] GameObject ClickParticle;
    [SerializeField] GameObject BrokenParticle;
    [SerializeField] AudioClip RegularPipe, BrokenPipe;
    Slider Timer;
    RawImage TimerWater;

    [SerializeField] LevelSettings[] Levels;

    void Awake()
    {
        int LevelIndex = PlayerPrefs.GetInt("difficulty", 0);
        Instantiate(Levels[LevelIndex].Layouts[Random.Range(0, Levels[LevelIndex].Layouts.Length)], transform);

        List<GameObject> Pipes = new List<GameObject>();

        // Ignores first 2 (Start & End pipes)
        for (int i = 0; i < transform.GetChild(0).childCount; i++) 
        {
            GameObject Pipe = transform.GetChild(0).GetChild(i).gameObject;

            if (!Pipe.GetComponent<SpriteRenderer>().name.Contains("End") && !Pipe.GetComponent<SpriteRenderer>().name.Contains("Start"))
            {
                Pipe.AddComponent<BoxCollider2D>();

                Pipe.AddComponent<AudioSource>();
                Pipe.GetComponent<PipeController>().RegularPipe = RegularPipe;
                Pipe.GetComponent<PipeController>().BrokePipe = BrokenPipe;

                Pipe.GetComponent<PipeController>().ClickParticle = ClickParticle;
                Pipe.GetComponent<PipeController>().BrokenParticle = BrokenParticle;

                Pipes.Add(transform.GetChild(0).GetChild(i).gameObject);
            }

        }

        // Goes through all pupes and chooses a random one to be broken
        int initalPipeCount = Pipes.Count;
        for (int i = 0; i < Levels[LevelIndex].BrokenPipesCount && i < initalPipeCount; i++)
        {
            int randomIndex = Random.Range(0, Pipes.Count);
            Pipes[randomIndex].GetComponent<PipeController>().broken = 3;
            Pipes.RemoveAt(randomIndex);
        }

        Timer = GameObject.Find("Timer").GetComponent<Slider>();
        Timer.maxValue = Levels[LevelIndex].Timer;
        Timer.value = 0;

        TimerWater = Timer.GetComponentInChildren<RawImage>();
    }

    private void FixedUpdate()
    {
        if (Timer.value < Timer.maxValue)
        {
            Timer.value += Time.deltaTime;
            TimerWater.uvRect = new Rect(TimerWater.uvRect.position + new Vector2(0.1f, 0) * Time.deltaTime, TimerWater.uvRect.size);
        }
        else
        {
            Time.timeScale = 0;
            GameOverScreen.SetActive(true);
            GetComponent<AudioSource>().Stop();
            GameObject.Find("Pause Icon").SetActive(false);
            this.enabled = false;
        }

    }

    public void CheckPipes()
    {
        // Goes through every pipe and if they're all solved then complete the game
        bool Solved = true, Fixed = true;
        foreach(Transform child in transform.GetChild(0).GetComponentInChildren<Transform>())
        {
            if (child.GetComponent<PipeController>() != null)
            {
                if (!child.GetComponent<PipeController>().solved) Solved = false;

                if (child.GetComponent<PipeController>().broken > 0) Fixed = false;
            }
        }

        if(Fixed) GetComponent<AudioSource>().Stop(); // Stops leak sound

        if (!Solved) return;

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
