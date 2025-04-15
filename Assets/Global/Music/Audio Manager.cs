using UnityEngine.Audio;
using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

    LevelGenerator SS;

    PipeLayout PL;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GameObject gm = GameObject.Find("Level");
        if (gm != null)
        {
            SS = gm.GetComponent<LevelGenerator>();
            if (SS == null)
                Debug.LogWarning("ScoreScript not found on Game Manager!");
        }
        else
        {
            Debug.LogWarning("Game Manager object not found!");
        }

        GameObject l = GameObject.Find("Layouts");
        if (l != null)
        {
            PL = l.GetComponent<PipeLayout>();
            if (PL == null)
                Debug.LogWarning("PipeLayout not found on Layouts!");
        }
        else
        {
            Debug.LogWarning("Layouts object not found!");
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


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        GameObject gm = GameObject.Find("Level");
        if (gm != null)
        {
            SS = gm.GetComponent<LevelGenerator>();
            if (SS == null) Debug.LogWarning("ScoreScript missing!");
        }
        else
        {
            Debug.LogWarning("Game Manager not found!");
        }

        GameObject l = GameObject.Find("Layouts");
        if (l != null)
        {
            PL = l.GetComponent<PipeLayout>();
            if (PL == null) Debug.LogWarning("PipeLayout missing!");
        }
        else
        {
            Debug.LogWarning("Layouts not found!");
        }

       
        if (scene.name == "Menu")
        {
            Play("Intro");
        }
        else if (scene.name == "NEW Lock Balancing")
        {
            StopPlaying("Intro");
            Play("Puff");
        }
        else if (scene.name == "Pipe Mania")
        {
            StopPlaying("Intro");

            if (PL != null && PL.GauntletMode)
            {
                StopPlaying("Puff");
                Play("Storm");
            }
            else
            {
                Play("Puff");
            }
        }
        else if (scene.name == "Canal Cruiser")
        {
            StopPlaying("Intro");

            if (SS != null && SS.GauntletMode)
            {
                StopPlaying("Puff");
                Play("Storm");
            }
            else
            {
                Play("Puff");
            }
        }
        else if (scene.name == "Level Select Map")
        {
            StopPlaying("Puff");
        }
    }


    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (s.source.isPlaying) return; 

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
