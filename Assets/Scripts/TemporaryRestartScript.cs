using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TemporaryRestartScript : MonoBehaviour
{
    public static TemporaryRestartScript instance;
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
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        SceneManager.LoadScene(1);
        FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
        FightModel.currentPlayerLevel = 0;
        FightModel.gameTime.Value = 120;
        FightModel.gameScore.Value = 0;
        Time.timeScale = 1;
    }
}
