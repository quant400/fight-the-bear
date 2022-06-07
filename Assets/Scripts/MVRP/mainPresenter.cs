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


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if ((scene.name == chickenGameModel.singlePlayerScene1.sceneName) && !gameplayView.instance.started) 
        {
            Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(2)
                        .Do(_ => UIController.instance.fightCanvas.SetActive(false))
                        .Do(_ => chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnStartGame)
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
            chickenGameModel.gameCurrentStep
                   .Subscribe(procedeGame)
                   .AddTo(this);

            void procedeGame(chickenGameModel.GameSteps status)
            {
                switch (status)
                {
                    case chickenGameModel.GameSteps.OnLogin:
                       
                        if (chickenGameModel.userIsLogged.Value)
                        {
                        chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnCharacterSelection;

                        }

                    else
                        {
                        uiView.goToMenu("login");
                        uiView.observeLogin();
                        }
                    break;
                case chickenGameModel.GameSteps.Onlogged:

                    uiView.goToMenu("main");
                    break;
                case chickenGameModel.GameSteps.OnPlayMenu:

                    uiView.goToMenu("main");

                    break;
                case chickenGameModel.GameSteps.OnLeaderBoard:

                    uiView.goToMenu("leaderboeard");
                    break;
                case chickenGameModel.GameSteps.OnCharacterSelection:
                    uiView.goToMenu("characterSelection");
                    if (!gameplayView.instance.isTryout)
                    {
                        characterSelectionView.MoveRight();
                        characterSelectionView.MoveLeft();
                    }

                    //webView.checkUSerLoggedAtStart(); /// condisder when start load again .....  !!!! 
                    break;
                case chickenGameModel.GameSteps.OnCharacterSelected:
                    uiView.goToMenu("characterSelected");
                    gameEndView.resetDisplay();
                    scenesView.loadSinglePlayerScene(1);
                    UIController.instance.fightCanvas.SetActive(false);

                    break;
                case chickenGameModel.GameSteps.OnStartGame:
                    Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(2)
                        .Do(_ => gameView.StartGame())
                        .Subscribe()
                        .AddTo(this);

                    chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnGameRunning;

                    break;
                case chickenGameModel.GameSteps.OnGameRunning:
                    Debug.Log("game Is running");
                    break;
                case chickenGameModel.GameSteps.OnGameEnded:
                    uiView.goToMenu("results");
                    if (!gameplayView.instance.isTryout)
                        gameEndView.setScoreAtStart();
                    gameView.EndGame();
                    break;
                case chickenGameModel.GameSteps.OnBackToCharacterSelection:
                    gameEndView.initializeValues();
                    gameEndView.resetDisplay();
                    dataView.initilizeValues();
                    scenesView.LoadScene(chickenGameModel.mainSceneLoadname.sceneName);
                    Observable.Timer(TimeSpan.Zero)
                       .DelayFrame(2)
                       .Do(_ => chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnCharacterSelection)
                       .Subscribe()
                       .AddTo(this);
                    break;
                case chickenGameModel.GameSteps.onSceneLoaded:
                    Debug.Log("sceneLoaded");
                    break;


            }

            }
        }
    }


