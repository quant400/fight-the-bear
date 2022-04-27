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
    public void Start()
    {
        observeScoreChange();
        endGameAfterValueChange();
        ObserveGameObverBtns();
    }
    public void setScoreAtStart()
    {
        if (canvasToDisable == null)
        {
            canvasToDisable = gameplayView.instance.gameObject.transform.GetChild(0).gameObject;
        }
        currentNFT = gameplayView.instance.chosenNFT;
        if (gameplayView.instance.GetSessions() <= 10)
        {
            if (gameplayView.instance.isRestApi)
            {
                Debug.Log("before Score");

                DatabaseManagerRestApi._instance.setScoreRestApiMain(currentNFT.id.ToString(), SinglePlayerScoreBoardScript.instance.GetScore());
                Debug.Log("posted Score");
            }
            else
            {
                // DatabaseManager._instance.setScore(currentNFT.id.ToString(), currentNFT.name, SinglePlayerScoreBoardScript.instance.GetScore());

            }
        }
        gameplayView.instance.GetScores();
        setScoreResutls();

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
        if (gameplayView.instance.GetSessions() < 10)
        {

            sessionsLeft.SetActive(true);
            sessionsNotLeft.SetActive(false);
            currentScore.text = "CHICKENS CAUGHT : " + SinglePlayerScoreBoardScript.instance.GetScore().ToString();
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }
        else if (gameplayView.instance.GetSessions() >= 10)
        {
            sessionsLeft.SetActive(false);
            sessionsNotLeft.SetActive(true);
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }
        SinglePlayerScoreBoardScript.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);

    }
    public void setScoreResutls()
    {

        if (gameplayView.instance.GetSessions() < 10)
        {

            sessionsLeft.SetActive(true);
            sessionsNotLeft.SetActive(false);
            currentScore.text = "CHICKENS CAUGHT : " + SinglePlayerScoreBoardScript.instance.GetScore().ToString();
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }
        else if (gameplayView.instance.GetSessions() >= 10)
        {
            sessionsLeft.SetActive(false);
            sessionsNotLeft.SetActive(true);
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }


        AudioSource ad =GetComponent<AudioSource>();
        ad.clip = gameOverClip;
        ad.loop = false;
        ad.volume = 0.2f;
        ad.Play();
        //characters = spawner.GetCharacterList();
        Debug.Log("Load character");
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
        localDisplay = temp;
        //upddate other values here form leaderboard
        SinglePlayerScoreBoardScript.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
        if (chickenGameModel.gameCurrentStep.Value == chickenGameModel.GameSteps.OnGameEnded)
        {
            scorereactive.Value = gameplayView.instance.dailyScore;
            sessions.Value = gameplayView.instance.sessions;
        }
    }
    public void setScoreToUI()
    {
        gameEnded.Value = true;
        if (gameplayView.instance.GetSessions() < 10)
        {

            sessionsLeft.SetActive(true);
            sessionsNotLeft.SetActive(false);
            currentScore.text = "CHICKENS CAUGHT : " + SinglePlayerScoreBoardScript.instance.GetScore().ToString();
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }
        else if (gameplayView.instance.GetSessions() >= 10)
        {
            sessionsLeft.SetActive(false);
            sessionsNotLeft.SetActive(true);
            dailyScore.text = "DAILY SCORE : " + (gameplayView.instance.GetDailyScore());
            allTimeScore.text = "ALL TIME SCORE : " + (gameplayView.instance.GetAllTimeScore());
            sessionCounterText.text = "NFT DAILY RUNS : " + (gameplayView.instance.GetSessions()) + "/10";

        }

    }
    public void TryAgain()
    {
        chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnCharacterSelected;
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
