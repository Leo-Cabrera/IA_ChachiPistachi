
using UnityEngine;

public class PlayStinger : MonoBehaviour
{
    public AudioSource stinger;
    public bool hasPlayed = false;

    private void Awake()
    {
        ApplySFXVolume();
    }

    private void ApplySFXVolume()
    {
        if (stinger != null)
        {
            stinger.volume = PlayerPrefs.GetFloat("SFXVolume", 0.3f);
        }
    }

    private void LateUpdate()
    {
        if (hasPlayed)
        {
            return;
        }

        MeshRenderer meshRender = GetComponent<MeshRenderer>();

        if (meshRender != null && meshRender.enabled)
        {
            if (stinger != null && !stinger.isPlaying)
            {
                ApplySFXVolume();

                stinger.Play();
                hasPlayed = true;
            }
        }
    }
}
