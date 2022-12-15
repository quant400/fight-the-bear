
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class NFTInfo
{
    public int id;
    public string name;
}
public class NFTGetView : MonoBehaviour
{
    [SerializeField] characterSelectionView characterSelectView;
    public static UnityWebRequest temp;
    [SerializeField]
    GameObject noNFTCanvas;


    private void Start()
    {
        if (characterSelectView == null)
        {
            characterSelectView = GameObject.FindObjectOfType<characterSelectionView>();
        }
    }
    public void GetNFT()
    {
        StartCoroutine(KeyMaker.instance.GetOtherNft());
        StartCoroutine(KeyMaker.instance.GetRequest());
    }

    public void Display(NFTInfo[] NFTData)
    {
        NFTInfo[] used;
        if (gameplayView.instance.hasOtherChainNft)
        {
            NFTInfo tempNft = new NFTInfo() { name = "grane", id = 100000000 };
            NFTInfo[] tempArr = new NFTInfo[NFTData.Length + 1];
            for (int i = 0; i < tempArr.Length; i++)
            {
                if (i == 0)
                    tempArr[i] = tempNft;
                else
                    tempArr[i] = NFTData[i - 1];
            }
            bearGameModel.currentNFTArray = tempArr;
            used = tempArr;
        }

        else
        {
            bearGameModel.currentNFTArray = NFTData;
            used = NFTData;
        }
        if (used.Length == 0)
        {
            /*noNFTCanvas.SetActive(true);
            bearGameModel.userIsLogged.Value = false;*/
            gameplayView.instance.usingFreemint = true;
            characterSelectView.FreeMint();
            bearGameModel.userIsLogged.Value = true;
        }
        else
        {
            noNFTCanvas.SetActive(false);
            characterSelectView.SetData(used);
            bearGameModel.userIsLogged.Value = true;
        }


    }
    public void savedLoggedDisplay()
    {
        if (gameplayView.nftDataArray.Length == 0)
        {
            noNFTCanvas.SetActive(true);
            bearGameModel.userIsLogged.Value = false;
        }
        else
        {
            noNFTCanvas.SetActive(false);
            characterSelectView.SetData(bearGameModel.currentNFTArray);
            bearGameModel.userIsLogged.Value = true;
        }
    }

    //temp Fuction for skip
    public void Skip()
    {
        StartCoroutine(KeyMaker.instance.GetRequestSkip());
        characterSelectView.Skip();
    }
}

