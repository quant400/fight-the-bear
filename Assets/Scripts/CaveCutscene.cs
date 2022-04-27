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
    string[] lines = {
        "Watch out! micro tragedy!\n\n China wants to ban bitcoin again",
        "Oh no Ethereum 2.0 is delayed 'till next year again",
        "Be careful funds are not safu",
        "Contract hacked. Funds drained",
        "OMG another rug pull",
        "That meme coin is pumping.\n\n You should have bought",
        "Aaaaaaaaaaaaaaand its gone"
    };
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
        if (other.CompareTag("Player") && last)
        {
            player = other.gameObject;
            var FC = player.GetComponent<FightController>();
            FC.DisableMovement();
            if (FC.GetBearNumber() <= 7)
                FC.ActivateText(lines[FC.GetBearNumber()].ToUpper());
            else
            {
                int ind = Random.Range(0, 8);
                FC.ActivateText(lines[ind].ToUpper());
            }
            mCam.m_Priority = 20;
            started = true;
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
