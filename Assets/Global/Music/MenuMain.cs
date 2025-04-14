using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().StopPlaying("Level");
        FindObjectOfType<AudioManager>().Play("Intro");
    }

}
