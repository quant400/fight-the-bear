using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class FightModel
{
    public enum fightStatus
    {
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
    }
    public enum bearFightModes
    {
        BearCinematicMode,
        BearIdle,
        BearShortFollowing,
        BearShortAttacking,
        BearShortAttackDone,
        BearTakeDamage,
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
        playerDistanceAttacking,
        playerTakeBigDamage,
        playerRunAwayMode,
        playerShockingMode,
        playerWon,
        playerDead,
    }
    public static ReactiveProperty<fightStatus> currentFightStatus = new ReactiveProperty<fightStatus>();
    public static ReactiveProperty<PlayerFightModes> currentPlayerStatus = new ReactiveProperty<PlayerFightModes>();
    public static ReactiveProperty<bearFightModes> currentBearStatus = new ReactiveProperty<bearFightModes>();
    public static bearFightModes lastBearStatus= bearFightModes.BearIdle;

    public static float bearShielHealth=3;
    public static float bearDistanceHealth = 3;

    public static float playerStartHealth=100;
    public static float bearStartHealth = 125;

    public static ReactiveProperty<float> currentPlayerHealth=new ReactiveProperty<float>();
    public static ReactiveProperty<float> currentBearHealth = new ReactiveProperty<float>();
    public static float playerCloseHitValue=1;
    public static float playerDistanceHitValue=3;
    public static float bearCloseHitValue = 2;
    public static float bearDistanceHitValue = 4;
    public static float shortAttackRangeValue = 5;

    public static int DistanceFightDuration;
    public static int RunFightDuration;
    public static int LastSceneFightDuration;
    public static float fightDuration;
    public static ReactiveProperty<float> currentLeftTime;
    public static bool bearIsStunned;
    public static float bearStunnedDuration=5f;
    public static int currentFightMode=1;
}
