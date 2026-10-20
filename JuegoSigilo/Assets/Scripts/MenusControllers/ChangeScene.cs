using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 0.1f;
    public MenusController MenusController;

    IEnumerator FadeAndLoad(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
    public void LoadGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayMusic("GameMusic");

        StartCoroutine(FadeAndLoad("Casa"));
    }

    public void ChangeMenu()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        Debug.Log("He llegado a fade!!!!!!");
        transition.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        transition.SetTrigger("End");

        MenusController.HideMenus();
        MenusController.ShowMenu();
    }
}
