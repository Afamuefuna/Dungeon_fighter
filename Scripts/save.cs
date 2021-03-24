using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class save : MonoBehaviour
{
    [SerializeField]
    private TMP_Text saveText;
    bool shouldCount;

    [SerializeField]
    private float count, stopCount;

    private void OnMouseDown()
    {
        GameManager.Instance.savePlayer();
        shouldCount = true;
        saveText.text = "Saved";
    }

    private void Update()
    {
        if (shouldCount)
        {
            count = count + Time.deltaTime;
            if (count > stopCount)
            {
                saveText.text = "Click To Save";
                shouldCount = false;
                count = 0;
            }
        }
    }

}
