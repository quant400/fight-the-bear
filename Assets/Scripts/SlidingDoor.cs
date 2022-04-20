using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class SlidingDoor : MonoBehaviour
{
    public void OpenDoor()
    {
       transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>().Priority = 11;
        Invoke("Open", 2f);
       
    }

    void Open()
    {
        FightController fc = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
        fc.DisableMovement();
        transform.DOMove(new Vector3(transform.position.x, -15, transform.position.z), 4f).OnComplete(() =>
        {
            transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>().Priority = 9;
            fc.EnableMovement();
        });
    }
}
