using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
public class GameOverScript : MonoBehaviour
{
    [SerializeField]
    Transform characterDisplay;
    GameObject[] characters;
    [SerializeField]
    TMP_Text currentScore, dailyScore, allTimeScore , sessionCounterText;
    [SerializeField]
    GameObject canvasToDisable;
    [SerializeField]
    AudioClip gameOverClip;
    NFTInfo currentNFT;
    [SerializeField]
    GameObject sessionsLeft, sessionsNotLeft;
    ReactiveProperty<int> scorereactive = new ReactiveProperty<int>();
    ReactiveProperty<int> sessions = new ReactiveProperty<int>();
    ReactiveProperty<bool> gameEnded = new ReactiveProperty<bool>();
    [SerializeField] Button tryAgain, back;
    // [SerializeField]
    //SinglePlayerSpawner spawner;
    public void Start()
    {
        observeScoreChange();
        endGameAfterValueChange();
    }
    private void OnEnable()
    {
        if (canvasToDisable == null)
        {
            canvasToDisable = SinglePlayerScoreBoardScript.instance.gameObject.transform.GetChild(0).gameObject;
        }
        currentNFT = SingleplayerGameControler.instance.chosenNFT;
        if (SingleplayerGameControler.instance.GetSessions() <=10)
        {
            if (SingleplayerGameControler.instance.isRestApi)
            {
                DatabaseManagerRestApi._instance.setScoreRestApiMain(currentNFT.id.ToString(), SinglePlayerScoreBoardScript.instance.GetScore());

            }
            else
            {
               // DatabaseManager._instance.setScore(currentNFT.id.ToString(), currentNFT.name, SinglePlayerScoreBoardScript.instance.GetScore());

            }
        }
        SingleplayerGameControler.instance.GetScores();


    }
    public void ObserveGameObverBtns()
    {

        tryAgain.OnClickAsObservable()
            .Do(_ => TryAgain())
            .Where(_ => PlaySounds.instance != null)
            .Do(_ => PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);
        back.OnClickAsObservable()
           .Do(_ => chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnPlayMenu)
           .Where(_ => PlaySounds.instance != null)
           .Do(_ => PlaySounds.instance.Play())
           .Subscribe()
           .AddTo(this);
    }
    public void setScoreResutls()
    {
       
        if (SingleplayerGameControler.instance.GetSessions() < 10)
        {
            
            sessionsLeft.SetActive(true);
            sessionsNotLeft.SetActive(false);
            currentScore.text = "CHICKENS CAUGHT : " + SinglePlayerScoreBoardScript.instance.GetScore().ToString();
            dailyScore.text = "DAILY SCORE : " + (SingleplayerGameControler.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (SingleplayerGameControler.instance.GetAllTimeScore() );
            sessionCounterText.text = "NFT DAILY RUNS : " + (SingleplayerGameControler.instance.GetSessions()) + "/10";

        }
        else if (SingleplayerGameControler.instance.GetSessions() >= 10)
        {
            sessionsLeft.SetActive(false);
            sessionsNotLeft.SetActive(true);
            dailyScore.text = "DAILY SCORE : " + (SingleplayerGameControler.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (SingleplayerGameControler.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (SingleplayerGameControler.instance.GetSessions()) + "/10";

        }


        AudioSource ad = GameObject.FindGameObjectWithTag("SFXPlayer").GetComponent<AudioSource>();
        ad.clip = gameOverClip;
        ad.loop = false;
        ad.volume = 0.2f;
        ad.Play();
        //characters = spawner.GetCharacterList();
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        GameObject displayChar = Resources.Load(Path.Combine("SinglePlayerPrefabs/Characters", NameToSlugConvert(currentNFT.name))) as GameObject;
        var temp = Instantiate(displayChar, characterDisplay.position, Quaternion.identity, characterDisplay);

        //destroying all player related components
        Destroy(temp.transform.GetChild(1).gameObject);
        Destroy(temp.transform.GetChild(0).gameObject);
        Destroy(temp.transform.GetChild(2).gameObject);
        Destroy(temp.transform.GetChild(3).gameObject);
        Destroy(temp.GetComponent<StarterAssetsInputs>());
        Destroy(temp.GetComponent<ThirdPersonController>());
        Destroy(temp.GetComponent<CharacterController>());
        Destroy(temp.GetComponent<PlayerInput>());
        temp.GetComponent<Animator>().SetBool("Ended", true);

        temp.transform.localPosition = Vector3.zero;
        temp.transform.localRotation = Quaternion.identity;
        temp.transform.localScale = Vector3.one * 2;

        //upddate other values here form leaderboard
        SinglePlayerScoreBoardScript.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void endGameAfterValueChange()
    {
        gameEnded
            .Where(_ => _ == true)
            .Do(_ => setScoreResutls())
            .Subscribe()
            .AddTo(this);
    }
    public void observeScoreChange()
    {
        scorereactive
            .Do(_ => setScoreToUI())
            .Subscribe()
            .AddTo(this);

        sessions
            .Do(_ => setScoreToUI())
            .Subscribe()
            .AddTo(this);

    }
    private void Update()
    {
        if (chickenGameModel.gameCurrentStep.Value == chickenGameModel.GameSteps.OnGameEnded)
        {
            scorereactive.Value = SingleplayerGameControler.instance.dailyScore;
            sessions.Value = SingleplayerGameControler.instance.sessions;
        }
    }
    public void setScoreToUI()
    {
        gameEnded.Value = true;
        currentScore.text = "CHICKENS CAUGHT : " + SinglePlayerScoreBoardScript.instance.GetScore().ToString() ;
        dailyScore.text = "DAILY SCORE : " + (SingleplayerGameControler.instance.GetDailyScore() );
        allTimeScore.text = "ALL TIME SCORE : " + (SingleplayerGameControler.instance.GetAllTimeScore() );
        sessionCounterText.text= "NFT DAILY RUNS : " + (SingleplayerGameControler.instance.GetSessions())+"/10";
    }
    public void TryAgain()
    {
        scenesView.loadSinglePlayerScene(1);

    }
    public void goToMain()
    {
        scenesView.LoadScene(chickenGameModel.mainSceneLoadname.sceneName);
        chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnBackToMenu;
    }

    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;
    }
}
