using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    public GameObject key;
    public Collider pickZone;
    public bool active = true;
    void Start()
    {
        if (active) EnableKey();
        else DisableKey();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableKey()
    {
        if (key != null)
        {
            key.SetActive(true);
            active = true;

        }

    }
    public void DisableKey()
    {
        if (key != null)
        {
            key.SetActive(false);
            active = false;

        }

    }
}

