using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour
{
    bool hasClicked;
    public void onClick()
    {
        if (!hasClicked)
        {
            GameManager.Instance.restartScene = true;
            GameManager.Instance.resetEnemySaved();
            GameManager.Instance.hasPlayedIntroduction = false;
            GameManager.Instance.hasPlayedSaveWord = false;
            GameManager.Instance.openGameSceneFromSave(false);
            hasClicked = true;
        }
;
    }
}
