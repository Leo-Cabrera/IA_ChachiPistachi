
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] private int llavesNecesarias = 3;

    private bool victoriaActivada = false;

    private void OnTriggerEnter(Collider other)
    {
        if (victoriaActivada || !other.CompareTag("Player"))
            return;

        PlayerKeys playerKeys = other.GetComponent<PlayerKeys>();

        if (playerKeys == null)
        {
            Debug.LogWarning("No se encuentra PlayerKeys en el jugador.");
            return;
        }

        if (playerKeys.CurrentKeys >= llavesNecesarias)
        {
            victoriaActivada = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.GameVictory();
            }
            else
            {
                Debug.LogError("No se encuentra el GameManager.");
            }
        }
        else
        {
            Debug.Log("Necesitas las 3 llaves para escapar.");
        }
    }
}
