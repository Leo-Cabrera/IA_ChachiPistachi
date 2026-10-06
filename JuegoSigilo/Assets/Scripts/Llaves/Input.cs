using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.LogError("Keyboard.current es NULL");
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("¡¡¡ SPACE DETECTADO !!!");
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log("¡¡¡ S DETECTADA !!!");
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("¡¡¡ N DETECTADA !!!");
        }
    }
}
