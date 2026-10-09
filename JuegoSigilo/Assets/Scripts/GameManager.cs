using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public CanvasGroup gameOverPanel;
    //public CanvasGroup gameOverImage;
    //public CanvasGroup gameVictoryImage;


    //public float fadeDuration = 0.5f;

    //private bool gameOverActive = false;

    
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
        //if (gameOverPanel != null) gameOverPanel.alpha = 0f;
        //gameVictoryImage.alpha = 0f;
        //gameOverImage.alpha = 0f;
        //gameOverPanel.interactable = false;
        //gameOverPanel.blocksRaycasts = false;

    }
    //public void GameOver()
    //{
    //    if (gameOverActive) return;
    //    gameOverActive = true;
    //    StartCoroutine(FadeIn(gameOverImage, "Lose"));
    //}

    //public void GameVictory()
    //{
    //    if (gameOverActive) return;
    //    gameOverActive = true; StartCoroutine(FadeIn(gameVictoryImage, "Win"));

    //}
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
    //public void GoMenu()
    //{
    //    if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Button");
    //    Time.timeScale = 1.0f;
    //    SceneManager.LoadScene("MainTitle");
    //    if (AudioManager.instance != null) AudioManager.instance.PlayMusic("TitleMusic");
    //}
}
