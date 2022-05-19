using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class cinematicView : MonoBehaviour
{
    //public CinemachineVirtualCamera[] cameraList;
    //public int currentCamera;
    //void Start()
    //{
    //    cameraList = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
    //    currentCamera = 0;

    //    for (int i = 0; i < cameraList.Length; i++)
    //    {
    //        cameraList[i].gameObject.SetActive(false);
    //    }

    //    if (cameraList.Length > 0)
    //    {
    //        cameraList[0].gameObject.SetActive(true);
    //    }
    //}

    //void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        currentCamera++;
    //        if (currentCamera < cameraList.Length)
    //        {
    //            cameraList[currentCamera - 1].gameObject.SetActive(false);
    //            cameraList[currentCamera].gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            cameraList[currentCamera - 1].gameObject.SetActive(false);
    //            currentCamera = 0;
    //            cameraList[currentCamera].gameObject.SetActive(true);
    //        }
    //    }
    //}
    public static void setCamera(bool CinMode ,int cinematicMode)
    {
        if (FightModel.playerCamera != null && FightModel.CinematicCamera != null)
        {
            FightModel.playerCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;

            FightModel.playerCamera.SetActive(!CinMode);
                FightModel.CinematicCamera.SetActive(CinMode);
                FightModel.CinematicCamera.GetComponent<Animator>().Play(cinematicMode.ToString());

        }
    }
}
