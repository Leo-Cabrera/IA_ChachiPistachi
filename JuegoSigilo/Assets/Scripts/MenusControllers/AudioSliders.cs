
using UnityEngine;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{
    public Slider _musicSlider;
    public Slider _sfxSlider;

    private void Awake()
    {
        // Recuperar los volúmenes guardados
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);

        _musicSlider.value = musicVolume;
        _sfxSlider.value = sfxVolume;

        // Aplicar los valores al AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.MusicVolume(musicVolume);
            AudioManager.instance.SFXVolume(sfxVolume);
            AudioManager.instance.StepsVolume(sfxVolume);
        }
    }

    public void MusicVolume()
    {
        float volume = _musicSlider.value;

        // Guardar el volumen para futuras escenas
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // Aplicar a la música principal
        if (AudioManager.instance != null)
        {
            AudioManager.instance.MusicVolume(volume);
        }

        // Aplicar a los ambientes si su escena está cargada
        AmbienceManager[] managers =
            FindObjectsOfType<AmbienceManager>();

        foreach (AmbienceManager manager in managers)
        {
            manager.ApplyVolume(volume);
        }
    }

    public void SFXVolume()
    {
        float volume = _sfxSlider.value;

        PlayerPrefs.SetFloat("SFXVolume", volume);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SFXVolume(volume);
            AudioManager.instance.StepsVolume(volume);
        }
    }
}
