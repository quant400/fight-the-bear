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
        SceneManager.sceneLoaded += OnSceneLoaded;

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

                    uiView.goToMenu("main");
                    break;
                case bearGameModel.GameSteps.OnPlayMenu:

                    uiView.goToMenu("main");

                    break;
                case bearGameModel.GameSteps.OnLeaderBoard:

                    uiView.goToMenu("leaderboeard");
                    break;
                case bearGameModel.GameSteps.OnCharacterSelection:
                    uiView.goToMenu("characterSelection");
                    if (!gameplayView.instance.isTryout)
                    {
                        characterSelectionView.MoveRight();
                        characterSelectionView.MoveLeft();
                    }

                    //webView.checkUSerLoggedAtStart(); /// condisder when start load again .....  !!!! 
                    break;
                case bearGameModel.GameSteps.OnCharacterSelected:
                    uiView.goToMenu("characterSelected");
                    gameEndView.resetDisplay();
                    scenesView.loadSinglePlayerScene(1);

                    break;
                case bearGameModel.GameSteps.OnStartGame:
                    Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(2)
                        .Do(_ => gameView.StartGame())
                        .Subscribe()
                        .AddTo(this);
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnPathLoad;
                    //bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGameRunning;

                    break;
                case bearGameModel.GameSteps.OnGameRunning:
                    Debug.Log("game Is running");
                    break;
                case bearGameModel.GameSteps.OnGameEnded:
                    uiView.goToMenu("results");
                    if (!gameplayView.instance.isTryout)
                        gameEndView.setScoreAtStart();
                    gameView.EndGame();
                    break;
                case bearGameModel.GameSteps.OnBackToCharacterSelection:
                    gameEndView.initializeValues();
                    gameEndView.resetDisplay();
                    dataView.initilizeValues();
                    scenesView.LoadScene(bearGameModel.mainSceneLoadname.sceneName);
                    Observable.Timer(TimeSpan.Zero)
                       .DelayFrame(2)
                       .Do(_ => bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelection)
                       .Subscribe()
                       .AddTo(this);
                    break;
                case bearGameModel.GameSteps.OnSceneLoaded:
                    Debug.Log("sceneLoaded");
                    break;

                case bearGameModel.GameSteps.OnPathLoad:
                   MapView.instance.SpawnStarting();
                    break;
                
                case bearGameModel.GameSteps.OnPathLoaded:
                    MapView.instance.SpawnPlayer();
                    SFXView.instance.SetSFX("Path");
                    break;

                case bearGameModel.GameSteps.OnTryAgain:
                    gameplayView.instance.started = false;
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCharacterSelected;
                    GameUIView.instance.ResetGame();
                    break;

                case bearGameModel.GameSteps.OnGoToMain:
                    gameplayView.instance.started = false;
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
                    MapView.instance.StartCutsene();
                    break;

                case bearGameModel.GameSteps.OnEnterCave:
                    //Change here
                    MapView.instance.GoIntoCave();
                    SFXView.instance.SetSFX("Bear");
                    break;

            }

            }
        }
    }


