
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    private AudioSource[] ambienceSources;

    private void Awake()
    {
        // Obtener los AudioSource de este GameObject
        ambienceSources = GetComponents<AudioSource>();

        // Recuperar el volumen guardado
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);

        ApplyVolume(volume);
    }

    public void ApplyVolume(float volume)
    {
        if (ambienceSources == null)
        {
            ambienceSources = GetComponents<AudioSource>();
        }

        foreach (AudioSource source in ambienceSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
    }
}
