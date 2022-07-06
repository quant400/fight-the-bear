using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BearDislplayCamera : MonoBehaviour
{
    [SerializeField]
    float timeToComplete;

    float completed;
    CinemachineTrackedDolly cam;

    GameObject player;
    BearSFXController bSFX;
    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
        player = MapView.instance.GetPlayer();
        bSFX = GetComponentInParent<BearSFXController>();
        bSFX.Invoke("PlayRoar", 2f);
        Invoke("Roar", 2f);
        
    }

    private void Update()
    {
        if (completed <= 1)
        {
            completed += Time.deltaTime / timeToComplete;
            cam.m_PathPosition = completed;

        }
        else
        {
            GetComponent<CinemachineVirtualCamera>().Priority = 8;
            player.GetComponentInChildren<CharacterController>().enabled = true;
            player.GetComponentInChildren<StarterAssets.ThirdPersonController>().enabled = true;
            gameObject.GetComponentInParent<BearController>().StartFight();
            this.enabled = false;
        }
    }


    void Roar()
    {
        GetComponentInParent<Animator>().SetTrigger("Roar");
    }
}
