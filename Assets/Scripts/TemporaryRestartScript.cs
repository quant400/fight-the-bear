using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TemporaryRestartScript : MonoBehaviour
{
   public static TemporaryRestartScript instance;
    [SerializeField]
    GameObject playerUI;
    //move to some other script later
    void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        DontDestroyOnLoad(this);
    }


    public void Reset()
    {
        playerUI.GetComponent<UIController>().ResetGame();
       
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        //SceneManager.LoadScene(1);
    }


    public void TryOutHomrBtn()
    {
        if (gameplayView.instance.isTryout)
        {
           
            chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnLogin;
            SceneManager.LoadScene(chickenGameModel.mainSceneLoadname.sceneName);

        }
    }
 
}
