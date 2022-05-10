using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_WEBGL
public class WebLogin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private int expirationTime;
    private string account;

   
    [SerializeField]
    NFTGetter nftGetter;
    [SerializeField]
    GameObject loginButton;

    // temp for skip
    [SerializeField]
    GameObject skipButton;
    private void Start()
    {
        if (SingleplayerGameControler.playerLogged)
        {
            nftGetter.savedLoggedDisplay();
        }
    }
    public void OnLogin()
    {
        if (SingleplayerGameControler.playerLogged)
        {
            loginButton.GetComponent<Button>().interactable = false;
            skipButton.GetComponent<Button>().interactable = false;
            nftGetter.savedLoggedDisplay();
        }
        else
        {
            Web3Connect();
            OnConnected();
        }
        
    }

    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "") {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        // reset login message
        SetConnectAccount("");
        // load next scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        loginButton.GetComponent<Button>().interactable = false;
        skipButton.GetComponent<Button>().interactable = false;
        nftGetter.GetNFT();
       

    }

    public void OnSkip()
    {
        nftGetter.Skip();
    }
}
#endif
