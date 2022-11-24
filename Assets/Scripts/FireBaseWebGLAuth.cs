using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using DG.Tweening;
using System;

public struct LoginObject
{
    public string uid;
    public string email;
    public string emailVerified;
    public string displayName;

}

public class FireBaseWebGLAuth : MonoBehaviour
{
    [Header("Login")]
    public Transform SignInPanel;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;

    //Register variables
    [Header("Register")]
    public Transform registerPanel;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header ("Others")]
    [SerializeField]
    GameObject methodSelect;
    GameObject currentOpenWindiow;
    [SerializeField]
    TMP_Text InfoDisplay;

    private void Start()
    {
        currentOpenWindiow = methodSelect;
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            return;
        }

        FirebaseAuth.OnAuthStateChanged(gameObject.name, "DisplayUserInfo", "DisplayInfo");
    }

    public void OnSignInClick()
    {
        warningLoginText.text = "";
        if (emailLoginField.text == "" || !IsValidEmail(emailLoginField.text))
        {
            SignInPanel.DOShakePosition(1, 1);
            warningLoginText.text = "Please enter a valid email".ToUpper();
            warningLoginText.color = Color.red;
        }
        else
        {
            SignInWithEmailAndPassword();
        }
    }

    public void OnRegisterClick()
    {
        warningRegisterText.text = "";
        if(emailRegisterField.text=="" || !IsValidEmail(emailRegisterField.text))
        {
            registerPanel.DOShakePosition(1,1);
            warningRegisterText.text = "Please enter a valid email".ToUpper();
            warningRegisterText.color = Color.red;
        }
        else if(passwordRegisterField.text!=passwordRegisterVerifyField.text)
        {
            registerPanel.DOShakePosition(1, 1);
            warningRegisterText.text = "Password does not match".ToUpper();
            warningRegisterText.color = Color.red;
        }
        else
        {
            CreateUserWithEmailAndPassword();
        }
    }


    public void SignInWithEmailAndPassword() =>
          FirebaseAuth.SignInWithEmailAndPassword(emailLoginField.text, passwordLoginField.text, gameObject.name, "SignedIn", "DisplayError");
    public void CreateUserWithEmailAndPassword() =>
        FirebaseAuth.CreateUserWithEmailAndPassword(emailRegisterField.text, passwordRegisterField.text, gameObject.name, "SignedIn", "DisplayError");

    public void SignInWithGoogle()
    {
        PlayerPrefs.SetString("LastLogin", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetInt("SignOut",0);
        FirebaseAuth.SignInWithGoogle(gameObject.name, "SignedIn", "DisplayError");
    }



    void DisplayInfo(string info)
    {
        Debug.Log(info);
    }
    void DisplayUserInfo(string info)
    {
        if (info != "" && CheckIfloginValid() && (!PlayerPrefs.HasKey("SignOut") || PlayerPrefs.GetInt("SignOut") == 0))
        {
            Debug.Log(info);
            FirebaseUser pl = JsonUtility.FromJson<FirebaseUser>(info);
            /*ebug.Log(pl.email);
             Debug.Log(pl.uid);
             Debug.Log(pl.isEmailVerified);
             Debug.Log(pl.displayName);*/
            SignedIn("Signed in as ".ToUpper()+pl.email.ToUpper()+"\n\n"+pl.providerData);
        }

    }
    void SignedIn(string info)
    {
        PlayerPrefs.SetInt("SignOut", 0);
        InfoDisplay.text = info.ToUpper();
        currentOpenWindiow.SetActive(false);
        PlayerPrefs.SetString("Account", "0xD408B954A1Ec6c53BE4E181368F1A54ca434d2f3");
        gameplayView.instance.isTryout = false;
        //change what loads when mint nft added and stuff linked
        GetComponentInParent<NFTGetView>().Skip();
        PlayerPrefs.SetString("LastLogin", System.DateTime.Now.ToBinary().ToString());
        //Debug.Log(PlayerPrefs.GetString("LastLogin"));

    }
    
    public void SignOut()
    {
        PlayerPrefs.SetInt("SignOut", 1);
        GetComponentInParent<uiView>().goToMenu("login");
        InfoDisplay.text = "";
    }

    void DisplayError(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        if (currentOpenWindiow.name == "Login")
        {

            warningLoginText.text = parsedError.message.ToUpper();
        }
        else if (currentOpenWindiow.name == "Register")
        {

            warningRegisterText.text = parsedError.message.ToUpper();
        }
        else
            Debug.Log(parsedError.message);
    }


    #region utility

    public void OpenSingin()
    {
        if(currentOpenWindiow==null)
        {
            currentOpenWindiow = methodSelect;
        }
        currentOpenWindiow.SetActive(false);
        currentOpenWindiow = SignInPanel.gameObject;
        SignInPanel.gameObject.SetActive(true);
    }

    public void OpenRegister()
    {
        if (currentOpenWindiow == null)
        {
            currentOpenWindiow = methodSelect;
        }
        currentOpenWindiow.SetActive(false);
        currentOpenWindiow = registerPanel.gameObject;
        registerPanel.gameObject.SetActive(true);
    }
    public void Close()
    {
        if (currentOpenWindiow != null)
        {
            currentOpenWindiow.SetActive(false);
            currentOpenWindiow = methodSelect;
        }
    }
    bool IsValidEmail(string email)
    {
        Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",RegexOptions.IgnoreCase);

        return emailRegex.IsMatch(email);
    }
   

    bool CheckIfloginValid()
    {
        if (PlayerPrefs.HasKey("LastLogin"))
        {
            DateTime currentDate = System.DateTime.Now;

            long temp = Convert.ToInt64(PlayerPrefs.GetString("LastLogin"));
            DateTime oldDate = DateTime.FromBinary(temp);
            TimeSpan difference = currentDate.Subtract(oldDate);
            if (difference.TotalMinutes >= 5)
                return false;
            else
                return true;
        }
        else
            return false;

    }

    public void LoginBtn()
    {
        Debug.Log("login clicked");
        methodSelect.SetActive(true);
    }

        #endregion utility
}

