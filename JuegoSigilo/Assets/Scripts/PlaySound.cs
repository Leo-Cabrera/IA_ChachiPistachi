using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioToPlay;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!audioToPlay.isPlaying) {
                audioToPlay.pitch = Random.Range(1.0f, 1.3f);
                audioToPlay.Play();
            }
        }
    }
}
