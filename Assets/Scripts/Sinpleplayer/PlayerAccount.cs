using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAccount : MonoBehaviour
{
    [SerializeField]
    GameObject[] players;
    NFTInfo[] myNFT;


    public void SetData(NFTInfo[] Pdata)
    {
        myNFT = Pdata;
        SetDefault();
           
    }


    void SetDefault()
    {
        if (myNFT[0].id == 538.ToString())
            players[1].SetActive(true);

        else
            players[0].SetActive(true);

    }
}
