using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public MenusController MenusController;

    IEnumerator FadeAndLoad(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
    public void LoadGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic("GameMusic");
        //StartCoroutine(FadeAndLoad("MainGame"));
    }

    public void ChangeMenu()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        transition.SetTrigger("End");

        MenusController.HideMenus();
        MenusController.ShowMenu();
    }
}
