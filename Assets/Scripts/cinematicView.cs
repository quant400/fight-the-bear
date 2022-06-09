using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;
public class cinematicView : MonoBehaviour
{
    public static cinematicView instance;
    private void Awake()
    {
       
        if (instance != null)
            Destroy(this);
        else
            instance = this;

    }
    public void setCamera(bool CinMode ,int cinematicMode)
    {
        if (FightModel.playerCamera != null && FightModel.CinematicCamera != null)
        {
            if (CinMode)
            {
                FightModel.playerCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                StartCoroutine(switchCamera(0.1f, CinMode, cinematicMode));
            }
            else
            {
                FightModel.playerCameraBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                StartCoroutine(switchCamera(2, CinMode, cinematicMode));
            }



        }
    }
    public static IEnumerator switchCamera(float time,bool CinMode,int cinematicMode)
    {
        if (CinMode==true)
        {
            yield return new WaitForSeconds(time);
            FightModel.playerCamera.SetActive(!CinMode);
            FightModel.CinematicCamera.transform.position = FighterView.instance.lookAt.position + FightModel.offsetFromPlayer;
            FightModel.CinematicCamera.transform.LookAt(FightModel.currentBear.transform);
            FightModel.CinematicCamera.SetActive(CinMode);
        }
        else
        {
            FightModel.CinematicCamera.transform.position = FighterView.instance.lookAt.position + FightModel.offsetFromPlayer;
            FightModel.CinematicCamera.transform.LookAt(FightModel.currentBear.transform);
            FightModel.CinematicCamera.SetActive(CinMode);
            FightModel.playerCamera.SetActive(!CinMode);
        }

    }
    
}
