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
            FightModel.CinematicCamera.SetActive(CinMode);
            FightModel.CinematicCamera.GetComponent<Animator>().Play(cinematicMode.ToString());
        }
        else
        {
            FightModel.CinematicCamera.GetComponent<Animator>().Play(cinematicMode.ToString());
            FightModel.CinematicCamera.SetActive(CinMode);
            FightModel.CinematicBackFakeCamera.SetActive(true);
            yield return new WaitForSeconds(time);
            FightModel.CinematicBackFakeCamera.SetActive(false);
            FightModel.playerCamera.SetActive(!CinMode);
        }

    }
    
}
