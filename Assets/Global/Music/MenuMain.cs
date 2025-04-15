using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMain : MonoBehaviour
{
    // Start is called before the first frame update
    
   

    void Start()
    {
        FindObjectOfType<AudioManager2>().StopPlaying("Level");
        FindObjectOfType<AudioManager2>().StopPlaying("Puffy");
        FindObjectOfType<AudioManager>().Play("Intro");
    }

}
