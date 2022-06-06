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
    AudioSource aud;
    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerPowerUpHandler>().StartPowerUp(varient, powerUpDuration, PowerupSpeedMultiplier);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            PlayPowewrUp();
            Invoke("Dest", 1f);
        }
    }

    public void PlayPowewrUp()
    {
        if (!SFXController.instance.sfxMuted)
        {
            aud.Play();
        }
    }

    void Dest()
    {
        Destroy(gameObject);
    }
}
