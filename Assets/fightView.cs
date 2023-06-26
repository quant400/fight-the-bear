using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class fightView : MonoBehaviour
{
    public static fightView instance;
    public enum comboNames
    {
        Idle,
        ComboOne,
        ComboTwo,
        ComboThree,
        ComboFour,
        ComboFive,
        ComboSix,
        ComboSeven,
    }
    AnimatorClipInfo[] m_CurrentClipInfo;
    public GameObject bear;
    public GameObject bearShield;
    public GameObject ThowringSpheres;
    public GameObject distanceFightEffects;
    public SlidingDoor door;
    public GameObject hittedRocks;
    public GameObject gate;
    public GameObject fightObsObj;
    ReactiveProperty<int> currentBearShieldHealth = new ReactiveProperty<int>();
    ReactiveProperty<float> currentBearHealthDistance = new ReactiveProperty<float>();


    [SerializeField]
    public Image bearHealth, playerHelthDisplay,bearAnger;
    bool canChangeStatus;
    ReactiveProperty<bool> timerOn = new ReactiveProperty<bool>();
    public float cinematicTime;
    public class modesetted
    {
        public bool[] modes = new bool[3] { false, false, false };
    }
    public modesetted modesStatus = new modesetted();
    public int currentLevel;
    public ReactiveProperty<bool> isInDistanceRage = new ReactiveProperty<bool>();
    Vector3 bearPosRage;
    ReactiveProperty<bool> gameStarted = new ReactiveProperty<bool>();
    public Transform spawnPointsParent;
    List<Transform> spawnPoints=new List<Transform>();
    public Material[] shieldMaterials;
    bool levelAdded;
    bool stillOnMode;
    private void Awake()
    {
        FightModel.fightObserveObj= fightObsObj;
        currentLevel = FightModel.currentPlayerLevel;
        setGameLevel(currentLevel);
        if (instance != null)
            Destroy(this);
        else
            instance = this;

    }

   

    // Start is called before the first frame update
    void Start()
    {
        

        FightModel.canTakeDamage = false;
        bear = FightModel.currentBear;
        FightModel.rageModeValue.Value = 0;
        FighterView.instance.intilize(false);
        FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearCinematicMode;
        FightModel.currentFightStatus.Value = FightModel.fightStatus.OnEnterCave;
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        Observable.Timer(TimeSpan.Zero)
                                  //.Do(_ => cinematicView.instance.setCamera(true, 0))
                                  .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                 .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                 .Do(_ => FightModel.currentPlayer.transform.position = new Vector3(0, 0, -15))
                                 .Do(_ => FightModel.currentPlayer.transform.eulerAngles = new Vector3(0, 0, 0))
                                 // .Delay(TimeSpan.FromSeconds(cinematicTime))
                                 //  .Do(_ => cinematicView.instance.setCamera(false, 0))
                                 // .Do(_ => ResetCamera())
                                 .Do(_=> setMoving(false))
                                 .Delay(TimeSpan.FromSeconds(4))
                                 .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                 .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                 .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Do(_ => gameStarted.Value = true)
                                .Do(_ => observeTimerAndRage())
                                .Delay(TimeSpan.FromMilliseconds(1000))
                                .Do(_=>FightModel.canTakeDamage=true)
                                .Do(_ => setMoving(FightModel.fightObserveObj))

                          .Subscribe()
                          .AddTo(this);
        observeFightStatus();
        observePlayerAndBearHealth();
        observeBearShield();
        bear = FightModel.currentBear;
        FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks(true);
        int children = spawnPointsParent.childCount;
        if (children > 0)
        {
            for (int i = 0; i < children; ++i)
                spawnPoints.Add(spawnPointsParent.GetChild(i));
        }
  

    }
    void ResetCamera()
    {
        FightModel.currentPlayer.transform.LookAt(FightModel.currentBear.transform);
        FightModel.playerVirtualCamera.enabled = false;
        FighterView.instance.lookAt.LookAt(FightModel.currentBear.transform);
        FightModel.playerVirtualCamera.transform.SetPositionAndRotation(FighterView.instance.playerBackCamera.position, FighterView.instance.playerBackCamera.rotation);
        FightModel.playerVirtualCamera.enabled = true;
    }
    void observeTimerAndRage()
    {

        timerOn
            .Where(_ => FightModel.gameTime.Value > 0)
            .Where(_=> gameStarted.Value)
            .Where(_=>_)
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_=>FightModel.gameTime.Value--)
            .Do(_=> timerOn.Value=false)
            .DelayFrame(1)
                        .Do(_ => timerOn.Value = true)

            .Subscribe()
                    .AddTo(FightModel.fightObserveObj);
        FightModel.rageModeValue
          .Where(_=> gameStarted.Value)
          .Where(_=>!stillOnMode)
          .Do(_ => bearAnger.fillAmount = _)
          .Where(_ => _ == 0 || _ == 1)
                      .Do(_ => setGameModeFromHealth(_))
                      .Subscribe()
         .AddTo(FightModel.fightObserveObj);

    }
    void setGameModeFromHealth(float bearRage)
    {
        int ran = UnityEngine.Random.Range(2, 4);
        if(FightModel.lastRand== ran)
        {
            if (ran == 3)
            {
                ran = 2;
                if (bearRage == 0)
                {
                    setFightMode(1);
                    FightModel.bearIsStunned = false;
                    return;
                }
                else if (bearRage >= 1)
                {
                    setFightMode(ran);

                }
                FightModel.lastRand = ran;
                return;
            }
            else 
            {
                ran = 3;
                if (bearRage == 0)
                {
                    setFightMode(1);
                    FightModel.bearIsStunned = false;
                    return;
                }
                else if (bearRage >= 1)
                {
                    setFightMode(ran);

                }
                FightModel.lastRand = ran;
                return;
            }
        }
        else
        {
            if (bearRage == 0)
            {
                setFightMode(1);
                FightModel.bearIsStunned = false;
                return;
            }
            else if (bearRage >= 1)
            {
                setFightMode(ran);

            }
            FightModel.lastRand = ran;
        }
    }
        void initilizeBeardistanceAttack(bool state)
    {

        if (!state)
        {
            currentBearShieldHealth.Value = FightModel.bearShielHealth;
            currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
            ThowringSpheres.SetActive(state);
            distanceFightEffects.SetActive(state);
            bearShield.transform.localScale = Vector3.zero;//new line added to reduce pusch from shield
            bearShield.SetActive(state);
            bearPosRage = bear.transform.position;
            bearShield.transform.position = new Vector3(bear.transform.position.x, bearShield.transform.position.y, bear.transform.position.z);
            distanceFightEffects.transform.position = bear.transform.position;
        }
        else
        {
            bearPosRage = bear.transform.position;
            bearShield.transform.position = new Vector3(bear.transform.position.x, bearShield.transform.position.y, bear.transform.position.z);
            distanceFightEffects.transform.position = bear.transform.position;
            currentBearShieldHealth.Value = FightModel.bearShielHealth;
            currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
            float timeToWait = cinematicTime +0.5f;
            Observable.Timer(TimeSpan.Zero)

                                          .Delay(TimeSpan.FromSeconds(1))
                                          .Do(_ => bearShield.SetActive(true))
                                          .Do(_ => bearShield.transform.DOScale(new Vector3(9.8f, 9.8f, 9.8f), 0.5f))  // new line added to reduce pus fom shield size was 9.8
                                          .Delay(TimeSpan.FromSeconds(timeToWait))
                                          .Do(_ => activateAfterDelay())
                                          .Subscribe()
                                          .AddTo(FightModel.fightObserveObj);
        }
       
    }
    void activateAfterDelay()
    {
        
        ThowringSpheres.SetActive(true);
        distanceFightEffects.SetActive(true);      
    }
    Vector3 GetMostFarPosFromBear()
    {
        float d = 0;
        Vector3 target = spawnPoints[0].position;
        foreach (Transform t in spawnPoints)
        {
            if (Vector3.Distance(FightModel.currentBear.transform.position, t.position) > d)
            {
                target = t.position;
                d = Vector3.Distance(FightModel.currentBear.transform.position, t.position);
            }
        }
        return target;
    }
    void observeBearShield()
    {
        bearShield.OnTriggerEnterAsObservable()
            .Where(_ => _.CompareTag("ThrowRocks"))
            .Do(_ => currentBearShieldHealth.Value--)
            .Do(_=>bearShield.GetComponent<MeshRenderer>().material=shieldMaterials[currentBearShieldHealth.Value])
            .Do(_=> _.gameObject.GetComponent<MeshRenderer>().enabled=false)
            .Delay(TimeSpan.FromMilliseconds(100))
            .Do(_ => _.gameObject.GetComponent<MeshRenderer>().enabled = true)
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeShieldDamage)
            .Subscribe()
            .AddTo(bearShield);
        bear.OnTriggerEnterAsObservable()
                    .Where(_ => bearShield.activeSelf==false)
                    .Where(_ => _.CompareTag("ThrowRocks"))
                    .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage)
                    .Where(_=> FightModel.currentFightStatus.Value == FightModel.fightStatus.OnRangeDistanceFight)
                    .Do(_ => currentBearHealthDistance.Value--)
                    .Do(_ => _.gameObject.GetComponent<MeshRenderer>().enabled = false)
                    .Delay(TimeSpan.FromMilliseconds(100))
                    .Do(_ => _.gameObject.GetComponent<MeshRenderer>().enabled = true)
                    .Subscribe()
                    .AddTo(FightModel.bearObserveObj);
        currentBearHealthDistance
            .Where(_ => _ < 1)
            .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight)
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearKnokedShortly)
            .Do(_ => FightModel.bearIsStunned = true)
            .Do(_=> ThowringSpheres.SetActive(false))
            .Do(_=> distanceFightEffects.SetActive(false))
            .Delay(TimeSpan.FromSeconds(FightModel.bearStunnedDuration))
            .Do(_ => bear.GetComponent<Animator>().SetBool("IsStunned", false))
            .Do(_ => FightModel.rageModeValue.Value = 0)
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);
        currentBearShieldHealth
            .Do(_=> desShield(_))
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);
        this.UpdateAsObservable()
            .Where(_=> FightModel.currentFightStatus.Value != FightModel.fightStatus.OnStartCenimatic && FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon&& FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
            .Do(_=> setBearPosFromRage())
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);
        isInDistanceRage
            .Where(_=>_==false)
            .Where(_ => FightModel.currentFightStatus.Value != FightModel.fightStatus.OnStartCenimatic && FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
            .Where(_ => FightModel.currentFightMode == 1)
            .Do(_ => bear.GetComponent<Animator>().Play("Idle", 0))
            .Do(_ => bear.GetComponent<Animator>().SetBool("IsIdle", true))
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);

    }
    void setBearPosFromRage()
    {
        if (isInDistanceRage.Value)
        {
            if (bear.transform.position != bearPosRage)
            {
                bear.transform.position = bearPosRage;
            }
            if (FighterView.instance != null)
            {
                if (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearKnokedShortly)
                {
                    if (!bearView.instance.anim.GetCurrentAnimatorStateInfo(0).IsName("DistanceAttackBear"))
                    {
                        bearView.instance.anim.Play("DistanceAttackBear");
                    }
                    if (FighterView.instance.playerAnimator.GetInteger("comboCounter") != 0)
                    {
                        FighterView.instance.comboValue.Value = 0;
                        FighterView.instance.currentState = 0;
                        FighterView.instance.playerAnimator.SetInteger("comboCounter", 0);
                        FighterView.instance.playerAnimator.SetBool("Fight", false);

                    }
                }
            }
        }
    }
    void observePlayerAndBearHealth()
    {
        if (currentLevel == 0)
        {
            FightModel.currentPlayerHealth.Value = FightModel.playerStartHealth;
        }
        FightModel.currentBearHealth.Value = FightModel.bearStartHealth;
        FightModel.currentPlayerHealth
            .Do(_ => playerHelthDisplay.fillAmount = _ / FightModel.playerStartHealth)
            .Where(_ => _ <= 0)
            .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerDead)
            .Do(_=> FightModel.currentFightStatus.Value=FightModel.fightStatus.OnFightLost)
            .Do(_=> FightModel.currentFightMode = 1)
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
            .Delay(TimeSpan.FromMilliseconds(3000))
            .Do(_=>Time.timeScale=0)
            .Do(_=> gameplayView.instance.isPaused = true)
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);
        FightModel.currentBearHealth
            .Do(_ => bearHealth.fillAmount = _ / FightModel.bearStartHealth)
            .Where(_ => _ <= 0)
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead)
            .Do(_ => bearShield.SetActive(false)) // added to disable shield if it stays on.
            .Do(_ => door.gameObject.SetActive(true))
            .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnFightWon)
            .Delay(TimeSpan.FromMilliseconds(4000))
            .Do(_=> endGame(true))
            .Subscribe()
            .AddTo(FightModel.fightObserveObj);
        FightModel.fightStatusValue
           .Where(_ => canChangeStatus)
           .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnChangeState)
           .Subscribe()
           .AddTo(FightModel.fightObserveObj);
   
      

    }
    public void endGame(bool winState)
    {

        if (winState)
        {
            gate.SetActive(true);
            gate.OnTriggerEnterAsObservable()
                .Where(_ => _.CompareTag("Player"))
                .Do(_ => bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGoToNextLevel)
                .Do(_ => SceneManager.UnloadSceneAsync(bearGameModel.singlePlayerScene3.sceneName))
                .Subscribe()
                .AddTo(FightModel.fightObserveObj);
            if (door == null)
            {
                door = GameObject.FindObjectOfType<SlidingDoor>();
            }
            if(currentLevel==4)
            {
                FightModel.gameScore.Value+=100;
                GameUIView.instance.EnableGameOver(3f, "You defeated the bears.\n\nYou get 100 bonus points");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
                FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
            }
               
            else 
                door.Invoke("OpenDoor",1f);
            FighterView.instance.initilized = false;

        }
        else
        {
            FighterView.instance.initilized = false;
        }
    }
    void hideRocks()
    {
        hittedRocks.transform.DOMove(new Vector3(hittedRocks.transform.position.x, -15, hittedRocks.transform.position.z), 4f).OnComplete(() =>
        {
            door.OpenDoor();
            hittedRocks.gameObject.SetActive(false);
        });
    }
    void desShield(int v)
    {
        if (v < 1)
        {
            if (bearShield != null)
            {
                bearShield.transform.localScale = Vector3.zero; // added to reduce shiel push
                bearShield.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            setFightMode(1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            setFightMode(2);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            setFightMode(3);
        }
    }
    void initilizedGame()
    {
        FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 7f;
        FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 10.5f;
        FighterView.instance.playerAnimator.SetBool("Dead", false);
        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerCinematicMode;
    }
    void observeFightStatus()
    {
       

        FightModel.currentFightStatus
             .Subscribe(procedeFight)
               .AddTo(FightModel.fightObserveObj);

        void procedeFight(FightModel.fightStatus status)
        {
            switch (status)
            {
                case FightModel.fightStatus.OnEnterCave:
                    

                    break;
                case FightModel.fightStatus.OnPath:
                    initilizedGame();
                    break;
                case FightModel.fightStatus.OnStartFight:


                    break;
                case FightModel.fightStatus.OnCloseDistanceFight:
                    initilizeBeardistanceAttack(false);
                    break;
                case FightModel.fightStatus.OnRangeDistanceFight:
                    initilizeBeardistanceAttack(true);

                    break;
                case FightModel.fightStatus.OnRangeDistanceFightWon:


                    break;
                case FightModel.fightStatus.OnRunFromRageBear:
                    initilizeBeardistanceAttack(false);


                    break;
                case FightModel.fightStatus.OnBearHitRock:


                    break;
                case FightModel.fightStatus.OnLastFightScene:


                    break;
                case FightModel.fightStatus.OnFightWon:
                    gameStarted.Value = false;
                    FightModel.gameScore.Value += 20;
                    FightModel.gameTime.Value += 25;
                    if (!levelAdded)
                    {
                        FightModel.currentPlayerLevel += 1;
                        levelAdded = true;
                    }
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 7f;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 10.5f;
                    FightModel.currentPlayer.GetComponent<RockThrowView>().throwRockWon(1000f,0.5f);
                    Observable.Timer(TimeSpan.Zero)  
                                         .Delay(TimeSpan.FromSeconds(2f))
                                         .Do(_ => Destroy(FightModel.currentBear.GetComponent<fightView>()))
                                         .Subscribe()
                                         .AddTo(FightModel.fightObserveObj);
                    break;
                case FightModel.fightStatus.OnFightLost:
                    FightModel.currentPlayer.GetComponent<RockThrowView>().throwRockWon(1000f, 0.5f);
                    gameStarted.Value = false;
                    FightModel.currentPlayerLevel = 0;
                    FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerDead;
                    //FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearWon;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 0f;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 0f;
                    GameUIView.instance.EnableGameOver(3f,"WASTED");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
                    FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
                    break;
                case FightModel.fightStatus.OnTimeUp:


                    break;
                case FightModel.fightStatus.OnChangeState:
                    Observable.Timer(TimeSpan.Zero)
                                          .Do(_ => initilizeBeardistanceAttack(false))
                                          .Do(_ => setFightMode(FightModel.fightStatusValue.Value))
                                          .Do(_ => canChangeStatus = false)
                                          .Delay(TimeSpan.FromSeconds(10f))
                                          .Do(_ => canChangeStatus = true)
                                          .Subscribe()
                                          .AddTo(FightModel.fightObserveObj);
                    break;


            }
        }
       
    }
   
   void setMoving(bool state)
    {
        if (!state)
        {
            FighterView.instance.playerAnimator.SetFloat("Speed",0);
            FighterView.instance.playerAnimator.Play("Idle");
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 0f;
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 0f;
        }
        else
        {
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 7f;
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 10.5f;
        }
    }
  
    public void setFightMode(int i)
    {

        switch (i)
        {
            case 1 :
                    FightModel.currentFightMode = 1;
                    FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                isInDistanceRage.Value = false;
                if (gameStarted.Value)
                {
                    bearView.instance.setAnimtor(1);
                }


                break;
            case 2:
                stillOnMode = true;
                FightModel.currentFightMode = 2;
                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnRangeDistanceFight;
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDistanceAttacking;
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
                FighterView.instance.comboValue.Value = 0;
                isInDistanceRage.Value = true;
                bearView.instance.setAnimtor(2);

                Observable.Timer(TimeSpan.Zero)
                                //.Do(_ => cinematicView.instance.setCamera(true, 2))
                                //.Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                //.Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                //.Do(_ => FightModel.currentPlayer.transform.position = GetMostFarPosFromBear())
                                //.Delay(TimeSpan.FromSeconds(1))
                                //.Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                //.Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                //.Delay(TimeSpan.FromSeconds(cinematicTime))
                                //.Do(_ => cinematicView.instance.setCamera(false, 0))
                                //.Do(_ => ResetCamera())
                                //.Delay(TimeSpan.FromSeconds(3))
                                //.Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Delay(TimeSpan.FromSeconds(1))
                                .Do(_ => observeFightStatus())
                                .Delay(TimeSpan.FromSeconds(15))
                                .Do(_=> stillOnMode = false)
                                .Where(_ => FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon && FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
                                .Do(_=> setFightMode(1))
                          .Subscribe()
                          .AddTo(FightModel.fightObserveObj);
                break;
            case 3:
                stillOnMode = true;
                FightModel.currentFightMode = 3;
                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                isInDistanceRage.Value = false;
                bearView.instance.setAnimtor(3);
                Observable.Timer(TimeSpan.Zero)
                                //.Do(_ => cinematicView.instance.setCamera(true, 1))
                                // .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                //.Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                //.Do(_ => FightModel.currentPlayer.transform.position = GetMostFarPosFromBear())
                                //.Delay(TimeSpan.FromSeconds(1))
                                //.Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                //.Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                //.Delay(TimeSpan.FromSeconds(cinematicTime-1))
                                //.Do(_ => cinematicView.instance.setCamera(false, 0))
                                //.Do(_ => ResetCamera())
                                //.Delay(TimeSpan.FromSeconds(3))
                                //.Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Delay(TimeSpan.FromSeconds(1f))
                                .Do(_ => observeFightStatus())
                                .Delay(TimeSpan.FromSeconds(20))
                                .Do(_ => stillOnMode = false)
                                .Where(_ => FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon && FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
                                .Do(_ => setFightMode(1))
                                .Do(_ => bearView.instance.anim.SetBool("ForceIdle", true))
                          .Subscribe()
                          .AddTo(FightModel.fightObserveObj);
                break;
        }
        for (int j = 0; j < modesStatus.modes.Length; j++)
        {
            if (j == i)
            {
                modesStatus.modes[j] = true;

            }
            else
            {
                modesStatus.modes[j] = false;

            }
        }
    }
    void setGameLevel(int level)
    {
        FightModel.bearStartHealth = 100 + (25 * level);
        FightModel.bearCloseHitValue = 5+(level*5);
        FightModel.bearDistanceHitValue = 5+(level*5);
    }
    public void startGame()
    {
        FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearCinematicMode;
        FightModel.currentFightStatus.Value = FightModel.fightStatus.OnEnterCave;
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        ThowringSpheres.SetActive(true);
        hittedRocks.SetActive(true);
        bearHealth = GameUIView.instance.bearHealth;
        playerHelthDisplay = GameUIView.instance.playerHelthDisplay;
        bearAnger = GameUIView.instance.bearAgression;
        bear = FightModel.currentBear;
        bear.GetComponent<Animator>().Play("Idle", 0);
        GameUIView.instance.fightCanvas.SetActive(true);
        FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks(true);
        gate.SetActive(false);
    }
}


