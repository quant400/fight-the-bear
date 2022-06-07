using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerPowerUpHandler : MonoBehaviour
{
    public void StartPowerUp(powerUpVarient p , float duration , float speedmult)
    {
        StartCoroutine(Powerup(p, duration, speedmult));
    }
    IEnumerator Powerup(powerUpVarient varient , float powerUpDuration, float PowerupSpeedMultiplier)
    {
        
        switch (varient)
        {
            case powerUpVarient.attack:
                GetComponent<FightController>().setAttackDamage(10);
                yield return new WaitForSeconds(powerUpDuration);
                GetComponent<FightController>().setAttackDamage(-10);
                break;
            case powerUpVarient.shield:
                GetComponent<FightController>().ActivateShield();
                yield return new WaitForSeconds(powerUpDuration);
                GetComponent<FightController>().DeactivateShield();
                break;
            case powerUpVarient.speed:
                ThirdPersonController TPC = GetComponent<ThirdPersonController>();
                TPC.MoveSpeed *= PowerupSpeedMultiplier;
                TPC.SprintSpeed = TPC.MoveSpeed;
                yield return new WaitForSeconds(powerUpDuration);
                TPC.MoveSpeed /= PowerupSpeedMultiplier;
                TPC.SprintSpeed = TPC.MoveSpeed * 1.5f;
                break;
        }
       


    }
}
