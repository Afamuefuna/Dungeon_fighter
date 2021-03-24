using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadButton : MonoBehaviour
{
    bool hasClicked;
    public void onClick()
    {
        if (!hasClicked)
        {
            GameManager.Instance.loadingStart = GameObject.FindGameObjectWithTag("start").GetComponent<TMP_Text>();

            GameManager.Instance.restartScene = false;
            GameManager.Instance.loadGameState(GameManager.Instance.loadingStart);
            GameManager.Instance.openGameSceneFromSave(true);
            hasClicked = true;
        }
    }
}
