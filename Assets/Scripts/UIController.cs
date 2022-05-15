using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UniRx;
using System;
public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    public GameObject fightCanvas;
    [SerializeField]
    public Image bearHealth, playerHelthDisplay;
    [SerializeField]
    public Image bearAgression;

    [SerializeField]
    public SliderScript slider;
    [SerializeField]
    GameObject sequenceInputs, gaugeInputs;

    [SerializeField]
    public TMP_Text scoreDisplay;
    [SerializeField]
    public TMP_Text timerDisplay;
    [SerializeField]
    public  TMP_Text CutSceneText;
    [SerializeField]
    public GameObject cutSceneImage;

    [SerializeField]
    public GameObject gameOverPanel;



    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this);
        observeScoreAndTimer();
    }
    void observeScoreAndTimer()
    {
        FightModel.gameScore
            .Do(_ => scoreDisplay.text ="SCORE "+ _.ToString())
            .Subscribe()
            .AddTo(this);
        FightModel.gameTime
           .Do(_ => timerDisplay.text = "TIME LEFT : " + _.ToString())
           .Where(_ => _==0)
           .Do(_ =>gameOverPanel.SetActive(true))
            .Do(_ => FightModel.currentFightStatus.Value = FightModel.fightStatus.OnFightLost)
            .Do(_ => FightModel.currentFightMode = 1)
            .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
            .Delay(TimeSpan.FromMilliseconds(3000))
            .Do(_ => Time.timeScale = 0)
           .Subscribe()
           .AddTo(this);
    }
    public void ActivateInputs()
    {
        fightCanvas.SetActive(true);
        gaugeInputs.SetActive(false);
        sequenceInputs.SetActive(false);

    }

    public void DeactivateFightCanvas()
    {
        fightCanvas.SetActive(false);
    }
    public void EnableGauge()
    {
        gaugeInputs.SetActive(true);
    }
    public void DisableGauge()
    {
        gaugeInputs.SetActive(false);
    }
    public void UpdateValues(float PH, float BH, float score)
    {
        playerHelthDisplay.fillAmount = PH;
        bearHealth.fillAmount = BH;
        scoreDisplay.text = ("Score : ").ToUpper() + score.ToString("00");
    }

    public void UpdateTimerVal(float val)
    {
        timerDisplay.text = ("Time Left : ").ToUpper() + val.ToString("00");
    }

    public void ActivateText(string info)
    {
        CutSceneText.text = info;
        CutSceneText.gameObject.SetActive(true);
        cutSceneImage.SetActive(true);
    }

    public void DeactivateText()
    {
        CutSceneText.gameObject.SetActive(false);
        cutSceneImage.SetActive(false);
    }

    public void SetSliderInput(string currentKey)
    {

        slider.SetInput(currentKey.ToString());
        slider.StartSlider();

    }
    public float GetSliderValue()
    {
        return slider.StopSlider();
    }

    
    // move to game over view later
    public void DisplayGameOver(string message, float score)
    {
        gameOverPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = message.ToUpper();

        gameOverPanel.transform.GetChild(3).GetComponent<TMP_Text>().text ="SCORE: " +((int)(score)).ToString();
       
        gameOverPanel.SetActive(true);
    }



    public void SetAggression(float val)
    {
        bearAgression.fillAmount = val;
    }


    public void ResetGame()
    {
        fightCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        scoreDisplay.text = ("Score : ").ToUpper()+"0";
        timerDisplay.text = ("Time Left : ").ToUpper() + "45";
        FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        FightModel.currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        TemporaryRestartScript.instance.Reset();
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
       
    }
}
