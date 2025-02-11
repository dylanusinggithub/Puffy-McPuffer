using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
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
    public GameObject layer;
    bool played = true;

    GameObject pipes;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        time_remaining = maxtime;
        //scoretext = GetComponent<Text>();
        pipes = GameObject.FindWithTag("Pipe"); //grab pipes when home from another script wherever initialized // start here dylan !
    }



    // Update is called once per frame
    void Update()
    {
        if (gameManager.gamewon == true)
        {
            
            //Time.timeScale = 0f;
            youwin.SetActive(true);
            score.SetActive(true);
            puffything.SetActive(true);
            PuffyPic.GetComponent<Animation>().enabled = false;
            layer.SetActive(true);
            //I'll add a counter instead assuming we're making it warioware style so just ++ points here
            //scoretext.text=time_remaining.ToString("Your score is " + time_remaining);

            //pipes.GetComponent<PipeScript>().enabled = false;
        }
        else if (time_remaining > 0)
        {
            time_remaining -= Time.deltaTime;
            timer_image.fillAmount = time_remaining / maxtime;
        }
        else if (time_remaining < 0 && played )
        {
            
            //Time.timeScale = 0f;
            FindObjectOfType<AudioManager>().Play("Sad");
            //Debug.Log("WHY THE SOUNg");
            youlose.SetActive(true);
            puffything2.SetActive(true);
            PuffyPic.GetComponent<Animation>().enabled = false;
            layer.SetActive(true);
            played = false;
            //pipeScript.enabled = false;
            //pipes.GetComponent<PipeScript>().enabled = false;
        }
    }
}
