using System.Collections.Generic;
using UnityEngine;

public class MenusController : MonoBehaviour
{
    // Canvas variables
    public List<CanvasGroup> menus;
    public CanvasGroup titleDecor;

    public int menuNum = 0;
    // Movement variables
    public float movementDuration = 2f;

    void Start()
    {
        HideMenus();
        ShowMenu();
    }

    public void ShowMenu()
    {
        Debug.Log("He llegado a showmenu!!!!!!");
        Debug.Log("menuNum: " + menuNum);
        for (int i = 0; i < menus.Count; i++)
        {
            if (i == menuNum)
            {
                menus[i].alpha = 1f;
                menus[i].interactable = true;
                menus[i].blocksRaycasts = true;
            }
            

        }
        if (menuNum == 0) titleDecor.alpha = 1f;

        else titleDecor.alpha = 0f;
    }

    public void HideMenus(){
        Debug.Log("He llegado a hidemenus!!!!!!");
        Debug.Log("menuNum: " + menuNum);
        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].alpha = 0f;
            menus[i].interactable = false;
            menus[i].blocksRaycasts = false;
        }
    }

 

    public void ChangeMenuNum (int newNum){
        Debug.Log("He llegado a changemenunum!!!!!!");
        Debug.Log("menuNum: " + menuNum);
        menuNum = newNum;
        Debug.Log("newNum: " + menuNum);
    }

    public void ExitGame()
    {
        Debug.Log("El juego se ha cerrado.");
        Application.Quit();
    }


}
