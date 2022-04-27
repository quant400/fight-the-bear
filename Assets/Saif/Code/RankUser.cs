using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUser : MonoBehaviour
{
    public Text userName;
    public Text userScore;
    public Text userRank;
    public Text assetID;
    public Text SessionsToday;

    public void setRankInfo(string _name, string _score, string _rank,string _assetID,int _sessions)
    {
        userName.text = _name;
        userScore.text = "Score : " +_score;
        userRank.text = _rank;
        assetID.text = "AssetID : "+_assetID;
        SessionsToday.text = "Session : "+_sessions;
    }
    public void setRankInfoRestApi(string _name, string _score, string _rank, int _assetID, int _sessions)
    {
        userName.text = _name;
        userScore.text = "Score : " + _score;
        userRank.text = _rank;
        assetID.text = "AssetID : " + _assetID.ToString();
        SessionsToday.text = "Session : " + _sessions;
    }
}