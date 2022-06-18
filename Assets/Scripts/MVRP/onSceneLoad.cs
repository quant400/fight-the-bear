using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onSceneLoad : MonoBehaviour
{
    
    void Start()
    {
        bearGameModel.lastSavedStep = bearGameModel.gameCurrentStep.Value;
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnSceneLoaded;
        bearGameModel.gameCurrentStep.Value = bearGameModel.lastSavedStep;

    }
}
