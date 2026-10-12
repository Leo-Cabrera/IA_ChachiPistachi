using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnKey : MonoBehaviour
{
    public List<Key> keysSpots; //la lista de llaves que tenemos en el mapa (activas o inactivas)

    /* ya no necesitamos esto porque solo se actualiza al inicio con las 3 llaves que tendremos disponibles
    si luego cogieramos una llave no se actualizaria y seguiria siendo que hay 3 disponibles
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
    }*/

    // Update is called once per frame
    void Update()
    {
        /* ya no se necesita se ha comprobado que funciona
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log("S presionada");
            SpawnRandomKey();
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("N presionada");
            DeSpawnKey();
        }*/
    }
    public void SpawnRandomKey()
    {
        List<Key> inactiveKeys = new List<Key>();  //lista de llaves sin activar

        foreach (Key key in keysSpots)  //recorremos todas las llaves que tenemos se vean o no en el mapa
        {
            if (key != null && !key.active)  //si la llave no está activada (no se ve en el mapa)
            {
                inactiveKeys.Add(key);  //entonces es un spot para que aparezca si es necesario
            }
        }

        if (inactiveKeys.Count == 0) return;  //si no hay llaves inactivas entonces salimos

        int indice = Random.Range(0, inactiveKeys.Count);  //ahora para que aparezca aleatoriamente hacemos el range

        inactiveKeys[indice].EnableKey();  //activaremos la llave que esté inactiva y haya salido aleatoriamente
    }

    /*void DeSpawnKey()
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

    }*/


}
