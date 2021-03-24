using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class deathController : MonoBehaviour
{
    bool hasClicked;

    public void restart()
    {
        if (!hasClicked)
        {
            GameManager.Instance.openGameSceneFromSave(false);
            GameManager.Instance.ClickedOnSavedScene = false;
            GameManager.Instance.restartScene = true;
            GameManager.Instance.resetEnemySaved();
            GameManager.Instance.hasPlayedSaveWord = false;
            hasClicked = true;
        }
    }

    public void load()
    {
        if (!hasClicked)
        {
            GameManager.Instance.loadingStart = GameObject.FindGameObjectWithTag("start").GetComponent<TMP_Text>();

            GameManager.Instance.loadGameState(GameManager.Instance.loadingStart);
            GameManager.Instance.restartScene = false;
            GameManager.Instance.openGameSceneFromSave(true);
            hasClicked = true;
        }
    }

    public void loadMainScene()
    {
        GameManager.Instance.mainSceneLoad();
    }

    public void quit()
    {
        GameManager.Instance.quitGame();
    }
}
