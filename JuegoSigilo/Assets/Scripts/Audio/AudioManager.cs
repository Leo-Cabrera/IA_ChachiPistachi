using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, stepsSource;
    public string initialMusic;
    public bool playingSteps = false;
    public float initialVolume = 0.3f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        musicSource.volume = initialVolume;
        sfxSource.volume = initialVolume;
    }

    private void Start()
    {
        
        PlayMusic(initialMusic);
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x=> x.audioName == name);
        if (s != null) { 
            if(name == "Steps")
            {
                playingSteps = true;
                stepsSource.clip = s.clip;
                stepsSource.Play();
            }
            else{
                musicSource.clip = s.clip;
                musicSource.Play();
            }
            
            
        }
    }
    public void StopMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.audioName == name);
        if (s != null)
        {
            if (name == "Steps")
            {
                playingSteps = false;
                stepsSource.clip = s.clip;
                stepsSource.Stop();
            }
            else
            {
                musicSource.clip = s.clip;
                musicSource.Stop();
            }
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.audioName == name);
        if (s != null)
        {
            
            sfxSource.PlayOneShot(s.clip);
        }
    }


    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;    
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    public void StepsVolume(float volume)
    {
        stepsSource.volume = volume;
    }
}
