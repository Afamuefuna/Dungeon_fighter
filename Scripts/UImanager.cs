using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UImanager : MonoBehaviour
{
    private static UImanager _instance;

    public static UImanager Instance { get { return _instance; } }

    [SerializeField]
    TMP_Text TextTypingPanel;

    public float delay;
    [TextArea(20, 20)]
    public string welcomeText, welcomeText1, combatInstructionText, saveWord, bossWord,
        bossWord1, bossWinWord, congratulatoryMessage;
    private string currentText = "";

    [SerializeField]
    GameObject attack, dialoguePanel, pausePanel, menuButtonInWin;

    [SerializeField]
    TMP_Text Title, nextText;

    int displayedTextID = 0;

    public bool playedSaveWord, isOnDialogue;

    [SerializeField]
    bool startedFirstCoroutine, canCount = true;

    public Animator mAnimator;

    public string currentState;

    [SerializeField]
    float counter, timeTowait;

    public enum wordMode { start, saveWord, BossFight, win }

    public wordMode wordToSay;

    private bool playedBossWord;
    public bool endOfBossWord;
    private bool playedWinWord;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    IEnumerator ShowText(string fullText, TMP_Text TypingText)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            TypingText.text = currentText;
            Audio.Instance.UISound(Audio.Instance.typeWriter);
            yield return new WaitForSeconds(delay);
        }
    }

    public void mainSceneLoad()
    {
        GameManager.Instance.mainSceneLoad();
    }

    public void gameSceneLoad()
    {
        GameManager.Instance.loadScene("Game");
        Audio.Instance.BGsound(Audio.Instance.BG2);
    }

    public void pause()
    {
        GameManager.Instance.pauseGame();
        pausePanel.SetActive(true);
    }

    public void resume()
    {
        GameManager.Instance.resumeGame();
        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.restartScene)
        {
            if (!GameManager.Instance.hasPlayedIntroduction)
            {
                startInstructions();

                if (canCount)
                {
                    dialoguePanel.SetActive(true);

                    counter = counter + Time.deltaTime;
                    if (counter > timeTowait)
                    {
                        if (!startedFirstCoroutine)
                        {
                            StartCoroutine(ShowText(welcomeText, TextTypingPanel));
                            startedFirstCoroutine = true;
                            canCount = false;
                            displayedTextID = 0;
                            GameManager.Instance.hasPlayedIntroduction = true;
                        }
                    }
                }
            }
        }

        if (dialoguePanel.activeInHierarchy)
        {
            isOnDialogue = true;
        }
        else
        {
            isOnDialogue = false;
        }
    }

    public void startInstructions()
    {
        Title.text = "WELCOME";
        wordToSay = wordMode.start;
    }

    public void startSaveWord()
    {
        if (!GameManager.Instance.hasPlayedSaveWord)
        {
            wordToSay = wordMode.saveWord;
            dialoguePanel.SetActive(true);
            Title.text = "Andromalius";
            StopAllCoroutines();
            StartCoroutine(ShowText(saveWord, TextTypingPanel));
            GameManager.Instance.hasPlayedSaveWord = true;
        }
    }

    public void startBossWord()
    {
        if (!playedBossWord)
        {
            wordToSay = wordMode.BossFight;
            dialoguePanel.SetActive(true);
            Title.text = "Bossus";
            StopAllCoroutines();
            StartCoroutine(ShowText(bossWord, TextTypingPanel));
            playedBossWord = true;
        }
    }

    public void enemyHealthUpdate(float currentHealth, float fullHealth, TMP_Text lifeText)
    {
        float halfOfHealth;
        float lowHealth;

        halfOfHealth = fullHealth / 2;
        lowHealth = fullHealth - (fullHealth - 2);

        if(currentHealth == fullHealth)
        {
            lifeText.color = Color.green;
        }

        if(currentHealth <= halfOfHealth
            && currentHealth > lowHealth)
        {
            lifeText.color = Color.yellow;
        }

        if(currentHealth == lowHealth)
        {
            lifeText.color = Color.red;
        }

        lifeText.text = currentHealth.ToString();
    }

    public void startWinWord()
    {
        if (!playedWinWord)
        {
            wordToSay = wordMode.win;
            dialoguePanel.SetActive(true);
            Title.text = "Congratulations";
            StopAllCoroutines();
            StartCoroutine(ShowText(bossWinWord, TextTypingPanel));
            playedWinWord = true;
        }
    }

    public void nextButton()
    {
        displayedTextID += 1;

        if (GameManager.Instance.restartScene)
        {
            if(wordToSay == wordMode.start)
            {
                if (!canCount)
                {
                    if (displayedTextID == 1)
                    {
                        StopAllCoroutines();
                        StartCoroutine(ShowText(welcomeText1, TextTypingPanel));
                        return;
                    }
                    else if (displayedTextID == 2)
                    {
                        StopAllCoroutines();
                        Title.text = "CONTROLS";
                        StartCoroutine(ShowText(combatInstructionText, TextTypingPanel));
                        return;
                    }
                    else
                    {
                        displayedTextID = 0;
                        dialoguePanel.SetActive(false);
                        StopAllCoroutines();
                        return;
                    }
                }
                
            }
        }
        if(wordToSay == wordMode.saveWord)
        {
            if(displayedTextID == 1)
            {
                dialoguePanel.SetActive(false);
                StopAllCoroutines();
                displayedTextID = 0;
                return;
            }
        }
        if(wordToSay == wordMode.BossFight)
        {
            if (displayedTextID == 1)
            {
                StopAllCoroutines();
                StartCoroutine(ShowText(bossWord1, TextTypingPanel));
            }
            if(displayedTextID == 2)
            {
                dialoguePanel.SetActive(false);
                StopAllCoroutines();
                displayedTextID = 0;
                endOfBossWord = true;
                return;
            }
        }
        if(wordToSay == wordMode.win)
        {
            if (displayedTextID == 1)
            {
                StopAllCoroutines();
                StartCoroutine(ShowText(congratulatoryMessage, TextTypingPanel));
                menuButtonInWin.SetActive(true);
                nextText.gameObject.SetActive(false);
                nextText.text = "Return";
            }
            if(displayedTextID == 2)
            {
                dialoguePanel.SetActive(false);
                StopAllCoroutines();
                displayedTextID = 0;
            }
            return;
        }
    }
}
