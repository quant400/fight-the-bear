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
    FightController FC;
    GameObject player;
    private bool started;
    public bool last=false;
    string[] lines = {
       "Watch out! micro tragedy!\n\n China wants to ban bitcoin again",
        "Oh no Ethereum 2.0\n\n is delayed \n\n 'till next year again",
        "Be careful\n\n funds are not safu",
        "Contract hacked.\n\n Funds drained",
        "OMG\n\n another rug pull",
        "That meme coin is pumping.\n\n You should have bought",
        "Aaaaaaaaaaaaaaand\n\n its gone..."
    };
    private void Start()
    {
        cam = mCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        FC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
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
                FC.EnableMovement();
                UIController.instance.DeactivateText();
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
            Invoke("DisplayText", 1f);
            mCam.m_Priority = 20;
            started = true;
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void DisplayText()
    {
        if (FC.GetBearNumber() <= 7)
            UIController.instance.ActivateText(lines[FC.GetBearNumber()].ToUpper());
        else
        {
            int ind = Random.Range(0, 8);
            UIController.instance.ActivateText(lines[ind].ToUpper());
        }
    }
}
