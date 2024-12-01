using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play (string nameOfSound)
    {
        Sound s = Array.Find(sounds, sound => sound.nameOfSound == nameOfSound);
        if (s == null)
        {
            Debug.LogWarning("SOUNT NOT HERE: " +  nameOfSound);
            return;
        }
            
        s.source.Play();
    }
}
