using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static PipeEditor;

public class PipeLayout : MonoBehaviour
{
    [SerializeField] GameObject GameOverScreen, WinScren;
    [SerializeField] GameObject SinglePlay;

    [SerializeField] GameObject GauntletText;
    public bool GauntletMode = false;

    [Header("Pipe Settings")] 
    [SerializeField] GameObject ClickParticle;
    [SerializeField] GameObject BrokenParticle;
    [SerializeField] GameObject SteamyParticle;
    [SerializeField] AudioClip RegularPipe, BrokenPipe;
    Slider Timer;
    RawImage TimerWater;

    int LevelIndex;

    [Header("Level Settings")]
    [SerializeField] int ManualIndex;
    [SerializeField] bool PressMeToSetLevel = false;

    private void OnValidate()
    {
        if (PressMeToSetLevel)
        {
            PressMeToSetLevel = false;
            PlayerPrefs.SetInt("difficulty", ManualIndex);
        }
    }

    [SerializeField] LevelSettings[] Levels;

    public ParticleSystem drip;

    List<GameObject> Pipes = new List<GameObject>();
    int BrokenPipesCount = 0;

    private Dictionary<Transform, ParticleSystem> activeLeaks = new Dictionary<Transform, ParticleSystem>();
    void Awake()
    {
        LevelIndex = PlayerPrefs.GetInt("difficulty", 0);
        Instantiate(Levels[LevelIndex].Layouts[Random.Range(0, Levels[LevelIndex].Layouts.Length)], transform);
         
        // Sets up all the pipes accordingly
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            GameObject Pipe = transform.GetChild(0).GetChild(i).gameObject;
            if (!Pipe.name.Contains("End") && !Pipe.name.Contains("Start"))
            {
                Pipe.AddComponent<AudioSource>();
                Pipe.AddComponent<PipeController>();

                Pipes.Add(Pipe);
            }
        }

        GauntletMode = Levels[LevelIndex].GauntletMode;
        if (!GauntletMode)
        {
            foreach (GameObject Pipe in Pipes) CreateBrokenPipes(Pipe);

            // Goes through all pipes and chooses a random one to be broken
            int initalPipeCount = Pipes.Count;
            for (int i = 0; i < Levels[LevelIndex].BrokenPipesCount && i < initalPipeCount; i++)
            {
                int randomIndex = Random.Range(0, Pipes.Count);
                Pipes[randomIndex].GetComponent<PipeController>().broken = 3;
                Pipes.RemoveAt(randomIndex);
            }
        }
        else foreach (GameObject Pipe in Pipes)
        {
            Pipe.GetComponent<PipeController>().gaunletMode = true;
            Pipe.GetComponent<PipeController>().enabled = false;

            Pipe.GetComponent<Animator>().enabled = true;
        }

        Timer = GameObject.Find("Timer").GetComponent<Slider>();
        Timer.maxValue = Levels[LevelIndex].Timer;
        Timer.value = 0;

        TimerWater = Timer.GetComponentInChildren<RawImage>();

        StartCoroutine(GauntletAppear());

        if (LevelDesigner.SinglePlay) SinglePlay.SetActive(true);

    }

    IEnumerator GauntletAppear()
    {
        // Disables Pause Button
        GameObject.Find("Pause Menu").transform.GetChild(1).gameObject.SetActive(true);

        Time.timeScale = 0;
        GauntletText.SetActive(true);

        yield return new WaitForSecondsRealtime(GauntletText.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Time.timeScale = 1;
        GauntletText.SetActive(false);
        GameObject.Find("Pause Menu").transform.GetChild(1).gameObject.SetActive(true);
    }

    GameObject CreateBrokenPipes(GameObject Pipe)
    {
        Pipe.AddComponent<BoxCollider2D>();

        Pipe.GetComponent<PipeController>().RegularPipe = RegularPipe;
        Pipe.GetComponent<PipeController>().BrokePipe = BrokenPipe;

        Pipe.GetComponent<PipeController>().ClickParticle = ClickParticle;
        Pipe.GetComponent<PipeController>().BrokenParticle = BrokenParticle;
        Pipe.GetComponent<PipeController>().SteamyParticle = SteamyParticle;

        return Pipe;
    }

    float oldPipeCount = 0;
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
            GameObject.Find("Pause Menu").SetActive(false);
            this.enabled = false;
        }

        if (GauntletMode)
        {
            //\frac{\operatorname{floor}\left(x^{1.7}\cdot p\right)}{p\ } <-- Copy and paste this bad boy into desmos for graph

            float PipeCount = Timer.value / Timer.maxValue;
            PipeCount = Mathf.Floor(Mathf.Pow(PipeCount, 1.7f) * Levels[LevelIndex].BrokenPipesCount) / Levels[LevelIndex].BrokenPipesCount;

            if (PipeCount != oldPipeCount)
            {
                int RND = Random.Range(0, Pipes.Count);

                CreateBrokenPipes(Pipes[RND]);
                Pipes[RND].GetComponent<PipeController>().enabled = true;
                Pipes[RND].GetComponent<PipeController>().broken = 3;

                Pipes.Remove(Pipes[RND]);

                oldPipeCount = PipeCount;
            }
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

        if (Levels[LevelIndex].GauntletMode)
        {
            // Actitvated Twice
            BrokenPipesCount ++;
            if (Levels[LevelIndex].BrokenPipesCount > (BrokenPipesCount / 2) + 2) return;
        }
        else
        {
            if (!Solved || Time.timeScale == 0) return;
        }
        

        Time.timeScale = 0;
        if (Levels[LevelIndex].GauntletMode) GameObject.Find("Pause Menu").GetComponent<MenuButtons>().BTN_NextLevel();
        else WinScren.SetActive(true);

        GameObject.Find("Pause Icon").SetActive(false);

        this.enabled = false;
    }

    public void OrderedLeaks()
    {
        if (Levels[LevelIndex].GauntletMode) return;

        bool Solved = true;
        foreach (Transform child in transform.GetChild(0).GetComponentInChildren<Transform>())
        {
            PipeController pipe = child.GetComponent<PipeController>();
            if (pipe != null)
            {
                
                if (!pipe.solved)
                {
                    Solved = false;
                    if (!activeLeaks.ContainsKey(child)) 
                    {
                        
                        
                        ParticleSystem newParticle = Instantiate(drip, child.position + new Vector3(0.8f, 0f, 0f), Quaternion.identity);
                        newParticle.Play();

                        activeLeaks.Add(child, newParticle);
                    }
                    return; 
                }
               
                else if (pipe.solved && activeLeaks.ContainsKey(child))
                {
                    activeLeaks[child].Stop();

                    child.GetComponent<Animator>().enabled = true;

                    Destroy(activeLeaks[child].gameObject); 
                    activeLeaks.Remove(child);
                }
            }
        }

        if (!Solved) return;
    }

    [System.Serializable]
    class LevelSettings
    {
        [Range(5, 30)] public int Timer;
        [Range(0, 10)] public int BrokenPipesCount;
        public bool GauntletMode;
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
