
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioToPlay;

    private void Awake()
    {
        ApplySFXVolume();
    }

    private void ApplySFXVolume()
    {
        if (audioToPlay != null)
        {
            audioToPlay.volume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioToPlay != null && !audioToPlay.isPlaying)
            {
                ApplySFXVolume();

                audioToPlay.pitch = Random.Range(1.0f, 1.3f);
                audioToPlay.Play();
            }
        }
    }
}
