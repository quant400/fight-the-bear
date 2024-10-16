
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx.Operators;
using UniRx;
using UniRx.Triggers;

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

    [SerializeField]
    GameObject tryoutButton;
    public void checkUSerLoggedAtStart()
    {
        if (bearGameModel.userIsLogged.Value)
        {
            nftGetter.savedLoggedDisplay();
        }
        else
        {
            bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnLogin;

        }
    }
    public void OnLogin(Button loginBtn, Button skipBtn, Button tryoutBtn)
    {
        if (bearGameModel.userIsLogged.Value)
        {
            loginBtn.GetComponent<Button>().interactable = false;
            skipBtn.GetComponent<Button>().interactable = false;
            tryoutBtn.GetComponent<Button>().interactable = false;
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
        tryoutButton.GetComponent<Button>().interactable = false;
        gameplayView.instance.usingMeta = true;
        loginButton.GetComponentInParent<FireBaseWebGLAuth>().Close();
        nftGetter.GetNFT();


    }

    public void OnSkip()
    {
        PlayerPrefs.SetString("Account", "0xD408B954A1Ec6c53BE4E181368F1A54ca434d2f3");
        gameplayView.instance.isTryout = false;
        nftGetter.Skip();
        //nftGetter.GetNFT();
    }

    public void OnTryout()
    {
        gameplayView.instance.isTryout = true;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(6).gameObject.SetActive(true);
        transform.GetChild(7).gameObject.SetActive(true);
        nftGetter.Skip();
    }
}


