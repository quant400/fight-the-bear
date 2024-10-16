using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FightController : MonoBehaviour
{
    Animator playerAnim;
    GameObject bear;
    BearController bearC;
    CharacterController CC;

    [SerializeField]
    float playerHelth;
    [SerializeField]
    float playerDammage;
    private Quaternion originalRot;
    States currentState;
    AttackingStates currentAttackingState;
    public int bearNumber = 0;
    [SerializeField]

    float atk, def, tek;

    //for new input 
    KeyCode[] validSequenceKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    KeyCode[] currentSequence;
    int currentIndex;
    //[SerializeField]
    //float timeToReset;
    //float timeleft;
    private bool takingIputs;
    [SerializeField]
    int sequenceLength;
    string[] currentString;
    bool[] actionList;
    int actionIndex;
    public bool actionDone;

    //slider input
    KeyCode currentKey;
    float currentValue;

    bool inFight = false;
    bool specialAttack = false;
    public bool canHit = true;

    float score;

    float timeCounter;
    [SerializeField]
    float startingTime;
    [SerializeField]
    float TimeAddedPerBear;

    private bool shield = false;
    [SerializeField]
    GameObject shieldObject;
    [SerializeField]
    GameObject attackObject;
    public bool died;
    bool stoptimer = false;
    public bool timeEnded = false;
    bool punch = true;
    public string FightStyle; // later get from chosen nft

    public PlayerSFXController pSFXC;

    DamageDisplay DD;

    private void Awake()
    {
        //bearNumber = LoaderScript.instance.bearNumber;
        playerAnim = GetComponent<Animator>();
        //InputsToDisplay();
        //slider.StartSlider();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CC = GetComponent<CharacterController>();
        FightStyle = "boxing";
        pSFXC = GetComponent<PlayerSFXController>();
        DD = GetComponentInChildren<DamageDisplay>();

    }

    #region Fight Functions
    public void StartFight(GameObject b)
    {
        bear = b;
        bearNumber++;
        inFight = true;
        GameUIView.instance.ActivateInputs();
        bearC = bear.GetComponent<BearController>();
        stoptimer = false;
        //bearC.StartFight();
        UpdateValues();
    }
    public void ExitFight()
    {
        GameUIView.instance.DeactivateFightCanvas();
        //fightCanvas.SetActive(false);
        //playerAnim.applyRootMotion = false;
        playerAnim.SetBool("Block", false);
        playerAnim.SetBool("Fight", false);
        currentState = States.Idel;
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        inFight = false;
       /* if (!died && !timeEnded)
        {
            score += startingTime;
            startingTime += TimeAddedPerBear;
            
        }*/
        GameUIView.instance.UpdateTimerVal(startingTime);
        //timerDisplay.text = ("Time Left : ").ToUpper() + startingTime.ToString("00");

    }

  
    void EnableSpecialAttack()
    {
        specialAttack = true;
        //playerAnim.applyRootMotion = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CC.transform.position = bear.transform.GetChild(5).position + -bear.transform.forward*bearNumber;
        CC.transform.LookAt(new Vector3(bear.transform.position.x, CC.transform.position.y, bear.transform.position.z));
        playerAnim.SetBool("Fight", true);
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 11;
        Invoke("InputsToDisplay", 1);
        DisableMovement();

    }
    public void DisableSpecialAttack()
    {
        specialAttack = false;
        CC.transform.position = bear.transform.GetChild(5).position;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        GameUIView.instance.DisableGauge();
        //gaugeInputs.SetActive(false);
        currentState = States.Idel;
        playerAnim.SetBool("Fight", false);
        EnableMovement();
    }

    public void Punch()
    {
        if (currentState != States.Hit && currentState != States.Dead)
        {
            if (currentState != States.Blocking)
            {
                originalRot = transform.localRotation;
                currentState = States.Attacking;
                currentAttackingState = AttackingStates.punching;
                DisableMovement();
                int p = Random.Range(1, 3);
                if (specialAttack)
                    p = 10;
                pSFXC.PlaySwoosh();
                playerAnim.SetFloat("PunchVal", p);
                playerAnim.SetTrigger("Punch");
            }
            /*else
            {
                playerAnim.SetBool("Block", false);
                currentState = States.Idel;
                Punch();
            }*/
        }
    }

    public void Kick()
    {
        if (currentState != States.Hit && currentState != States.Dead)
        {
            if (currentState != States.Blocking)
            {
                originalRot = transform.localRotation;
                currentState = States.Attacking;
                currentAttackingState = AttackingStates.kicking;
                DisableMovement();
                int p = Random.Range(1, 3);
                if (specialAttack)
                    p = 10;
                pSFXC.PlaySwoosh();
                playerAnim.SetFloat("KickVal", p);
                playerAnim.SetTrigger("Kick");
            }
           /* else
            {
                playerAnim.SetBool("Block", false);
                currentState = States.Idel;
                Kick();
            }*/
        }
    }

    public void Block()
    {
        if (currentState == States.Idel)
        {

            playerAnim.SetBool("Fight", true);
            currentState = States.Blocking;
            DisableMovement();
            playerAnim.SetBool("Block", true);
        }

        //StartCoroutine(ResetAnim());
    }

    void StopBlock()
    {
        currentState = States.Idel;
        playerAnim.SetBool("Block", false);
        playerAnim.SetBool("Fight", false);
        EnableMovement();
       
    }
    public void TakeDammage(float ammount)
    {
        if(playerAnim.GetBool("Jump"))
        {
            playerAnim.SetBool("Jump", false);
            playerAnim.SetBool("Grounded", true);
            
        }
        if (currentState != States.Hit && !shield)
        {
            pSFXC.Playhit();
            playerAnim.SetBool("Fight", true);
            if (currentState != States.Blocking)
            {
                currentState = States.Hit;
                playerHelth -= ammount;
                score -= ammount;
                if (score < 0)
                    score = 0;
                DD.DisplayDamage(ammount);
                UpdateValues();
                if (playerHelth <= 0)
                {
                    DisableMovement();
                    died = true;
                    Time.timeScale = 0.5f;
                    currentState = States.Dead;
                    //playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Dead", true);
                    Invoke("RestoreTime", 2f);
                    //ExitFight();
                }
                else
                {
                    //PushBack(1);
                    DisableMovement();
                    playerAnim.SetTrigger("Hit");
                }

            }
            else
            {

                currentState = States.Hit;
                playerHelth -= (ammount - 10);
                score -= ammount;
                if (score < 0)
                    score = 0;
                DD.DisplayDamage(ammount - 10);
                //playerHelth -= ((ammount/2) * (1-(def/100)));
                UpdateValues();
                if (playerHelth <= 0)
                {
                    DisableMovement();
                    died = true;
                    currentState = States.Dead;
                    Time.timeScale = 0.5f;
                    playerAnim.SetBool("Block", false);
                    playerAnim.SetBool("Dead", true);
                    Invoke("RestoreTime", 2f);
                    //Die();
                }
                else
                {
                    //EnableSpecialAttack();
                    bearC.Stunned();
                    playerAnim.SetTrigger("Hit");
                }
            }
        }
    }
    public void ActivateShield()
    {
        shield = true;
        shieldObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void DeactivateShield()
    {
        shield = false;
        shieldObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void StopTimer()
    {
        stoptimer = true;
    }
    #endregion Fight Functions
    public void UpdateValues()
    {
        /* playerHelthDisplay.fillAmount = playerHelth/100f;
         bearHealth.fillAmount = bearC.GetBearHelth()/(100f+25f*(float)(bearNumber-1));
         scoreDisplay.text = ("Score : ").ToUpper() + score;*/
        float ph = playerHelth / 100f;
        float bh = bearC.GetBearHelth() / (100f + 25f * (float)(bearNumber - 1));
        GameUIView.instance.UpdateValues(ph, bh, score);
    }

    public void ResetAnim()
    {

        if (!timeEnded && !specialAttack)
        {
            if (currentState == States.Attacking)
            {
                //transform.localRotation = originalRot;
                Invoke("EnableMovement", 0.5f);
            }
            else
                EnableMovement();
        }
        canHit = true;
        currentState = States.Idel;
        playerAnim.SetBool("Block", false);
        playerAnim.ResetTrigger("Hit");

    }
    public void DisableMovement()
    {
        CC.enabled = false;
        if (currentState != States.Blocking)
        {
            GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
        }

        if (currentState != States.Attacking && currentState != States.Blocking)
        {
            GetComponent<StarterAssets.ThirdPersonController>().enabled = false;

        }
    }
    public void EnableMovement()
    {
        CC.enabled = true;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;

    }
    void EnableAttack()
    {
        canHit = true;
    }
    void EnableInputs()
    {
        takingIputs = true;
    }

   

    public void GivePoints(float val)
    {
        score += val;
       
    }
     public float GetScore()
    {
        return score;
    }
 

   
  
    public void MoveToNext()
    {
        currentState = States.Idel;
        
        SceneManager.LoadScene(1);
        MapView.instance.ResetMap();
    }

    #region SetandGetFunction 
    public void SetStats(float a, float d, float t)
    {
        atk = a;
        def = d;
        tek = t;

        //playerDammage *= (1 + atk/100);
    }
    /*public void SetAggression(float val)
    {
        bearAgression.fillAmount = val;
    }*/

    public float GetDammage()
    {
        return playerDammage;
    }
    public void SetState(States s)
    {
        currentState = s;
    }
    public States GetState()
    {
        return currentState;
    }
    public AttackingStates GetAttackingState()
    {
        return currentAttackingState;
    }

    public BearController GetBear()
    {
        return bearC;
    }
    public int GetBearNumber()
    {
        return bearNumber;
    }

    public bool GetSpecialAttackStatus()
    {
        return specialAttack;
    }

    public float GetCurrentSliderVal()
    {
        return currentValue;
    }

    public void setAttackDamage(int x)
    {
        playerDammage +=x;
        if (x > 0)
            attackObject.transform.GetChild(0).gameObject.SetActive(true);
        else
            attackObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion 
    
    
    
    //for new inputs

    KeyCode[] GenerateSequence(int length)
    {
        KeyCode[] sequence = new KeyCode[length];
        for (int i = 0; i < length; i++)
        {
            KeyCode k;
            if(specialAttack)
                k = validSequenceKeys[Random.Range(0, validSequenceKeys.Length-1)];
            else
                k = validSequenceKeys[Random.Range(0, validSequenceKeys.Length)];
            sequence[i] = k;
        }

        return sequence;
    }

    void InputsToDisplay()
    {
        currentSequence = GenerateSequence(sequenceLength);
        DisplayCurrentInput();
        GameUIView.instance.EnableGauge();
        //gaugeInputs.SetActive(true);
    }

    public void DisplayCurrentInput()
    {
        EnableInputs();
        if(specialAttack)
        {
            currentKey = GenerateSequence(1)[0];
            GameUIView.instance.SetSliderInput(currentKey.ToString());
            /*slider.SetInput(currentKey.ToString());
            slider.StartSlider();*/
        }

    }

    private void Update()
    {
        //Debug.Log((currentState, died));
        if (inFight && currentState!=States.Dead && !timeEnded )
        {
            if (takingIputs)
            {

                if (specialAttack)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        currentValue = GameUIView.instance.GetSliderValue();
                        PlaySingleActions();

                    }
                }

            }
            else if(!specialAttack && CC.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    punch = !punch;
                if (Input.GetMouseButtonDown(0))
                {
                    //playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Fight", true);
                    if (punch)
                        Punch();
                    else
                        Kick();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Fight", true);
                    Block();
                }
                if (Input.GetMouseButtonUp(1))
                {
                    StopBlock();
                }
            }
           
        }

       
    }

    private void PlaySingleActions()
    {
        
        if (Mathf.Abs(currentValue - 0.5f) < 0.2f)
        {
            if (currentKey.ToString() == "Alpha1")
            {
                transform.DOMove((bear.transform.GetChild(5).position + (bear.transform.position - transform.position) * 0.2f), 0.5f);
                Punch();
            }
            else if (currentKey.ToString() == "Alpha2")
            {
                transform.DOMove((bear.transform.GetChild(5).position + (bear.transform.position - transform.position) * 0.2f), 0.5f);
                Kick();
            }
            else if (currentKey.ToString() == "Alpha3")
            {
                Block();
                bearC.StartAttack(0.5f);
            }
        }
        else
        {
            bearC.StartAttack(0.5f);
        }
        takingIputs = false;
    }

    public void PlayActions()
    {
        if (inFight && !specialAttack && !died)
        {
            //playerAnim.applyRootMotion = false;
            playerAnim.SetBool("Fight", false);
        }
    }

    public void PushBack(float dist)
    {
       // Debug.Log(("PushBack",currentState,CC.enabled));
        CC.enabled = false;
        transform.DOMove(transform.position + (bear.transform.forward * dist), 0.5f).OnComplete(()=>CC.enabled=true);
        //CC.enabled = true;
    }
   


    public void Die()
    {
        if (specialAttack)
            DisableSpecialAttack();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //score += startingTime;
        gameplayView.instance.SetLocalScore(score);
        /*if (died)
            GameUIView.instance.DisplayGameOver("WASTED!", score);
        else
        {
            GameUIView.instance.DisplayGameOver("You ran out of time.", score);
           
        }*/
        
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGameEnded;
    }
    private string ConvertToString(string[] tempDisp)
    {
        string result="";
       foreach(string x in tempDisp)
        {
            result += x;
        }
        return result;
    }

    void RestoreTime()
    {
        Time.timeScale = 1f;
    }
   
  
}
