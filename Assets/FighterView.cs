using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using StarterAssets;
using Cinemachine;
public class FighterView : MonoBehaviour
{
    public static FighterView instance;
    public Animator playerAnimator;
    ThirdPersonController playerController;
    public FightModel.PlayerFightModes currentState;
    FightModel.PlayerFightModes desState;
    // Start is called before the first frame update
    ReactiveProperty<float> currentBearShieldHealth = new ReactiveProperty<float>();
    ReactiveProperty<float> currentBearHealthDistance = new ReactiveProperty<float>();

    float[] comboTimes = new float[8] { 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f };
    int[] comboHitTimes = new int[8] { 1, 3, 4, 3, 4, 3, 5, 4 };

    public ReactiveProperty<bool> canHitAgainCombo = new ReactiveProperty<bool>();
    public ReactiveProperty<int> comboValue = new ReactiveProperty<int>();
    ReactiveProperty<bool> isComboIdle = new ReactiveProperty<bool>();
    ReactiveProperty<bool> IsDead = new ReactiveProperty<bool>();

    public GameObject detectionCollider;
    public GameObject hittedPannel;
    public ReactiveProperty<bool> bearHitted = new ReactiveProperty<bool>();
    public bool initilized;
    public GameObject playerCamera;
    public CinemachineBrain playerCameraBrain;
    public DamageDisplay DD;
    public CinemachineVirtualCamera playerVirtualCamera;
    public Transform playerBackCamera;
    public Transform lookAt;
    public GameObject cinematicCamera;
    public GameObject observerPath;

    [SerializeField]
    bool canChangeStatus;

