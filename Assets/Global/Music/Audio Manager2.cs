using UnityEngine.Audio;
using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class AudioManager2 : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager2 Instance;
    AudioManager au;
    void Awake()
    {
        /*GameObject obj = GameObject.Find("Audio Manager");

        if (obj != null)
        {
            au = obj.GetComponent<AudioManager>();

            if (au != null)
            {
                Destroy(au);
                Debug.Log("Opp 1 destroyed");
            }
        } */

        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }
    }

    void Start()
    {
        Play("Puffy");
        
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;

        s.source.Stop();
    }
}

//FindObjectOfType<AudioManager>().Play("Intro");