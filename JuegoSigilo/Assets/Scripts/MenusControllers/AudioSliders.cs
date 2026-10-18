using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{
   
    public Slider _musicSlider, _sfxSlider;
    

    private void Awake()
    {
       
        if (AudioManager.instance != null)
        {
            _musicSlider.value = AudioManager.instance.musicSource.volume;
            _sfxSlider.value = AudioManager.instance.sfxSource.volume;
        }
        
    }

    public void MusicVolume()
    {
        if (AudioManager.instance != null) AudioManager.instance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolume()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SFXVolume(_sfxSlider.value);
            AudioManager.instance.StepsVolume(_sfxSlider.value);
        }
        
      
        
    }
  

    
}
