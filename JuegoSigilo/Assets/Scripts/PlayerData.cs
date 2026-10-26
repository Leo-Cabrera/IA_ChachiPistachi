using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public GameObject player;
    public bool hiding = false;
    public bool onBox = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool RecuperarJugador()
    {
        if (player != null)
            return true;

        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("PlayerData: no se encuentra ningún jugador con tag Player.");
            return false;
        }

        return true;
    }
   


    public void Hide()
    {
        if (!RecuperarJugador())
            return;
        hiding = true;
        player.SetActive(false);
    }

    public void StopHiding()
    {
        if (!RecuperarJugador())
            return;

        hiding = false;
        player.SetActive(true);
    }

    public void OnBox()
    {
        onBox = true;
        player.SetActive(false);
    }

    public void DownBox()
    {
        onBox = false;
        player.SetActive(true);
    }

    public void Reset()
    {
        hiding = false;
        onBox = false;
        player.SetActive(true);
    }

}
