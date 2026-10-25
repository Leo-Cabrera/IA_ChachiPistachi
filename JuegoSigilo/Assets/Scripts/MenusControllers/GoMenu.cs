using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMenu : MonoBehaviour
{
    public CanvasGroup menuPanel;
    public CanvasGroup buttonsPanel;

    void Start()
    {
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;

        buttonsPanel.alpha = 1;
        buttonsPanel.blocksRaycasts = true;
        buttonsPanel.interactable = true;
    }

    public void GoMainMenu()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainTitle");
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic("TitleMusic");
        PlayerData.instance.Reset();

    }
    public void Show()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
       
        menuPanel.alpha = 1;
        menuPanel.blocksRaycasts = true;
        menuPanel.interactable = true;

        buttonsPanel.alpha = 0;
        buttonsPanel.blocksRaycasts = false;
        buttonsPanel.interactable = false;
    }

    public void Hide()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;

        buttonsPanel.alpha = 1;
        buttonsPanel.blocksRaycasts = true;
        buttonsPanel.interactable = true;
    }

}
