using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using StarterAssets;

public class FighterView : MonoBehaviour
{
    Animator playerAnimator;
    ThirdPersonController playerController;
    public FightModel.PlayerFightModes currentState;
    FightModel.PlayerFightModes desState;
    // Start is called before the first frame update
    ReactiveProperty<float> currentBearShieldHealth = new ReactiveProperty<float>();
    ReactiveProperty<float> currentBearHealthDistance = new ReactiveProperty<float>();

    float[] comboTimes = new float[8] { 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.1f, 0.15f, 0.15f };
    int[] comboHitTimes = new int[8] { 1, 3, 4, 3, 4, 3, 5, 4 };

    public ReactiveProperty<bool> canHitAgainCombo = new ReactiveProperty<bool>();
    public ReactiveProperty<int> comboValue = new ReactiveProperty<int>();
    ReactiveProperty<bool> isComboIdle = new ReactiveProperty<bool>();
    public GameObject detectionCollider;
    public GameObject hittedPannel;
    [SerializeField]
    bool canChangeStatus;
    private void Awake()
    {
        FightModel.currentPlayer = gameObject;

    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerController = GetComponent<ThirdPersonController>();
        observeFighterStatus();
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        canHitAgainCombo.Value = true;
        observePlayerCombo();
        FightModel.currentCombo = FightModel.comboNames.Idle;
        observePlayerHittedDistance();
    }

    // Update is called once per frame
    void Update()
    {
        observePlayerPunch();
        observePlayerBlock();
        isComboIdle.Value = playerAnimator.GetCurrentAnimatorStateInfo(1).IsName("Idle");

    }
    void observePlayerCombo()
    {
        canHitAgainCombo
                        .Where(_ => FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
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
            .Where(_ => FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerDead)
            .Where(_ => FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
            .Where(_ => _ == true)
            .DelayFrame(1)
            .Where(_ => _ == true)
            .Do(_ => resetCombo())
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
            .Subscribe()
            .AddTo(this);
    }
    void resetCombo()
    {
        comboValue.Value = 0;
        playerAnimator.SetBool("Fight", false);
        playerAnimator.SetInteger("comboCounter", 0);
        FightModel.currentCombo = FightModel.comboNames.Idle;
        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
    }
    IEnumerator comboHitBear(float waitTime, int combosCount)
    {
        for (int i = 0; i < combosCount; i++)
        {
            yield return new WaitForSeconds(waitTime / combosCount);
            onHitBear();
        }
    }
    void onHitBear()
    {
        if (Vector3.Distance(transform.position, FightModel.currentBear.transform.position) < FightModel.shortAttackRangeValue)
        {


            FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage;


        }
    }
    void observeFighterStatus()
    {
        FightModel.currentPlayerStatus
             .Subscribe(procedeFighter)
               .AddTo(this);

        void procedeFighter(FightModel.PlayerFightModes status)
        {
            switch (status)
            {
                case FightModel.PlayerFightModes.playerIdle:
                    playerAnimator.SetBool("isIdle", true);
                    playerAnimator.SetBool("Block", false);
                    playerController.MoveSpeed = 7f;
                    playerController.SprintSpeed = 10.5f;
                    Observable.Timer(TimeSpan.Zero)
                                           .DelayFrame(1)
                                           .Do(_ => playerAnimator.SetBool("isIdle", false))
                                           .Subscribe()
                        .AddTo(this);
                    break;
                case FightModel.PlayerFightModes.playerTakeDamage:
                    if ((FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                    {
                        playerAnimator.SetTrigger("Hit");
                        Observable.Timer(TimeSpan.Zero)
                               .DelayFrame(1)
                               .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                               .Where(_ => FightModel.currentPlayerHealth.Value > 0)
                               .Do(_ => FightModel.currentPlayerHealth.Value -= damageFromMode(FightModel.currentFightMode))
                               .Do(_ => hittedPannel.SetActive(true))
                               .Delay(TimeSpan.FromMilliseconds(400))
                               .Do(_ => hittedPannel.SetActive(false))
                               .Subscribe()

                            .AddTo(this);
                    }
                       
                    break;

                case FightModel.PlayerFightModes.playerCombo:
                    playerController.MoveSpeed = 0.5f;
                    playerController.SprintSpeed = 1f;
                    break;
                case FightModel.PlayerFightModes.playerDead:
                    playerAnimator.SetBool("Dead", true);
                    break;
                case FightModel.PlayerFightModes.playerBlockShortAttack:
                    playerController.MoveSpeed = 0.5f;
                    playerController.SprintSpeed = 1f;
                    playerAnimator.SetBool("Block", true);
                    break;
            }
        }
    }
    void observePlayerPunch()
    {
        if(FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight)
                {
                    if (canHitAgainCombo.Value)
                    {
                        if (comboValue.Value < comboTimes.Length - 1)
                        {
                            if (checkCanGoNextCombo())
                            {
                                comboValue.Value++;
                                canHitAgainCombo.Value = false;
                                FightModel.currentCombo = (FightModel.comboNames)comboValue.Value;
                                FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerCombo;
                            }
                        }
                        setComboInAnimator();
                    }
                }

            }
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
            playerAnimator.SetBool("Fight", true); ;
            playerAnimator.SetInteger("comboCounter", comboValue.Value);
        }

    }
    bool checkCanGoNextCombo()
    {
        bool r = false;

        if (playerAnimator.GetCurrentAnimatorStateInfo(1).IsName(((FightModel.comboNames)comboValue.Value).ToString()))
        {
            r = true;

        }
        else
        {
            r = false;
        }

        return r;
    }
    void observePlayerBlock()
    {
        if (FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerDead)
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
    }
    void observePlayerHittedDistance()
    {
        detectionCollider.OnTriggerEnterAsObservable()
            .Where(_ => _.transform.CompareTag("distanceAttack"))
            .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerTakeDamage)
            .Subscribe()
            .AddTo(this);
    }
    float damageFromMode(int mode)
    {
        float m = 1;
        if (mode == 1)
        {
            m = FightModel.bearCloseHitValue;
        }
        else if (mode == 2)
        {
            m = FightModel.bearDistanceHitValue;
        }
        else if (mode == 3)
        {
            m = FightModel.bearDistanceHitValue;
        }
        else if (mode == 4)
        {
            m = FightModel.bearDistanceHitValue;
        }
        return m;
    }
}
