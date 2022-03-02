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
        transform.DOMove(new Vector3(transform.position.x, -5, transform.position.z), 4f).OnComplete(() =>
            transform.parent.GetChild(2).GetComponent<CinemachineVirtualCamera>().Priority = 9
            );
    }
}
