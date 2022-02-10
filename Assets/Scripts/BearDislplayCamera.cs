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
    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
        player = GameObject.FindGameObjectWithTag("Player");
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
            this.enabled = false;
        }
    }
}
