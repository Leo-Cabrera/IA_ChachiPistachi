using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMenu : MonoBehaviour
{
    public CanvasGroup menuPanel;
    void Start()
    {
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;
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
    }

    public void Hide()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
        menuPanel.alpha = 0;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;

    }

}
