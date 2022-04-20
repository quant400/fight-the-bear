using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CaveCutscene : MonoBehaviour
{
    [SerializeField]
    float timeToComplete;

    float completed;
    [SerializeField]
    CinemachineVirtualCamera mCam;
    CinemachineTrackedDolly cam;
  
    GameObject player;
    private bool started;
    public bool last=false;
    private void Start()
    {
        cam = mCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (started)
        {
            if (completed <= 1)
            {
                completed += Time.deltaTime / timeToComplete;
                cam.m_PathPosition = completed;

            }
            else
            {
                mCam.Priority = 8;
                this.enabled = false;
                started = false;
                var FC = player.GetComponent<FightController>();
                FC.EnableMovement();
                FC.DeactivateText();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && last)
        {
            player = other.gameObject;
            var FC = player.GetComponent<FightController>();
            FC.DisableMovement();
            FC.ActivateText("Enter Text Here");
            mCam.m_Priority = 20;
            started = true;
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
