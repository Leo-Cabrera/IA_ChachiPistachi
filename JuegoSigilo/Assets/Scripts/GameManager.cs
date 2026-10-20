using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CanvasGroup gameOverPanel;
    public CanvasGroup gameOverImage;
    public CanvasGroup gameWinImage;

    public float fadeDuration = 0.5f;

    [SerializeField] private GameObject winPanel;

    //public float fadeDuration = 0.5f;

    private bool gameOverActive = false;

    
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.alpha = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        gameOverImage.alpha = 0f;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;

    }
    public void GameOver()
    {
        Debug.Log("GameManager: se ha llamado a GameOver");

        if (gameOverActive) return;

        gameOverActive = true;

        gameOverPanel.alpha = 1f;
        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;

        if (gameOverImage != null)
        {
            gameOverImage.alpha = 1f;
        }

        Time.timeScale = 0f;
    }

    public void GameVictory()
    {
        if (gameOverActive) return;

        gameOverActive = true;

        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    //private IEnumerator FadeIn(CanvasGroup image, string music)
    //{
    //    if (AudioManager.instance != null) AudioManager.instance.PlaySFX(music);

    //    float elapsedTime = 0f;
    //    gameOverPanel.interactable = true;
    //    gameOverPanel.blocksRaycasts = true;

    //    while (elapsedTime < fadeDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float opacity = Mathf.Clamp01(elapsedTime / fadeDuration);
    //        gameOverPanel.alpha = opacity;
    //        image.alpha = opacity;
    //        yield return null;
    //    }

    //    gameOverPanel.alpha = 1f;
    //    image.alpha = 1f;
    //}
    //public void PlayAgain()
    //{
    //    if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
    //    Time.timeScale = 1.0f;
    //    SceneManager.LoadScene("MainGame");
    //    if (AudioManager.instance != null) AudioManager.instance.PlayMusic("GameMusic");

    //}

    public void GoMenu()
    {
        SceneManager.LoadScene("MainMenu");
        //if (AudioManager.instance != null) AudioManager.instance.PlayMusic("TitleMusic");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
