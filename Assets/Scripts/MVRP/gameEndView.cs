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
public class gameEndView : MonoBehaviour
{
    [SerializeField]
    Transform characterDisplay;
    GameObject[] characters;
    [SerializeField]
    TMP_Text currentScore, dailyScore, allTimeScore, sessionCounterText;
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
    GameObject localDisplay;
    // [SerializeField]
    //SinglePlayerSpawner spawner;

    private void OnEnable()
    {
        if (gameplayView.instance.isTryout)
        {
            tryAgain.gameObject.SetActive(false);
        }
        else
        {
            tryAgain.gameObject.SetActive(true);
        }
    }
    public void Start()
    {
        observeScoreChange();
        endGameAfterValueChange();
        ObserveGameObverBtns();
    }
    public void setScoreAtStart()
    {
        /*if (canvasToDisable == null)
        {
            canvasToDisable = gameplayView.instance.gameObject.transform.GetChild(0).gameObject;
        }*/
        tryAgain.gameObject.SetActive(false);
        currentNFT = gameplayView.instance.chosenNFT;
        if (gameplayView.instance.GetSessions() <= 3)
        {
            if (gameplayView.instance.isRestApi)
            {
                Debug.Log("before Score");
                DatabaseManagerRestApi._instance.setScoreRestApiMain(currentNFT.id.ToString(), (int)FightModel.gameScore.Value);
                Debug.Log("posted Score");
                if (gameplayView.instance.GetSessions() == 3)
                    tryAgain.gameObject.SetActive(false);
            }
            else
            {
                // DatabaseManager._instance.setScore(currentNFT.id.ToString(), currentNFT.name, SinglePlayerScoreBoardScript.instance.GetScore());

            }
        }
        gameplayView.instance.GetScores();
        //setScoreResutls();

    }
    public void initializeValues()
    {
        scorereactive.Value = -1;
        sessions.Value = -1;
        gameEnded.Value = false;
    }
    public void ObserveGameObverBtns()
    {

        tryAgain.OnClickAsObservable()
            .Where(_=>gameEnded.Value==true)
            .Do(_ => TryAgain())
            .Where(_ => PlaySounds.instance != null)
            .Do(_ => PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);

      
    }
    public void updateResults()
    {
        currentScore.text = "SCORE: " + FightModel.gameScore.Value;

    }
   
    public void endGameAfterValueChange()
    {
        gameEnded
            .Where(_ => _ == true)
            .Do(_ => updateResults())
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
    public void resetDisplay()
    {
        if (localDisplay!=null)
        Destroy(localDisplay);
    }
    private void Update()
    {
        if (bearGameModel.gameCurrentStep.Value == bearGameModel.GameSteps.OnGameEnded)
        {
            scorereactive.Value = gameplayView.instance.dailyScore;
            sessions.Value = gameplayView.instance.sessions;
        }
    }
    public void setScoreToUI()
    {
        gameEnded.Value = true;
        currentScore.text = "SCORE: " + FightModel.gameScore.Value;

    }
    public void TryAgain()
    {
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnTryAgain;
    }
    public void goToMain()
    { 
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGoToMain;

    }

    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;
    }
}
