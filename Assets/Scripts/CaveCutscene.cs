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
    AudioSource sfx;
    public bool last=false;
    string[] lines = {
       "Watch out! micro tragedy!\n\n China wants to ban bitcoin again",
        "Oh no Ethereum 2.0\n\n is delayed \n\n 'till next year again",
        "Be careful\n\n funds are not safu",
        "Your stablecoin depegged, \n\n it's a death spiral...",
        "OMG\n\n another rug pull",
        "That meme coin is pumping.\n\n You should have bought",
        "Aaaaaaaaaaaaaaand\n\n its gone..."
    };
    private void Start()
    {
        cam = mCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        sfx = GetComponent<AudioSource>();
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
                FighterView.instance.MovmenteState(true);
                GameUIView.instance.DeactivateText();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && last)
        {
            bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnCloseToCave;
            player = other.gameObject;
        }
    }

    public void StartScene()
    {
        FighterView.instance.MovmenteState(false);
        Invoke("DisplayText", 1f);
        Invoke("PlayAudio", 0.25f);
        mCam.m_Priority = 20;
        started = true;
        transform.GetComponent<BoxCollider>().enabled = false;
    }

    void DisplayText()
    {
            int ind = Random.Range(0, 7);
            GameUIView.instance.ActivateText(lines[ind].ToUpper());
    }

    void PlayAudio()
    {
        if(!SFXView.instance.sfxMuted)
        sfx.Play();
    }
}
