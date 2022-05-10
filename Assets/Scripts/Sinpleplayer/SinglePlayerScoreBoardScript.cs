using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
public class SinglePlayerScoreBoardScript : MonoBehaviour
{
    public static SinglePlayerScoreBoardScript instance;
    [SerializeField]
    Transform scorePanel;

    Dictionary<string, int> playerIndex = new Dictionary<string, int>();
    [SerializeField]
    TMP_Text winnerText;
    int players;
    [SerializeField]
    TMP_Text chickenCount;
    int chickensCollected;
    [SerializeField]
    GameObject chickenCollectedImage;
    public float time;
    [SerializeField]
    Image timerFill;
    public bool started = false;
    [SerializeField]
    GameObject endGameObject;
    [SerializeField]
    TMP_Text timerValue;
    float currentTime;
    public ReactiveProperty<bool> timeIsUp = new ReactiveProperty<bool>();
    public ReactiveProperty<float> reactiveTime = new ReactiveProperty<float>();
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        timeIsUp.Value = true;


    }
    public void StartGame(float timeOfGame)
    {
        Debug.Log("timeSetted");
        time = timeOfGame;
        currentTime = time;
        reactiveTime.Value = time;
        started = true;
        timeIsUp.Value = false;
        counterObservation();
    }
    public void counterObservation()
    {
        timeIsUp
            .Where(_ => _ == false)
            .Where(_=>reactiveTime.Value>0)
            .Where(_ => started)
            .Do(_ => timeIsUp.Value = true)
            .Do(_=> SetReactiveTime())
            .Delay(TimeSpan.FromSeconds(1))
            .Where(_=>started)
            .Do(_=> timeIsUp.Value = false)
            .Subscribe()
            .AddTo(this);
        
        reactiveTime
           .Where(_ => started)
           .Where(_ => _ == 0)
           .Do(_=>started=false)
           .Do(_ => timerValue.text = "<color=red>" + _.ToString() + "</color>")
           .Delay(TimeSpan.FromSeconds(1))
           .Do(_ => SetTimeEndGame(_))
           .Subscribe()
           .AddTo(this);



    }
    public void SetReactiveTime()
    {
        reactiveTime.Value -= 1;
        currentTime = reactiveTime.Value;
        SetTimeInUI(currentTime);


    }
    public void SetTimeInUI(float time)
    {
        if (time > 20)
        {
            timerValue.text = ((int)(currentTime)).ToString();
        }
        else if (time <= 20)
        {
            timerValue.text = "<color=red>" + ((int)(currentTime)).ToString() + "</color>";
        }
        timerFill.fillAmount = (float)currentTime / (float)SinglePlayerScoreBoardScript.instance.time;
    }
    public void SetTimeEndGame(float time)
    {


        timerValue.text = "<color=red>" + "0" + "</color>";

        if (gameplayView.instance != null)
                {
                    gameplayView.instance.EndGame();

                }
                chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnGameEnded;
                DisplayScore();
            


        
    }
    private void Update()
    {
       
    }

    public void DisplayScore()
    {
        //winnerText.text = "You collected " + chickensCollected + " chickens";
        //winnerText.gameObject.SetActive(true);
        if (gameplayView.instance != null)
        {
            gameplayView.instance.gameOverObject.SetActive(true);
        }
    }
    public void AnimChickenCollected()
    {
        chickensCollected++;
        var temp = Instantiate(chickenCollectedImage, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
        temp.transform.DOMove(chickenCount.transform.position, 1f).OnComplete(() =>
        {  
            Destroy(temp);
            chickenCount.text = chickensCollected.ToString("00");
            DOTween.To(() =>chickenCount.fontSize, x => chickenCount.fontSize = x, 75, 0.5f).OnComplete(
              () => DOTween.To(() => chickenCount.fontSize, x => chickenCount.fontSize = x, 60, 0.5f));

        });
    }

    public int GetScore()
    {
        return chickensCollected;
    }
}

