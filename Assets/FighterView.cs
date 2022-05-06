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
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerController = GetComponent<ThirdPersonController>();
        observeFighterStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    playerAnimator.SetTrigger("Hit");
                    Observable.Timer(TimeSpan.Zero)
                                           .DelayFrame(1)
                                           .Do(_ => FightModel.currentPlayerStatus.Value= FightModel.PlayerFightModes.playerIdle)
                                            .Where(_ => FightModel.currentPlayerHealth.Value > 0)
                           .Do(_ => FightModel.currentPlayerHealth.Value -= damageFromMode(FightModel.currentFightMode))
                           .Subscribe()
                           
                        .AddTo(this);
                    break;

                case FightModel.PlayerFightModes.playerCombo:
                    playerController.MoveSpeed = 0.5f;
                    playerController.SprintSpeed = 1f;
                    break;
                case FightModel.PlayerFightModes.playerBlockShortAttack:
                    playerController.MoveSpeed = 0.5f;
                    playerController.SprintSpeed = 1f;
                    playerAnimator.SetBool("Block", true);
                    break;
            }
        }
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
