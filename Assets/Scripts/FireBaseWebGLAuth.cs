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
    public bool accpetedTos;

    [Header("PasswordReset")]
    public Transform passwordResetPanel;
    public TMP_InputField emailPasswordReset;
    public TMP_Text warningEmailReset;

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
        else if(!accpetedTos)
        {
            registerPanel.DOShakePosition(1, 1);
            warningRegisterText.text = "please read and accept the terms of servive and privacy policy".ToUpper();
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

    public void SignInWithGoogle()=>
        FirebaseAuth.SignInWithGoogle(gameObject.name, "SignedIn", "DisplayError");

    
    public void ResetPasswordEmail()=>
        FirebaseAuth.ResetPassword(emailPasswordReset.text, gameObject.name, "DisplayResetReply", "DisplayResetReply");



    void DisplayInfo(string info)
    {
        Debug.Log(info);
    }

    void DisplayResetReply(string info)
    {
        warningEmailReset.text = info.ToUpper();
    }
    void DisplayUserInfo(string info)
    {
        if (info != "")
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
        InfoDisplay.text = info.ToUpper();
        currentOpenWindiow.SetActive(false);
        currentOpenWindiow = methodSelect;
        PlayerPrefs.SetString("Account", "0xD408B954A1Ec6c53BE4E181368F1A54ca434d2f3");
        gameplayView.instance.isTryout = false;
        //change what loads when mint nft added and stuff linked
        GetComponentInParent<NFTGetView>().Display(new NFTInfo[0]);

    }
    
    public void LogOut()
    {
        FirebaseAuth.SignOut();
        GetComponentInParent<uiView>().goToMenu("login");
        InfoDisplay.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
        emailLoginField.text = "";
        passwordLoginField.text = "";
        warningLoginText.text = "";
        warningEmailReset.text = "";
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
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
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
        emailLoginField.text = "";
        passwordLoginField.text = "";
        warningLoginText.text = "";
        currentOpenWindiow.SetActive(false);
        currentOpenWindiow = registerPanel.gameObject;
        registerPanel.gameObject.SetActive(true);
    }

    public void OpenPasswordReset()
    {
        if (currentOpenWindiow == null)
        {
            currentOpenWindiow = methodSelect;
        }
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
        warningRegisterText.text = "";
        currentOpenWindiow.SetActive(false);
        currentOpenWindiow = passwordResetPanel.gameObject;
        passwordResetPanel.gameObject.SetActive(true);
    }
    public void Close()
    {
        if (currentOpenWindiow != null)
        {
            currentOpenWindiow.SetActive(false);
            currentOpenWindiow = methodSelect;
            emailRegisterField.text = "";
            passwordRegisterField.text = "";
            passwordRegisterVerifyField.text = "";
            warningRegisterText.text = "";
            emailLoginField.text = "";
            passwordLoginField.text = "";
            warningLoginText.text = "";
            emailPasswordReset.text = "";
            warningEmailReset.text = "";
        }
    }
    public void OpenMethodSelect()
    {
        methodSelect.SetActive(true);
    }
    bool IsValidEmail(string email)
    {
        Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",RegexOptions.IgnoreCase);

        return emailRegex.IsMatch(email);
    }


    public void ToggleTos(bool val)
    {
        accpetedTos = val;
    }
    public void LoadTos()
    {
        Application.OpenURL("https://www.cryptofightclub.io/terms-of-service");
    }
    public void LoadPrivacy()
    {
        Application.OpenURL("https://www.cryptofightclub.io/privacy-policy");
    }
 #endregion utility
       
}

