using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowMenu : MonoBehaviour
{
    public CanvasGroup menuPanel;
    public List<GameObject> buttonsBack = new List<GameObject>(); 


    void Start()
    {
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;
    }

    public void Show()
    {
        if(AudioManager.instance != null)AudioManager.instance.PlaySFX("Button");
       
        menuPanel.alpha = 1;
        menuPanel.blocksRaycasts = true;
        menuPanel.interactable = true;

        for (int i = 0; i < buttonsBack.Count; i++)
        {
            buttonsBack[i].SetActive(false);
        }
    }

    public void Hide() {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;

        for (int i = 0; i < buttonsBack.Count; i++)
        {
            buttonsBack[i].SetActive(true);
        }
    }

    public void GoMenu()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainTitle");
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic("TitleMusic");
     
       
        //PlayerData.instance.Reset();
        
    }
}
