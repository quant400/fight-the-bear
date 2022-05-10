using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onSceneLoad : MonoBehaviour
{
    
    void Start()
    {
        chickenGameModel.lastSavedStep = chickenGameModel.gameCurrentStep.Value;
        chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.onSceneLoaded;
        chickenGameModel.gameCurrentStep.Value = chickenGameModel.lastSavedStep;

    }
}
