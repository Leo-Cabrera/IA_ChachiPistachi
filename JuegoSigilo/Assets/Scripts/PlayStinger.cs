using UnityEngine;

public class PlayStinger : MonoBehaviour
{
    public AudioSource stinger;
    public bool hasPlayed = false;

    void LateUpdate() {
        if (hasPlayed) {
            return;
        }
        MeshRenderer meshRender = GetComponent<MeshRenderer>();

        if (meshRender.enabled == true) {
            if (!stinger.isPlaying) {
                stinger.Play();
                hasPlayed = true;
            }
        }
    }
}
