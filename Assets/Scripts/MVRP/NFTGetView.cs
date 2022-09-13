
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
        StartCoroutine(KeyMaker.instance.GetRequest());
    }

    public void Display(NFTInfo[] NFTData)
    {
       /* string data = "{\"Items\":" + temp.downloadHandler.text + "}";
        bearGameModel.currentNFTString = data;

        NFTInfo[] NFTData = JsonHelper.FromJson<NFTInfo>(data);*/
        bearGameModel.currentNFTArray = NFTData;
        if (NFTData.Length == 0)
        {
            noNFTCanvas.SetActive(true);
            bearGameModel.userIsLogged.Value = false;
        }
        else
        {
            noNFTCanvas.SetActive(false);
            characterSelectView.SetData(NFTData);
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

