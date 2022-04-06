using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

enum powerUpVarient
{
    attack,
    shield,
    speed
}
public class PowerUp : MonoBehaviour
{
    [SerializeField]
    powerUpVarient varient;
    [SerializeField]
    float powerUpDuration;
    [SerializeField]
    float PowerupSpeedMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            //GetComponent<Light>().enabled = false;
            StartCoroutine(Powerup(other.gameObject));
           
        }
    }


    IEnumerator Powerup(GameObject player)
    {
        switch (varient)
        {
            case powerUpVarient.attack:
                player.GetComponent<FightController>().setAttackDamage(10);
                yield return new WaitForSeconds(powerUpDuration);
                player.GetComponent<FightController>().setAttackDamage(-10);
                break;
            case powerUpVarient.shield:
                player.GetComponent<FightController>().ActivateShield();
                yield return new WaitForSeconds(powerUpDuration);
                player.GetComponent<FightController>().DeactivateShield();
                break;
            case powerUpVarient.speed:
                ThirdPersonController TPC = player.GetComponent<ThirdPersonController>();
                TPC.MoveSpeed *= PowerupSpeedMultiplier;
                TPC.SprintSpeed = TPC.MoveSpeed;
                yield return new WaitForSeconds(powerUpDuration);
                TPC.MoveSpeed /= PowerupSpeedMultiplier;
                TPC.SprintSpeed = TPC.MoveSpeed * 1.5f;
                break;
        }
      
        //for testing 

       
    }
}
