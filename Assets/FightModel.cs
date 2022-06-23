using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cinemachine;
public class FightModel
{
    public enum fightStatus
    {
        OnPath,
        OnEnterCave,
        OnStartCenimatic,
        OnStartFight,
        OnCloseDistanceFight,
        OnRangeDistanceFight,
        OnRangeDistanceFightWon,
        OnRunFromRageBear,
        OnBearHitRock,
        OnLastFightScene,
        OnFightWon,
        OnFightLost,
        OnTimeUp,
        OnChangeState,

    }
    public enum bearFightModes
    {
        BearCinematicMode,
        BearIdle,
        BearShortFollowing,
        BearShortAttacking,
        BearShortAttackDone,
        BearTakeDamage,
        BearTakeShieldDamage,
        BearDistanceAttacking,
        BearTakeBigDamage,
        BearRageMode,
        BearKnokedShortly,
        BearShockingPlayer,
        BearWon,
        BearDead,
    }
    public enum PlayerFightModes
    {
        playerCinematicMode,
        playerIdle,
        playerCombo,
        playerBlockShortAttack,
        playerShortAttacking,
        playerTakeDamage,
        playerTakeSmallDamage,
        playerDistanceAttacking,
        playerTakeBigDamage,
        playerRunAwayMode,
        playerShockingMode,
        playerWon,
        playerDead,
    }
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
    public static comboNames currentCombo;
    public static ReactiveProperty<fightStatus> currentFightStatus = new ReactiveProperty<fightStatus>();
    public static ReactiveProperty<PlayerFightModes> currentPlayerStatus = new ReactiveProperty<PlayerFightModes>();
    public static ReactiveProperty<bearFightModes> currentBearStatus = new ReactiveProperty<bearFightModes>();
    public static bearFightModes lastBearStatus= bearFightModes.BearIdle;

    public static int bearShielHealth=3;
    public static float bearDistanceHealth = 3;

    public static float playerStartHealth=100;
    public static float bearStartHealth = 100;

    public static ReactiveProperty<float> currentPlayerHealth=new ReactiveProperty<float>();
    public static ReactiveProperty<float> currentBearHealth = new ReactiveProperty<float>();
    public static float playerCloseHitValue=10;
    public static float playerDistanceHitValue=10;
    public static float bearCloseHitValue = 2;
    public static float bearDistanceHitValue = 3;
    public static float shortAttackRangeValue = 5;

    public static int DistanceFightDuration;
    public static int RunFightDuration;
    public static int LastSceneFightDuration;
    public static float fightDuration;
    public static ReactiveProperty<float> currentLeftTime;
    public static bool bearIsStunned;
    public static float bearStunnedDuration=5f;
    public static int currentFightMode=1;
    public static ReactiveProperty<int> fightStatusValue = new ReactiveProperty<int>();
    public static GameObject currentBear;
    public static GameObject currentPlayer;
    public static int currentPlayerLevel=0;
    public static ReactiveProperty<float> rageModeValue =new ReactiveProperty<float>();
    public static ReactiveProperty<float> gameScore =new ReactiveProperty<float>(0);
    public static ReactiveProperty<float> gameTime = new ReactiveProperty<float>(120);
    public static GameObject playerCamera;
    public static GameObject CinematicCamera;
    public static GameObject CinematicBackFakeCamera;
    public static GameObject bearObserveObj;
    public static GameObject fightObserveObj;
    public static Vector3 offsetFromPlayer;
    public static CinemachineVirtualCamera playerVirtualCamera;
    public static CinemachineBrain playerCameraBrain;
    public static int lastRand;
    public static bool canTakeDamage;

}
