using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance ; } }

    public string sceneToLoad;
    AsyncOperation loadingOperation;
    public bool clickedOnStart, ClickedOnSavedScene, restartScene;

    public TMP_Text loadingStart;

    public Player player;

    bool playedEntranceSwoosh;

    AudioSource Waterfall_;

    public bool hasKilledSkeleton, hasKilledMonster, hasKilledMoss, hasPlayedIntroduction, hasPlayedSaveWord;

    private void Awake()
    {
        playedEntranceSwoosh = false;
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void openGameSceneFromSave(bool restartOrLoad)
    {
        loadingStart = GameObject.FindGameObjectWithTag("start").GetComponent<TMP_Text>();
        Audio.Instance.UISound(Audio.Instance.drawSword);
        clickedOnStart = true;
        playedEntranceSwoosh = false;
        loadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        ClickedOnSavedScene = restartOrLoad;
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void mainSceneLoad()
    {
        Boss.unlockBossPoints = 0;
        loadScene("Main");
        clickedOnStart = false;
        Audio.Instance.BGsound(Audio.Instance.BG1);
    }

    public void pauseGame()
    {
        foreach (AudioSource audio in Audio.Instance.audioSources)
        {
            if (audio == Audio.Instance.audioSources[3])
                continue;

            if (audio == Audio.Instance.audioSources[4])
                continue;

            audio.mute = true;
        }

        Time.timeScale = 0;

        Waterfall_ = GameObject.FindGameObjectWithTag("waterfall").GetComponent<AudioSource>();

        Waterfall_.mute = true;
    }

    public void resumeGame()
    {
        foreach (AudioSource audio in Audio.Instance.audioSources)
        {
            if (audio == Audio.Instance.audioSources[3])
                continue;

            if (audio == Audio.Instance.audioSources[4])
                continue;

            audio.mute = false;
        }

        Time.timeScale = 1;

        Waterfall_ = GameObject.FindGameObjectWithTag("waterfall").GetComponent<AudioSource>();

        Waterfall_.mute = false;
    }

    private void Update()
    {
        if (clickedOnStart)
        {
            float loadProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            loadingStart.text = "Loading " + Mathf.Round(loadProgress * 100) + "%";

            if (loadingOperation.isDone)
            {
                if (!playedEntranceSwoosh)
                {
                    Audio.Instance.UISound(Audio.Instance.swoosh);
                    Audio.Instance.BGsound(Audio.Instance.BG2);
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    if (ClickedOnSavedScene)
                    {
                        loadPlayer(loadingStart);
                    }else if (!ClickedOnSavedScene)
                    {
                        Boss.unlockBossPoints = 0;
                    }
                    ClickedOnSavedScene = false;
                    clickedOnStart = false;
                    playedEntranceSwoosh = true;
                }
            }
        }
    }

    public void savePlayer()
    {
        saveSystem.SavePlayer(player);
    }

    public void loadPlayer(TMP_Text errorText)
    {
        PlayerData data = saveSystem.loadPlayer(errorText);

        player.Playerslive = data.health;

        Vector3 position;

        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        player.transform.position = position;
    }

    public void loadGameState(TMP_Text errorText)
    {
        PlayerData data = saveSystem.loadPlayer(errorText);

        hasKilledSkeleton = data.hasKilledSkeleton;
        hasKilledMoss = data.hasKilledMoss;
        hasKilledMonster = data.hasKilledMonster;
        hasPlayedIntroduction = data.hasPlayedIntroduction;
        hasPlayedSaveWord = data.hasPlayedSaveW0rd;
    }

    public void resetEnemySaved()
    {
        hasKilledMonster = false;
        hasKilledMoss = false;
        hasKilledSkeleton = false;
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
