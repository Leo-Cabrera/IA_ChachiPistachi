using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnKey : MonoBehaviour
{
    public List<Key> keysSpots; //la lista de llaves que tenemos en el mapa (activas o inactivas)

    private void Start()
    {
        List<Key> disponibles = new List<Key>();  //para guarda las posiciones que tengamos en el mapa de las que podra elegir

        foreach (Key llave in keysSpots)  //recorreo 
        {
            if (llave == null)  //para evitar que haya alguna referencia vacia 
                continue;

            llave.DisableKey();  //desactiva la llave para que no aparezca su modelo 
            disponibles.Add(llave);  //gaurda ese hueco en la lista como que está disponible
        }

        for (int i = 0; i < 3 && disponibles.Count > 0; i++)  //hacemos un for que vaya 3 veces para activar 3 llaves
        {
            int indice = Random.Range(0, disponibles.Count); //elegimos un numero al azar de la cantidad que tengamos disponibles
            disponibles[indice].EnableKey();  //el numero que haya tocado sera el hueco que activaremos la llave
            disponibles.RemoveAt(indice);  //la quitamos de la lista temporal que hemos usado para que ya no cuente como disponible
        }
    }

    void Update()
    {

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


}
