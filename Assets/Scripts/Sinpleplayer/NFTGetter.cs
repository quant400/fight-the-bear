using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NFTGetter : MonoBehaviour
{
    CharacterSelectionScript cS;
    public static UnityWebRequest temp;
    [SerializeField]
    GameObject noNFTCanvas;
    

    void Start()
    {
        cS = GetComponent <CharacterSelectionScript> ();
    }
    public void GetNFT()
    {
        Debug.LogWarningFormat("Change this before final build and also rename youngin, sledghammer and long shot");
        string acc = PlayerPrefs.GetString("Account");
        StartCoroutine(GetRequest("https://api.cryptofightclub.io/game/sdk/"+acc));

        //testing link
        //StartCoroutine(GetRequest("https://api.cryptofightclub.io/game/sdk/0xbecd7b5cfab483d65662769ad4fecf05be4d4d05"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    temp = webRequest;
                    Display();
                    break;
            }
        }
    }

    void Display()
    {
        string data = "{\"Items\":" + temp.downloadHandler.text + "}";

        NFTInfo[] NFTData = JsonHelper.FromJson<NFTInfo>(data);
        SingleplayerGameControler.nftDataArray = NFTData;
        if (NFTData.Length==0)
        {
            noNFTCanvas.SetActive(true);
            SingleplayerGameControler.playerLogged = false;
        }
        else
        {
            noNFTCanvas.SetActive(false);
            cS.SetData(NFTData);
            SingleplayerGameControler.playerLogged = true;
        }


    }
    public void savedLoggedDisplay()
    {
        if (SingleplayerGameControler.nftDataArray.Length == 0)
        {
            noNFTCanvas.SetActive(true);
            SingleplayerGameControler.playerLogged = false;
        }
        else
        {
            noNFTCanvas.SetActive(false);
            cS.SetData(SingleplayerGameControler.nftDataArray);
            SingleplayerGameControler.playerLogged = true;
        }
    }

    //temp Fuction for skip
    public void Skip()
    {
        cS.Skip();
    }
}

