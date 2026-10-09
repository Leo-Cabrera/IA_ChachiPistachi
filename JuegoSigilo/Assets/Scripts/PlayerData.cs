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
   


    public void Hide()
    {
        hiding = true;
        player.SetActive(false);
    }

    public void StopHiding()
    {
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

}
