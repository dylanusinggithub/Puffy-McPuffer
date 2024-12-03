using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    
    float time_remaining;
    public float maxtime = 20f;
    GameManager gameManager;
    


    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        time_remaining = maxtime;
        
    }



    // Update is called once per frame
    void Update()
    {
        if (time_remaining > 0 )
        {
            time_remaining -= Time.deltaTime;
            
        }
        else if (time_remaining < 0 & gameManager.gamewon == false)
        {
            FindObjectOfType<AudioManager>().Play("Sad");
            Debug.Log("WHY THE SOUNT");
            
        }
    }
}
