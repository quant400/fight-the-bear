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
    public GameObject player;
    public Animator playerAnimator;
    public Animator bearAnimator;
    public GameObject bearShield;
    public GameObject ThowringSpheres;
    public GameObject distanceFightEffects;

    ReactiveProperty<float> currentBearShieldHealth = new ReactiveProperty<float>();
    ReactiveProperty<float> currentBearHealthDistance = new ReactiveProperty<float>();

    float[] comboTimes = new float[8] { 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.1f, 0.15f, 0.15f };
    int[] comboHitTimes = new int[8] { 1, 3, 4, 3, 4, 3, 5, 4 };

    comboNames currenCombo;
    public ReactiveProperty<bool> canHitAgainCombo = new ReactiveProperty<bool>();
    public ReactiveProperty<int> comboValue = new ReactiveProperty<int>();
    ReactiveProperty<bool> isComboIdle = new ReactiveProperty<bool>();
    [SerializeField]
    Image bearHealth, playerHelthDisplay;
    bool canChangeStatus;
    public class modesetted
    {
        public bool[] modes = new bool[3] { false, false, false };
    }
    public modesetted modesStatus = new modesetted();
    // Start is called before the first frame update
    void Start()
    {
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        canHitAgainCombo.Value = true;
        observeFightStatus();
        observePlayerAndBearHealth();
        observeBearShield();
        observePlayerCombo();
        currenCombo = comboNames.Idle;
    }
    void setGameModeFromHealth(float bearHealt)
    {
        int ran = UnityEngine.Random.Range(2, 3);

        if ((bearHealt <= 125) && (bearHealt >= 110))
        {
            FightModel.fightStatusValue.Value = 1;
            return;
        }
        else if ((bearHealt < 110) && (bearHealt > 90))
        {
            FightModel.fightStatusValue.Value = ran;
            return;


        }
        else if ((bearHealt <= 90) && (bearHealt >= 70))
        {
            FightModel.fightStatusValue.Value = 1;
            return;

        }
        else if ((bearHealt < 70) && (bearHealt > 60))
        {
            FightModel.fightStatusValue.Value = ran;
            return;
        }
        else if ((bearHealt <= 60) && (bearHealt >= 40))
        {
            FightModel.fightStatusValue.Value = 1;
            return;
        }
        else if ((bearHealt < 40) && (bearHealt > 30))
        {
            FightModel.fightStatusValue.Value = ran;
            return;
        }
        else if ((bearHealt <= 30) && (bearHealt >= 10))
        {
            FightModel.fightStatusValue.Value = 1;
            return;

        }
        else if ((bearHealt < 10) && (bearHealt > 0))
        {
            FightModel.fightStatusValue.Value = ran;
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
                    .Where(_ => currentBearShieldHealth.Value < 1)
                    .Where(_ => _.CompareTag("ThrowRocks"))
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
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearRageMode)
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
            .Subscribe()
            .AddTo(this);
        FightModel.currentBearHealth
            .Do(_=> setGameModeFromHealth(_))
            .Do(_ => bearHealth.fillAmount = _ / FightModel.bearStartHealth)
            .Subscribe()
            .AddTo(this);
        FightModel.fightStatusValue
           .Where(_ => canChangeStatus)
           .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnChangeState)
           .Subscribe()
           .AddTo(this);
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
        observePlayerPunch();
        observePlayerBlock();
        isComboIdle.Value = playerAnimator.GetCurrentAnimatorStateInfo(1).IsName("Idle");
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


                    break;
                case FightModel.fightStatus.OnFightLost:


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
    void observePlayerCombo()
    {
        canHitAgainCombo
                        .Where(_=> FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
                        .Where(_ => _ == false)
                        .Delay(TimeSpan.FromSeconds(comboTimes[comboValue.Value] / 2))
                        .Do(_ => StartCoroutine(comboHitBear(0.5f, comboHitTimes[comboValue.Value])))
                        .Delay(TimeSpan.FromSeconds(comboTimes[comboValue.Value] / 2))
                        .Do(_ => canHitAgainCombo.Value = true)
                        .Delay(TimeSpan.FromSeconds(1))
                        .Where(_ => comboValue.Value == comboTimes.Length)
                        .Delay(TimeSpan.FromSeconds(1))
                        .Do(_ => resetCombo())
                        .Subscribe()
                        .AddTo(this);
        isComboIdle
            .Where(_ => FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
            .Where(_ => _ == true)
            .DelayFrame(1)
            .Where(_ => _ == true)
            .Do(_ => resetCombo())
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
            .Subscribe()
            .AddTo(this);
    }
    IEnumerator comboHitBear(float waitTime, int combosCount)
    {
        for ( int i =0; i < combosCount; i++)
        {
            yield return new WaitForSeconds(waitTime / combosCount);
            onHitBear();
        }
    }
    void observePlayerPunch()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
            {
                if (canHitAgainCombo.Value)
                {
                    if (comboValue.Value < comboTimes.Length-1)
                    {
                        if (checkCanGoNextCombo())
                        {
                            comboValue.Value++;
                            canHitAgainCombo.Value = false;
                            currenCombo = (comboNames)comboValue.Value;
                            FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerCombo;
                        }
                    }
                    setComboInAnimator();
                }
            }
            
        }
       

    }
    void observePlayerBlock()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
            {
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerBlockShortAttack;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
            {
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
            }
        }
    }
    void onHitBear()
    {
        if (Vector3.Distance(player.transform.position, bear.transform.position) < FightModel.shortAttackRangeValue)
        {
            
                
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage;

            
        }
    }
    void setComboInAnimator()
    {
        if (comboValue.Value > comboTimes.Length)
        {
            canHitAgainCombo.Value = false;
            
            return;
        }
        else
        {
            playerAnimator.SetBool("Fight", true);;
            playerAnimator.SetInteger("comboCounter", comboValue.Value);
        }

    }
    void resetCombo()
    {
        comboValue.Value = 0;
        playerAnimator.SetBool("Fight", false);
        playerAnimator.SetInteger("comboCounter", 0);
        currenCombo = comboNames.Idle;
        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
    }
    bool checkCanGoNextCombo()
    {
        bool r=false;
        
            if (playerAnimator.GetCurrentAnimatorStateInfo(1).IsName(((comboNames)comboValue.Value).ToString()))
            {
                r = true;

            }
            else
            {
                r = false;
            }

        return r;
    }
    void checkComboDoneOrInterepted()
    {
        if(playerAnimator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
        {
            resetCombo();
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

                break;
            case 2:
                FightModel.currentFightMode = 2;

                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnRangeDistanceFight;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearDistanceAttacking;
                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
                break;
            case 3:
               
                    FightModel.currentFightMode = 3;
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
}
