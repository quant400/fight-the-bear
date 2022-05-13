using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;

public class fightView : MonoBehaviour
{
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
    ReactiveProperty<float> currentBearShieldHealth = new ReactiveProperty<float>();
    ReactiveProperty<float> currentBearHealthDistance = new ReactiveProperty<float>();


    [SerializeField]
    Image bearHealth, playerHelthDisplay,bearAnger;
    bool canChangeStatus;
    public class modesetted
    {
        public bool[] modes = new bool[3] { false, false, false };
    }
    public modesetted modesStatus = new modesetted();
    public int currentLevel;
    private void Awake()
    {
        setGameLevel(currentLevel);

    }
    // Start is called before the first frame update
    void Start()
    {
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        observeFightStatus();
        observePlayerAndBearHealth();
        observeBearShield();
        bear = FightModel.currentBear;
        FightModel.currentPlayer.GetComponent<RockThrowView>().findRocks();
    }
    void setGameModeFromHealth(float bearHealt)
    {
        int ran = UnityEngine.Random.Range(2, 4);

        if ((bearHealt <= 1) && (bearHealt >= 0.85f))
        {
            FightModel.fightStatusValue.Value = 1;
            return;
        }
        else if ((bearHealt <0.85f) && (bearHealt > 0.75f))
        {
            FightModel.fightStatusValue.Value = ran;
            return;


        }
        else if ((bearHealt <=  0.75f) && (bearHealt >= 0.55f))
        {
            FightModel.fightStatusValue.Value = 1;
            return;

        }
        else if ((bearHealt <  0.55f) && (bearHealt > 0.45f))
        {
            FightModel.fightStatusValue.Value = ran;
            return;
        }
        else if ((bearHealt <=  0.45f) && (bearHealt >=  0.25f))
        {
            FightModel.fightStatusValue.Value = 1;
            return;
        }
        else if ((bearHealt <  0.25f) && (bearHealt > 0.15f))
        {
            FightModel.fightStatusValue.Value = ran;
            return;
        }
        else if ((bearHealt <= 0.15f) && (bearHealt > 0))
        {
            FightModel.fightStatusValue.Value = 1;
            return;

        }
       
       
    }
        void initilizeBeardistanceAttack(bool state)
    {
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        ThowringSpheres.SetActive(state);
        distanceFightEffects.SetActive(state);
        bearShield.SetActive(state);
        bearShield.transform.position = new Vector3(bear.transform.position.x, bearShield.transform.position.y, bear.transform.position.z);
        distanceFightEffects.transform.position = bear.transform.position;

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
            .Do(_ => setFightMode(1))
            .Do(_ => bear.GetComponent<Animator>().SetBool("IsStunned", false))
            .Subscribe()
            .AddTo(this);
        currentBearShieldHealth
            .Do(_=> desShield(_))
            .Subscribe()
            .AddTo(this);

     
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
            .Do(_ => setGameModeFromHealth(bearHealth.fillAmount))
            .Where(_=>_<=0)
            .Do(_=> FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead)
            .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnFightWon)
            .Delay(TimeSpan.FromMilliseconds(3000))
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
            door.OpenDoor();
            hittedRocks.gameObject.SetActive(false);
        }
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

                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDead;
                    break;
                case FightModel.fightStatus.OnFightLost:
                    FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerDead;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;

                    break;
                case FightModel.fightStatus.OnTimeUp:


                    break;
                case FightModel.fightStatus.OnChangeState:
                    Debug.Log("set to next");
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
                bearAnger.fillAmount = 0.1f;
                    FightModel.currentFightMode = 1;
                    FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;

                break;
            case 2:
                FightModel.currentFightMode = 2;
                bearAnger.fillAmount = 0.5f;

                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnRangeDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDistanceAttacking;
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
                break;
            case 3:
               
                    FightModel.currentFightMode = 3;
                bearAnger.fillAmount = 1f;

                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                
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
        if (level != 0)
        {
            FightModel.playerCloseHitValue = (6 / level) + 2;
            FightModel.playerDistanceHitValue = (8 / level) + 2;
        }
    }
}
