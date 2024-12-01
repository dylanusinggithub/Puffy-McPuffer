using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameObject youlose;
    public GameObject score;
    //Text scoretext;
    public GameObject youwin;
    public Image timer_image;
    float time_remaining;
    public float maxtime = 20f;
    GameManager gameManager;
    //PipeScript pipeScript;
    public GameObject puffything;
    public GameObject puffything2;
    public GameObject PuffyPic;


    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        time_remaining = maxtime;
        //scoretext = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gamewon == true)
        {
            youwin.SetActive(true);
            score.SetActive(true);
            puffything.SetActive(true);
            PuffyPic.GetComponent<Animation>().enabled = false;
            //I'll add a counter instead assuming we're making it warioware style so just ++ points here
            //scoretext.text=time_remaining.ToString("Your score is " + time_remaining);
        }
        else if (time_remaining > 0)
        {
            time_remaining -= Time.deltaTime;
            timer_image.fillAmount = time_remaining / maxtime;
        }
        else
        {
            youlose.SetActive(true);
            puffything2.SetActive(true);
            PuffyPic.GetComponent<Animation>().enabled = false;
            //pipeScript.enabled = false;
        }
    }
}
