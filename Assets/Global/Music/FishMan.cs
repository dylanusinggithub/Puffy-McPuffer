using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMan : MonoBehaviour
{
    public void Start()
    {
        FindObjectOfType<AudioManager>().StopPlaying("Intro");
        FindObjectOfType<AudioManager>().Play("Level");
    }
}
