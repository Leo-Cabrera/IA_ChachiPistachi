using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    public GameObject camaraCenital;
    public GameObject camaraInclinada;

    private Vector2 camaraInput;

    // Update is called once per frame
    void LateUpdate()
    {
        camaraInput = Vector2.zero;

        if (Keyboard.current.cKey.wasReleasedThisFrame) {
            if (camaraCenital.activeSelf == true) {
                camaraCenital.SetActive(false);
                camaraInclinada.SetActive(true);
            }
            else {
                camaraCenital.SetActive(true);
                camaraInclinada.SetActive(false);
            }
        }
        
    }
}
