using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;
using UniRx.Triggers;
using UniRx.Operators;
using System;
using UnityEngine.SceneManagement;

    public class mainPresenter : MonoBehaviour
    {
    [SerializeField] gameplayView gameView;
    [SerializeField] webLoginView webView;
    [SerializeField] characterSelectionView characterSelectionView;
    [SerializeField] uiView uiView;
    [SerializeField] gameEndView gameEndView;
    [SerializeField] DatabaseManagerRestApi dataView;
    [SerializeField] GameUIView gameUIView;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if ((scene.name == bearGameModel.singlePlayerScene1.sceneName) && !gameplayView.instance.started) 
        {
            Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(2)
                        .Do(_ => bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnStartGame)
                        .Subscribe()
                        .AddTo(this);
        }
    }
    // Start is called before the first frame update
    void Start()
        {
        ObservePanelsStatus();
      
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnLogin;
    }

    // Update is called once per frame
    void Update()
        {

        }

    void ObservePanelsStatus()
    {
            bearGameModel.gameCurrentStep
                   .Subscribe(procedeGame)
                   .AddTo(this);

            void procedeGame(bearGameModel.GameSteps status)
            {
                switch (status)
                {
                    case bearGameModel.GameSteps.OnLogin:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    SceneManager.LoadSceneAsync(bearGameModel.mainSceneLoadname.sceneName, LoadSceneMode.Additive);

                    if (bearGameModel.userIsLogged.Value)
                        {
                        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelection;

                        }

                    else
                        {
                        uiView.goToMenu("login");
                        uiView.observeLogin();
                        }
                    break;
                case bearGameModel.GameSteps.Onlogged:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("main");
                    break;
                case bearGameModel.GameSteps.OnPlayMenu:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("main");

                    break;
                case bearGameModel.GameSteps.OnLeaderBoard:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("leaderboeard");
                    break;
                case bearGameModel.GameSteps.OnCharacterSelection:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("characterSelection");
                    if (!gameplayView.instance.isTryout)
                    {
                        characterSelectionView.MoveRight();
                        characterSelectionView.MoveLeft();
                    }

                    //webView.checkUSerLoggedAtStart(); /// condisder when start load again .....  !!!! 
                    break;
                case bearGameModel.GameSteps.OnCharacterSelected:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("characterSelected");
                    gameEndView.resetDisplay();
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnStartGame;
                    break;
                case bearGameModel.GameSteps.OnStartGame:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(2)
                        .Do(_ => gameView.StartGame())
                        .Subscribe()
                        .AddTo(this);
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnPathLoad;
                    //bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGameRunning;

                    break;
                case bearGameModel.GameSteps.OnGameEnded:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    uiView.goToMenu("results");
                    if (!gameplayView.instance.isTryout)
                        gameEndView.setScoreAtStart();
                    gameView.EndGame();
                    break;
                case bearGameModel.GameSteps.OnBackToCharacterSelection:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    gameEndView.initializeValues();
                    gameEndView.resetDisplay();
                    dataView.initilizeValues();
                    SceneManager.LoadSceneAsync(bearGameModel.mainSceneLoadname.sceneName,LoadSceneMode.Additive);
                    Observable.Timer(TimeSpan.Zero)
                       .DelayFrame(2)
                       .Do(_ => bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelection)
                       .Subscribe()
                       .AddTo(this);
                    break;
                case bearGameModel.GameSteps.OnSceneLoaded:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    SceneManager.UnloadSceneAsync(bearGameModel.mainSceneLoadname.sceneName);
                    break;

                case bearGameModel.GameSteps.OnPathLoad:
                    FightModel.currentFightStatus.Value = FightModel.fightStatus.OnPath;
                    GameUIView.instance.DeactivateFightCanvas();
                    SceneManager.LoadScene(bearGameModel.singlePlayerScene1.sceneName, LoadSceneMode.Additive);
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    MapView.instance.SpawnStarting();
                    break;
                case bearGameModel.GameSteps.OnPathLoaded:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    int countLoaded = SceneManager.sceneCount;
                    unloadScene(bearGameModel.mainSceneLoadname.sceneName);
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnSpawnPlayer;
                    SFXView.instance.SetSFX("Path");
                    break;
                case bearGameModel.GameSteps.OnSpawnPlayer:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    initilizeValues();
                    if (FightModel.currentPlayer == null)
                    {
                        MapView.instance.SpawnPlayer();
                        FighterView.instance.intilize(true);
                    }
                    else
                    {
                        Observable.Timer(TimeSpan.Zero)
                            //.Do(_ => cinematicView.instance.setCamera(true, 0))
                            .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                            .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                            .Do(_ => FightModel.currentPlayer.transform.position = new Vector3(0, 0, -45))
                            .Do(_ => FightModel.currentPlayer.transform.eulerAngles = new Vector3(0, 0, 0))
                            .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                            .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                            .Do(_ => bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnFindingCave)
                            .Do(_ => FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks(false))
                            .Do(_ => FighterView.instance.intilize(true))
                            .Subscribe()
                           .AddTo(this);

                    }
                    break;
                case bearGameModel.GameSteps.OnTryAgain:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    gameplayView.instance.started = false;
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelected;
                    unloadScene(bearGameModel.singlePlayerScene3.sceneName);
                    GameUIView.instance.ResetGame();
                    break;
                case bearGameModel.GameSteps.OnGoToNextLevel:
                    SceneManager.UnloadSceneAsync(bearGameModel.singlePlayerScene3.sceneName);
                    uiView.goToMenu("characterSelected");
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnPathLoad;
                    break;

                case bearGameModel.GameSteps.OnGoToMain:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    Time.timeScale = 1;
                    gameplayView.instance.started = false;
                    unloadScene(bearGameModel.singlePlayerScene3.sceneName);
                    GameUIView.instance.ResetGame();
                    if(gameplayView.instance.isTryout)
                    {
                        gameplayView.instance.isTryout = false;
                        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnLogin;
                    }
                    else
                    {
                        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnBackToCharacterSelection;
                    }
                    
                    break;

                case bearGameModel.GameSteps.OnCloseToCave:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    MapView.instance.StartCutsene();
                    break;

                case bearGameModel.GameSteps.OnEnterCave:
                    Debug.Log(bearGameModel.gameCurrentStep.Value.ToString());
                    Observable.Timer(TimeSpan.Zero)
                      .DelayFrame(2)
                      .Do(_ => EnterCaseSet())
                      .Subscribe()
                      .AddTo(this);
                    Observable.Timer(TimeSpan.Zero)
                       .Do(_ =>FightModel.currentPlayer.GetComponent<RockThrowView>().throwAndSetBackDirect(1000))
                       .Do(_ => FightModel.currentPlayer.GetComponent<RockThrowView>().FakeRock.gameObject.SetActive(false))
                       .Subscribe()
                       .AddTo(this);
                   
                    break;

            }

            }
        }
    void EnterCaseSet()
    {
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelection;
        MapView.instance.GoIntoCave();
        GameUIView.instance.ActivateInputs();
        SFXView.instance.SetSFX("Bear");
        unloadScene("PathScene");
        uiView.goToMenu("characterSelected");
    }
    void unloadScene(string sceneName)
    {
        int countLoaded1 = SceneManager.sceneCount;
        Scene[] loadedScenes1 = new Scene[countLoaded1];

        for (int i = 0; i < countLoaded1; i++)
        {
            loadedScenes1[i] = SceneManager.GetSceneAt(i);
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                Debug.Log("finded");
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
    void initilizeValues()
    {
        FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearCinematicMode;
        FightModel.currentFightStatus.Value= FightModel.fightStatus.OnPath;
    }
}