    PlayerSFXController PlayerSFX;
    private void Awake()
    {
        FightModel.currentPlayer = gameObject;
        FightModel.playerCamera = playerCamera;
        FightModel.CinematicCamera = cinematicCamera;
        FightModel.playerCameraBrain = playerCameraBrain;
        FightModel.playerVirtualCamera = playerVirtualCamera;
        FightModel.offsetFromPlayer = FightModel.CinematicCamera.transform.position - lookAt.position;
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        PlayerSFX = GetComponent<PlayerSFXController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (initilized)
        {
            observePlayerPunch();
            observePlayerBlock();
            isComboIdle.Value = playerAnimator.GetCurrentAnimatorStateInfo(1).IsName("Idle");
        }
        if (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon)
        {

            if (playerAnimator.GetInteger("comboCounter") != 0)
            {
                comboValue.Value = 0;
                currentState = 0;
                playerAnimator.SetInteger("comboCounter", 0);
                playerAnimator.SetBool("Fight", false);

            }

        }
        


    }
    void observePlayerDead()
    {
        this.UpdateAsObservable()
            .Where(_ => !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            .Do(_ => playerAnimator.Play("Death"))
            .Do(_ => playerAnimator.SetBool("IsDead", true))
            .Where(_ => playerAnimator.GetBool("PickRock") == true)
            .Do(_ => playerAnimator.SetBool("PickRock", false))
            .Do(_ => playerAnimator.Play("Idle", 2))

            .Subscribe()
            .AddTo(fightView.instance);
        this.UpdateAsObservable()
           
            .Where(_ => FightModel.isHoldingRock)
            .Do(_ =>StartCoroutine( FightModel.currentPlayer.GetComponent<RockThrowView>().throwAndSetBackDelay(1000, 0.2f)))
            .Subscribe()
            .AddTo(fightView.instance);
    }
    void initilizeFighter()
    {
        Time.timeScale = 1;
        FightModel.currentPlayer.GetComponent<ThirdPersonController>().MoveSpeed = 7f;
        FightModel.currentPlayer.GetComponent<ThirdPersonController>().SprintSpeed = 10.5f;
        playerAnimator.SetBool("Dead", false);
        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerCinematicMode;
    }
    void observePlayerCombo(GameObject observer)
    {
        canHitAgainCombo
            
                        .Where(_ => (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnPath) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight) || (FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearKnokedShortly))
                        .Where(_=> !FightModel.isHoldingRock)
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
                        .AddTo(observer);
        isComboIdle
            .Where(_ => FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerDead)
            .Where(_ => !FightModel.isHoldingRock)
            .Where(_ => (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnPath) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight) || (FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearKnokedShortly))
            .Where(_ => _ == true)
            .DelayFrame(1)
            .Where(_ => _ == true)
            .Do(_ => resetCombo())
            .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
            .Subscribe()
                        .AddTo(observer);
        bearHitted
            .Where(_ => _ == true)
            .Delay(TimeSpan.FromSeconds(0.5f))
            .Do(_ => bearHitted.Value = false)
            .Subscribe()
                        .AddTo(observer);
        isComboIdle
            .Where(_=>true)
            .Do(_=> playerAnimator.SetInteger("comboCounter", 0))
             .Subscribe()
                        .AddTo(observer);
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
            if (FightModel.currentBear != null)
            {
                onHitBear();
            }
        }
    }
    void onHitBear()
    {
        if (!bearHitted.Value)
        {
            if (Vector3.Distance(transform.position, FightModel.currentBear.transform.position) < FightModel.shortAttackRangeValue)
            {
                if ((FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearTakeDamage &&
                           (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDistanceAttacking) &&
                           (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearRageMode)))
                {
                    bearHitted.Value = true;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage;
                    PlayerSFX.PlayPunch();
                }
                else if (FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearKnokedShortly)
                {
                    bearHitted.Value = true;
                    FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearTakeDamage;
                }
            }
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
                    playerAnimator.SetBool("Dead", false);
                    comboValue.Value = 0;
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
                    comboValue.Value = 0;

                    if ((FightModel.canTakeDamage) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                    {
                        Debug.Log("player Hitted");
                        playerAnimator.SetTrigger("Hit");
                        PlayerSFX.Playhit();
                        DD.DisplayDamage(damageFromMode(FightModel.currentFightMode));
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
                case FightModel.PlayerFightModes.playerTakeSmallDamage:
                    comboValue.Value = 0;

                    if ((FightModel.canTakeDamage) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                    {
                        Debug.Log("player Hitted");
                        playerAnimator.SetTrigger("Hit");
                        PlayerSFX.Playhit();
                        DD.DisplayDamage(damageFromMode(FightModel.currentFightMode)*0.2f);
                        Observable.Timer(TimeSpan.Zero)
                               .DelayFrame(1)
                               .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle)
                               .Where(_ => FightModel.currentPlayerHealth.Value > 0)
                               .Do(_ => FightModel.currentPlayerHealth.Value -= damageFromMode(FightModel.currentFightMode)*0.2f)
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
                    IsDead.Value = true;
                    observePlayerDead();
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
                if (!FightModel.isHoldingRock)
                {
                    if ((FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnPath) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight) || (FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearKnokedShortly))
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
        


    }
    public void MovmenteState(bool state)
    {
        if (state)
        {
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 7f;
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 10.5f;
        }
        else
        {
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed = 0f;
            FightModel.currentPlayer.GetComponent<StarterAssets.ThirdPersonController>().SprintSpeed = 0f;
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
            PlayerSFX.PlaySwoosh();
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
            if (!FightModel.isHoldingRock)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if ((FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnPath) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight))
                    {
                        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerBlockShortAttack;
                    }
                }
                if (Input.GetMouseButtonUp(1))
                {
                    if ((FightModel.currentFightStatus.Value == FightModel.fightStatus.OnFightWon) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnPath) || (FightModel.currentFightStatus.Value == FightModel.fightStatus.OnCloseDistanceFight))
                    {
                        FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerIdle;
                    }
                }
            }
        }
    }
    void observePlayerHittedDistance(GameObject observer)
    {
        detectionCollider.OnTriggerEnterAsObservable()
            .Where(_ => _.transform.CompareTag("distanceAttack"))
            .Do(_ => FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerTakeDamage)
            .Subscribe()
                        .AddTo(observer);
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
    public void intilize(bool onPath)
    {
        if (observerPath != null)
        {
            Destroy(observerPath);
        }
        playerAnimator = GetComponent<Animator>();
        initilizeFighter();
        playerController = GetComponent<ThirdPersonController>();
        observeFighterStatus();
        currentBearShieldHealth.Value = FightModel.bearShielHealth;
        currentBearHealthDistance.Value = FightModel.bearDistanceHealth;
        canChangeStatus = true;
        canHitAgainCombo.Value = true;
        FightModel.currentCombo = FightModel.comboNames.Idle;
        if (!onPath)
        {
            observePlayerHittedDistance(fightView.instance.gameObject);
            observePlayerCombo(fightView.instance.gameObject);
        }
        else
        {
            GameObject observer = new GameObject("PathObserver");
            observePlayerHittedDistance(observer);
            observePlayerCombo(observer);
            observerPath = observer;
        }
        initilized = true;
    }
}
