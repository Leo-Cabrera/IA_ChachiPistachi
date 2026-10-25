using UnityEngine;

public class YaikoController : MonoBehaviour
{
    public ArmorController gigante;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Gigante"))
        {
              gigante.EnterYaikoZone();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gigante"))
        {
            gigante.ExitYaikoZone();
        }
    }
}
