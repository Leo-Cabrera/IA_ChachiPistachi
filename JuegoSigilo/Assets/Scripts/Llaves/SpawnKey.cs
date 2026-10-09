using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnKey : MonoBehaviour
{
    public List<Key> keysSpots;
    private int availableKeys = 0;
    void Start()
    {
        foreach (Key key in keysSpots)
        {
            if (key != null && key.active)
            {
                availableKeys++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log("S presionada");
            SpawnRandomKey();
        }
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("N presionada");
            DeSpawnKey();
        }
    }
    void SpawnRandomKey()
    {

        if (availableKeys >= keysSpots.Count) return;


        List<Key> inactiveKeys = new List<Key>();

        foreach (Key key in keysSpots)
        {
            if (key != null && !key.active)
            {
                inactiveKeys.Add(key);
            }
        }

        int indice = Random.Range(0, inactiveKeys.Count);

        inactiveKeys[indice].EnableKey();

        availableKeys++;

    }
    void DeSpawnKey()
    {
        if (availableKeys <= 0) return;


        List<Key> activeKeys = new List<Key>();

        // Buscar llaves activas
        foreach (Key key in keysSpots)
        {
            if (key != null && key.active)
            {
                activeKeys.Add(key);
            }
        }
        int indice = Random.Range(0, activeKeys.Count);

        activeKeys[indice].DisableKey();

        availableKeys--;

    }


}
