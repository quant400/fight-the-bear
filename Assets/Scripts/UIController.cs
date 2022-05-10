using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    GameObject fightCanvas;
    [SerializeField]
    Image bearHealth, playerHelthDisplay;
    [SerializeField]
    Image bearAgression;

    [SerializeField]
    SliderScript slider;
    [SerializeField]
    GameObject sequenceInputs, gaugeInputs;

    [SerializeField]
    TMP_Text scoreDisplay;
    [SerializeField]
    TMP_Text timerDisplay;
    [SerializeField]
    TMP_Text CutSceneText;
    [SerializeField]
    GameObject cutSceneImage;

    [SerializeField]
    GameObject gameOverPanel;



    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this);
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
        TemporaryRestartScript.instance.Reset();
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
       
    }
}
