
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx.Operators;
using UniRx;
using UniRx.Triggers;
#if UNITY_WEBGL
public class webLoginView : MonoBehaviour
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
    NFTGetView nftGetter;
    [SerializeField]
    GameObject loginButton;

    // temp for skip
    [SerializeField]
    GameObject skipButton;
    public void checkUSerLoggedAtStart()
    {
        if (chickenGameModel.userIsLogged.Value)
        {
            nftGetter.savedLoggedDisplay();
        }
        else
        {
            chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.OnLogin;

        }
    }
    public void OnLogin(Button loginBtn, Button skipBtn )
    {
        if (chickenGameModel.userIsLogged.Value)
        {
            loginBtn.GetComponent<Button>().interactable = false;
            skipBtn.GetComponent<Button>().interactable = false;
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
        while (account == "")
        {
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

