using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public enum powerUpVarient
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
            other.GetComponent<PlayerPowerUpHandler>().StartPowerUp(varient, powerUpDuration, PowerupSpeedMultiplier);
            Destroy(gameObject);
        }
    }


  
}
