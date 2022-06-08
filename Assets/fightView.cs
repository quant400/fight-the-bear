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
    ReactiveProperty<float> currentBearShieldHealth = new ReactiveProperty<float>();
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


    private void Awake()
    {
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
        gameStarted.Value= true;
        FightModel.rageModeValue.Value = 0;
        FighterView.instance.intilize();
        FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearCinematicMode;
        FightModel.currentFightStatus.Value = FightModel.fightStatus.OnEnterCave;
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        Observable.Timer(TimeSpan.Zero)
                                .Do(_ => cinematicView.instance.setCamera(true, 0))
                                  .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                 .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                .Delay(TimeSpan.FromSeconds(cinematicTime))
                                 .Do(_ => cinematicView.instance.setCamera(false, 0))
                                 .Do(_=> FightModel.currentPlayer.transform.position= GetMostFarPosFromBear())
                                 .Delay(TimeSpan.FromSeconds(1))
                                 .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                 .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                 .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Do(_ => gameStarted.Value = true)
                                .Do(_ => observeTimerAndRage())
                          .Subscribe()
                          .AddTo(this);
        observeFightStatus();
        observePlayerAndBearHealth();
        observeBearShield();
        bear = FightModel.currentBear;
        FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks();
        int children = spawnPointsParent.childCount;
        if (children > 0)
        {
            for (int i = 0; i < children; ++i)
                spawnPoints.Add(spawnPointsParent.GetChild(i));
        }
  

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
                    .AddTo(this);
        FightModel.rageModeValue
            .Where(_=> gameStarted.Value)
          .Do(_ => bearAnger.fillAmount = _)
          .Where(_ => _ == 0 || _ == 1)
                      .Do(_ => setGameModeFromHealth(_))
                      .Subscribe()
         .AddTo(this);

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
                                          .Delay(TimeSpan.FromSeconds(timeToWait))
                                          .Do(_ => activateAfterDelay())
                                          .Subscribe()
                                          .AddTo(this);
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
            .Subscribe()
            .AddTo(bearShield);
        bear.OnTriggerEnterAsObservable()
                    .Where(_ => bearShield.activeSelf==false)
                    .Where(_ => _.CompareTag("ThrowRocks"))
                    .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage)
                    .Where(_=> FightModel.currentFightStatus.Value == FightModel.fightStatus.OnRangeDistanceFight)
                    .Do(_ => currentBearHealthDistance.Value--)
                    .Subscribe()
                    .AddTo(bear);
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
            .AddTo(this);
        currentBearShieldHealth
            .Do(_=> desShield(_))
            .Subscribe()
            .AddTo(this);
        this.UpdateAsObservable()
            .Where(_=> FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon&& FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
            .Do(_=> setBearPosFromRage())
            .Subscribe()
            .AddTo(this);
        isInDistanceRage
            .Where(_=>_==false)
            .Where(_ => FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon && FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost)
            .Where(_ => FightModel.currentFightMode == 1)
            .Do(_ => bear.GetComponent<Animator>().Play("Idle", 0))
            .Do(_ => bear.GetComponent<Animator>().SetBool("IsIdle", true))
            .Subscribe()
            .AddTo(this);

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
        FightModel.currentPlayerHealth.Value = FightModel.playerStartHealth;
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
            .Subscribe()
            .AddTo(this);
        FightModel.currentBearHealth
            .Do(_ => bearHealth.fillAmount = _ / FightModel.bearStartHealth)
            .Where(_=>_<=0)
            .Do(_=> FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead)
            .Do(_ => door.gameObject.SetActive(true))
            .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnFightWon)
            .Delay(TimeSpan.FromMilliseconds(4000))
            .Do(_=> endGame(true))
            .Subscribe()
            .AddTo(this);
        FightModel.fightStatusValue
           .Where(_ => canChangeStatus)
           .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnChangeState)
           .Subscribe()
           .AddTo(this);
   
      

    }
    public void endGame(bool winState)
    {

        if (winState)
        {
            gate.SetActive(true);
            gate.OnTriggerEnterAsObservable()
                .Where(_ => _.CompareTag("Player"))
                .Do(_ => SceneManager.LoadScene(1))
                .Subscribe()
                .AddTo(this);
            if (door == null)
            {
                door = GameObject.FindObjectOfType<SlidingDoor>();
            }
            door.OpenDoor();
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
    void desShield(float v)
    {
        if (v < 1)
        {
            if (bearShield != null)
            {
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
    void observeFightStatus()
    {
       

        FightModel.currentFightStatus
             .Subscribe(procedeFight)
               .AddTo(this);

        void procedeFight(FightModel.fightStatus status)
        {
            switch (status)
            {
                case FightModel.fightStatus.OnEnterCave:
                    

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

                    FightModel.currentPlayerLevel += 1;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 7f;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 10.5f;
                    FightModel.currentPlayer.GetComponent<RockThrowView>().throwRockWon(0.2f,0.5f);
                    break;
                case FightModel.fightStatus.OnFightLost:
                    gameStarted.Value = false;
                    FightModel.currentPlayerLevel = 0;
                    FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerDead;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 0f;
                    FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 0f;
                    UIController.instance.gameOverPanel.SetActive(true);
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
                                          .AddTo(this);
                    break;


            }
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


                break;
            case 2:
                FightModel.currentFightMode = 2;
                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnRangeDistanceFight;
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDistanceAttacking;
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
                FighterView.instance.comboValue.Value = 0;
                isInDistanceRage.Value = true;

                Observable.Timer(TimeSpan.Zero)
                                .Do(_ => cinematicView.instance.setCamera(true, 2))
                                .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                .Do(_ => FightModel.currentPlayer.transform.position = GetMostFarPosFromBear())
                                .Delay(TimeSpan.FromSeconds(1))
                                .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                .Delay(TimeSpan.FromSeconds(cinematicTime))
                                .Do(_ => cinematicView.instance.setCamera(false, 0))
                                .Delay(TimeSpan.FromSeconds(3))
                                .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Delay(TimeSpan.FromSeconds(1))
                                .Do(_ => observeFightStatus())
                                .Delay(TimeSpan.FromSeconds(15))
                                .Do(_=> setFightMode(1))
                          .Subscribe()
                          .AddTo(this);
                break;
            case 3:
               
                    FightModel.currentFightMode = 3;

                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                isInDistanceRage.Value = false;

                Observable.Timer(TimeSpan.Zero)
                                .Do(_ => cinematicView.instance.setCamera(true, 1))
                                 .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = false)
                                .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = false)
                                .Do(_ => FightModel.currentPlayer.transform.position = GetMostFarPosFromBear())
                                .Delay(TimeSpan.FromSeconds(1))
                                .Do(_ => FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().enabled = true)
                                .Do(_ => FightModel.currentPlayer.GetComponent<CharacterController>().enabled = true)
                                .Delay(TimeSpan.FromSeconds(cinematicTime-1))
                                .Do(_ => cinematicView.instance.setCamera(false, 0))
                                .Delay(TimeSpan.FromSeconds(3))
                                .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                                .Delay(TimeSpan.FromSeconds(1f))
                                .Do(_ => observeFightStatus())
                                .Delay(TimeSpan.FromSeconds(20))
                                .Do(_ => setFightMode(1))
                          .Subscribe()
                          .AddTo(this);
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
        FightModel.bearStartHealth = 125 + (10 * level);
        FightModel.bearCloseHitValue = 6+level;
        FightModel.bearDistanceHitValue = 10+(level*2);
        if (level >1)
        {
            FightModel.playerCloseHitValue = (6 / level) + 2;
            FightModel.playerDistanceHitValue = (8 / level) + 2;
        }
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
        bearHealth = UIController.instance.bearHealth;
        playerHelthDisplay = UIController.instance.playerHelthDisplay;
        bearAnger = UIController.instance.bearAgression;
        bear = FightModel.currentBear;
        bear.GetComponent<Animator>().Play("Idle", 0);
        UIController.instance.fightCanvas.SetActive(true);
        FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks();
        gate.SetActive(false);
    }
}
