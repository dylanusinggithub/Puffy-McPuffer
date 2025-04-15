using UnityEngine.Audio;
using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

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
        if (scene.name == "Menu")
        {
            Play("Puff");
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
            Play("Puff");
        }
        else if (scene.name == "Canal Cruiser")
        {
            StopPlaying("Intro");
            Play("Puff");
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

    
    public void Play (string name)
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
