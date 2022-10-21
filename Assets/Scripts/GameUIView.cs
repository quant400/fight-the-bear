using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UniRx;
public class GameUIView : MonoBehaviour
{
    public static GameUIView instance;

    [SerializeField]
    public GameObject fightCanvas;
    [SerializeField]
    public Image bearHealth, playerHelthDisplay;
    [SerializeField]
    public Image bearAgression;

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
    public GameObject gameOverPanel;

    [SerializeField]
    GameObject settingsPanel;

    [SerializeField]
    StarterAssets.UICanvasControllerInput virtualController;
    public GameObject RockPickButton;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this);
        observeScore();
    }

    public void ActivateInputs()
    {
        fightCanvas.SetActive(true);
        gaugeInputs.SetActive(false);
        sequenceInputs.SetActive(false);

    }
    void observeScore()
    {
        FightModel.gameScore
            .Do(_ => scoreDisplay.text = ("Score : ").ToUpper() + _.ToString("00"))
            .Subscribe()
            .AddTo(this);
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


    public void SetAggression(float val)
    {
        bearAgression.fillAmount = val;
    }


    public void ResetGame()
    {
        gameplayView.instance.isPaused = false;
        fightCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        scoreDisplay.text = ("Score : ").ToUpper()+"0";
        timerDisplay.text = ("Time Left : ").ToUpper() + "45";
        FightModel.gameScore.Value = 0;
        FightModel.currentPlayer.transform.GetChild(0).tag = "Untagged";
        FightModel.currentPlayer = null;
        FightModel.currentPlayerLevel = 0;
        Destroy(MapView.instance.GetPlayerLoc());
        MapView.instance.ResetPlayerLock();
        Time.timeScale = 1f;

    }
    
    public void EnableGameOver(float delay,string txt)
    {
        gameOverPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = txt.ToUpper();
        StartCoroutine(Gameover(delay));
    }

    IEnumerator Gameover(float d)
    {
        yield return new WaitForSeconds(d);
        gameOverPanel.SetActive(true);
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGameEnded;
    }


    public void OpenSettings()
    {
        gameplayView.instance.isPaused = true;
        Time.timeScale = 0f;
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        gameplayView.instance.isPaused = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }


    public void EnableVirtualController(GameObject player)
    {
        virtualController.GetRefrence(player);
        virtualController.gameObject.SetActive(true);
    }

    public void DisableVirtualController()
    {
        virtualController.gameObject.SetActive(true);
    }
}
