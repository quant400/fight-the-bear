using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UniRx.Triggers;
using UniRx;
using UniRx.Operators;
public class SingleplayerGameControler : MonoBehaviour
{
    public static SingleplayerGameControler instance;
    [SerializeField]
    int singleplayerScene;
    [SerializeField]
    int mainScene;

    public int toSpawn;
    //public int chosenAvatar; changed to nft object 
    [SerializeField]
    GameObject[] toDestroyOnload;
    GameObject player;
    //public int startDelay;
    [SerializeField]
    float timeForOneGame;
    [SerializeField]
    int initialChickenCount;
    [SerializeField]
    float spawnIntervals;
    [SerializeField]
    float mushroomPowerUpChance;

    public NFTInfo chosenNFT;

    public int dailyScore, AlltimeScore, sessions;
    public bool isRestApi;
    public ReactiveProperty<int> dailysessionReactive = new ReactiveProperty<int>();
    public static NFTInfo[] nftDataArray;
    public static bool playerLogged;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
            if (isRestApi)
            {
                observeReactiveSession();
            }
        }
    }


    public void LoadSingleplayer()
    {
        SceneManager.LoadScene(singleplayerScene);
        foreach (GameObject g in toDestroyOnload)
        {
            Destroy(g);
        }
    }
    

    public void StartGame()
    {
        SinglePlayerScoreBoardScript.instance.StartGame(GetTimeForGame());
        player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<ThirdPersonController>().SetStarted(true);
        GetScores();
        DatabaseManagerRestApi._instance.startSessionFromRestApi(chosenNFT.id);

    }
    public void EndGame()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<ThirdPersonController>().SetEnded(true);
    }
    public void loadMain()
    {

        SceneManager.LoadScene(mainScene);
    }

    public float GetTimeForGame()
    {
        return timeForOneGame;
    }
    public int GetChickenCount()
    {
        return initialChickenCount;
    }
    public float GetSpawnInterval()
    {
        return spawnIntervals;
    }

    public int GetSinglePlayerScene()
    {
        return singleplayerScene;
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
            .Where(_ => _ >= 10)
            .Do(_ => endGameDirectly())
            .Subscribe()
            .AddTo(this);
    }
    void endGameDirectly()
    {
        SingleplayerGameControler.instance.EndGame();
        SinglePlayerScoreBoardScript.instance.DisplayScore();
    }
}
