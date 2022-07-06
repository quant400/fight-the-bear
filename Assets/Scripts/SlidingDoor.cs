using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class SlidingDoor : MonoBehaviour
{
    [SerializeField]
    GameObject dooreffect;
    AudioSource sfx;

    private void Start()
    {
        sfx = GetComponent<AudioSource>();
    }
    public void OpenDoor()
    {
        
       transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>().Priority = 11;
        bearView.instance.deathCam.Priority = 0;
        bearView.instance.deathCam.gameObject.SetActive(false);
        Invoke("Open", 2f);
       
    }

    void Open()
    {
        FighterView.instance.MovmenteState(false);
        PlaySfx();
        transform.DOMove(new Vector3(transform.position.x, -15, transform.position.z), 4f).OnComplete(() =>
        {
            StopSfx();
            transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>().Priority = 9;
            dooreffect.SetActive(true);
            FighterView.instance.MovmenteState(true);
        });
    }


    void PlaySfx()
    {
        sfx.Play();
    }
    void StopSfx()
    {
        sfx.Pause();
    }
}
