
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UniRx.Triggers;
using UniRx;
using UniRx.Operators;
public class gameplayView : MonoBehaviour
{
    public static gameplayView instance;

    public int toSpawn;
    //public int chosenAvatar; changed to nft object 
    [SerializeField]
    GameObject player;
    //public int startDelay;
    [SerializeField]
    float timeForOneGame;
    // moved to spawners of each level
    //[SerializeField]
    //int initialChickenCount;
    //[SerializeField]
    //float spawnIntervals;
    [SerializeField]
    float mushroomPowerUpChance;

    public NFTInfo chosenNFT;

    public int dailyScore, AlltimeScore, sessions;
    public bool isRestApi;
    public ReactiveProperty<int> dailysessionReactive = new ReactiveProperty<int>();
    public static NFTInfo[] nftDataArray;
    public static bool playerLogged;
    public GameObject gameOverObject;

    bool sfxMuted = false;

    public bool isTryout = false;
    float score;
    public bool started = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
           
            if (isRestApi)
            {
                observeReactiveSession();
            }
        }
        if(PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetString("SFX") == "off")
                sfxMuted = true;
        }
    }


 

    public void StartGame()
    {
        started = true;
        //SinglePlayerScoreBoardScript.instance.StartGame(GetTimeForGame());
        player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<ThirdPersonController>().SetStarted(true);
        GetScores();
        Debug.Log(chosenNFT.id);
        if (!instance.isTryout)
            DatabaseManagerRestApi._instance.startSessionFromRestApi(chosenNFT.id);
        chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnGameRunning;

    }
    public void EndGame()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<ThirdPersonController>().SetEnded(true);
    }
   

    public float GetTimeForGame()
    {
        return timeForOneGame;
    }


    /*public int GetChickenCount()
    {
        return initialChickenCount;
    }
    public float GetSpawnInterval()
    {
        return spawnIntervals;
    }*/

    public bool GetSFXMuted()
    {
        return sfxMuted;
    }

    public void SetSFXMuted(bool b)
    {
        sfxMuted = b;
    }
    public float GetMushroomPowerUpChance()
    {
        return mushroomPowerUpChance;
    }

    public void PlayClick()
    {
        GetComponent<AudioSource>().Play();
    }

    public int GetDailyScore()
    {
        if (dailyScore == -1)
            return 0;
        return dailyScore;
    }
    public int GetAllTimeScore()
    {
        if (AlltimeScore == -1)
            return 0;
        return AlltimeScore;
    }
    public int GetSessions()
    {
        return sessions;
    }

    public void GetScores()
    {
        if (isRestApi)
        {
            GetSoresRestApi();
        }
        else
        {
            // DatabaseManager._instance.getDailyLeaderboardScore(chosenNFT.id.ToString(), x => { dailyScore = (int)x; });
            // DatabaseManager._instance.getLeaderboardScore(chosenNFT.id.ToString(), x => { AlltimeScore = (int)x; });
            //  DatabaseManager._instance.getSessionsCounter(chosenNFT.id.ToString(), x => { sessions = (int)x; });
        }

    }
    void GetSoresRestApi()
    {
        DatabaseManagerRestApi._instance.getDataFromRestApi(chosenNFT.id);

    }
    void observeReactiveSession()
    {
        dailysessionReactive
            .Where(_ => _ > 10)
            .Do(_ => endGameDirectly())
            .Do(_ => dailysessionReactive.Value = 0)
            .Subscribe()
            .AddTo(this);
    }
    void endGameDirectly()
    {
        EndGame();
        SinglePlayerScoreBoardScript.instance.DisplayScore();
    }



    public void SetLocalScore(float val)
    {
        score = val;
    }
    public float GetLocalScore()
    {
        return score;
    }
}

