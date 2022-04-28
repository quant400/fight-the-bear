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
    GameObject fightCanvas;
    [SerializeField]
    float playerHelth;
    [SerializeField]
    float playerDammage;
    States currentState;
    AttackingStates currentAttackingState;
    [SerializeField]
    Image bearHealth, playerHelthDisplay;
    int bearNumber=0;
    [SerializeField]
    Image bearAgression;
    float atk, def, tek;

    //for new input 
    KeyCode[] validSequenceKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    KeyCode[] currentSequence;
    int currentIndex;
    [SerializeField]
    TMP_Text inputDisplay;
    //[SerializeField]
    //float timeToReset;
    //float timeleft;
    private bool takingIputs;
    [SerializeField]
    Image timer;
    [SerializeField]
    int sequenceLength;
    string[] currentString;
    bool[] actionList;
    int actionIndex;
    public bool actionDone;

    //slider input
    [SerializeField]
    SliderScript slider;
    KeyCode currentKey;
    float currentValue;


    [SerializeField]
    GameObject sequenceInputs, gaugeInputs;

    bool inFight =false;
    bool specialAttack = false;
    public bool canHit=true;

    float score;
    [SerializeField]
    TMP_Text scoreDisplay;
    [SerializeField]
    TMP_Text timerDisplay;
    float timeCounter;
    [SerializeField]
    float startingTime;
    [SerializeField]
    float TimeAddedPerBear;
    [SerializeField]
    TMP_Text CutSceneText;
    [SerializeField]
    GameObject cutSceneImage;
   
    private bool shield=false;
    [SerializeField]
    GameObject shieldObject;
    [SerializeField]
    GameObject attackObject;
    [SerializeField]
    GameObject gameOverPanel;
    public bool died;
    bool stoptimer = false;
    public bool timeEnded=false;
    bool punch=true;
    public string FightStyle; // later get from chosen nft

    public PlayerSFXController pSFXC;

    DamageDisplay DD;

    Quaternion originalRot;

    //for alternate score
    float score2,score3;
    float timer2, timer3;
    bool timing2, timing3;
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
        ActivateInputs();
        bearC = bear.GetComponent<BearController>();
        stoptimer = false;
        //bearC.StartFight();
        StartT2();
        UpdateValues();
    }
    public void ExitFight()
    {
        fightCanvas.SetActive(false);
        //playerAnim.applyRootMotion = false;
        playerAnim.SetBool("Block", false);
        playerAnim.SetBool("Fight", false);
        currentState = States.Idel;
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        inFight = false;
        if (!died && !timeEnded)
        {
            startingTime += TimeAddedPerBear;
            StopT2();
        }
        timerDisplay.text = ("Time Left : ").ToUpper() + startingTime.ToString("00");
       
    }

    void ActivateInputs()
    {
        fightCanvas.SetActive(true);
        gaugeInputs.SetActive(false);
        sequenceInputs.SetActive(false);

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
        CC.transform.position = bear.transform.GetChild(5).position;
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
        gaugeInputs.SetActive(false);
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
            else
            {
                playerAnim.SetBool("Block", false);
                currentState = States.Idel;
                Punch();
            }
        }
    }

    public void Kick()
    {
        if (currentState != States.Hit && currentState!=States.Dead)
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
            else
            {
                playerAnim.SetBool("Block", false);
                currentState = States.Idel;
                Kick();
            }
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

    public void TakeDammage(float ammount)
    {
        if (currentState != States.Hit && !shield)
        {
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
                    currentState = States.Dead;
                    //playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Dead", true);
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
                DD.DisplayDamage(ammount-10);
                //playerHelth -= ((ammount/2) * (1-(def/100)));
                UpdateValues();
                if (playerHelth <= 0)
                {
                    DisableMovement();
                    died = true;
                    currentState = States.Dead;
                    playerAnim.SetBool("Block", false);
                    playerAnim.SetBool("Dead", true);
                    //Die();
                }
                else
                {
                    EnableSpecialAttack();
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
        playerHelthDisplay.fillAmount = playerHelth/100f;
        bearHealth.fillAmount = bearC.GetBearHelth()/(100f+25f*(float)(bearNumber-1));
        scoreDisplay.text = ("Score : ").ToUpper() + score;
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
        canHit=true;
        currentState = States.Idel;
        playerAnim.SetBool("Block", false);
        playerAnim.ResetTrigger("Hit");
        
    }
    public void DisableMovement()
    {  
        CC.enabled = false;
        if(currentState != States.Blocking)
        {
            GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
        }
        
        if (currentState != States.Attacking && currentState!=States.Blocking)
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

    public void ActivateText(string info)
    {
        CutSceneText.text = info;
        CutSceneText.gameObject.SetActive(true);
        cutSceneImage.SetActive(true);
    } 
    public void DeactivateText()
    {
        CutSceneText.gameObject.SetActive(false);
        cutSceneImage.SetActive(false);
    }

    public void GivePoints(float val)
    {
        score += val;
       
    }
    private void OnTriggerEnter(Collider other)
    {
       /* if(other.CompareTag("Bear") && other.GetComponentInChildren<BearController>()!=null && other.GetComponentInChildren<BearController>().GetState()!=States.Dead && !inFight)
        {
            bear = other.transform.GetChild(0).gameObject;
            other.enabled = false;
            StartFight();
        }
        if (other.CompareTag("NextTrigger"))
        {
            other.enabled = false;
            MapController.MC.SpawnNext();
        }*/
    }


    public void ResetGame()
    {
        /* bearNumber = 0;

         playerAnim.SetBool("Dead", false);
         playerHelth = 100;
         currentState = States.Idel;
         transform.position = new Vector3(0, 0, -45);
         transform.LookAt(Vector3.forward);
         specialAttack = false;
         startingTime = 45;
         timerDisplay.text = "Time Left : " + startingTime.ToString("00");
         score = 0;
         UpdateValues();
         ExitFight();
         EnableMovement();
         died = false;
         timeEnded =false;
         gameOverPanel.SetActive(false);
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
         SceneManager.LoadScene(1);
        */

        TemporaryRestartScript.instance.Reset(gameObject);



    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
    public void MoveToNext()
    {
        currentState = States.Idel;
        //CC.enabled = false;
        //transform.position = new Vector3(0, 0, -45);
        //CC.enabled = true;
        SceneManager.LoadScene(1);
        MapController.MC.ResetMap();
    }

    #region SetandGetFunction 
    public void SetStats(float a, float d, float t)
    {
        atk = a;
        def = d;
        tek = t;

        //playerDammage *= (1 + atk/100);
    }
    public void SetAggression(float val)
    {
        bearAgression.fillAmount = val;
    }

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
        gaugeInputs.SetActive(true);
    }

    public void DisplayCurrentInput()
    {
        EnableInputs();
        if(specialAttack)
        {
            currentKey = GenerateSequence(1)[0];
            slider.SetInput(currentKey.ToString());
            slider.StartSlider();
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
                    if (Input.GetKeyDown(currentKey))
                    {
                        currentValue = slider.StopSlider();
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
            }
            if (!stoptimer)
            {
                startingTime -= Time.deltaTime;
                timerDisplay.text = ("Time Left : ").ToUpper() + startingTime.ToString("00");
                if (timing2 && timer2 > 0)
                    timer2 -= Time.deltaTime;
                if (startingTime <= 0)
                {
                    StopTimer();
                    timeEnded = true;
                    Die();
                }
            }
        }

        if (timing3 && timer3 > 0)
            timer3 -= Time.deltaTime;
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
        if (died)
            gameOverPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "You Died.";
        else
        {
            gameOverPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "You ran out of time.";
        }
        gameOverPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = ((int)(score2+score)).ToString();
        gameOverPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = ((int)(score3 + score)).ToString();
        gameOverPanel.SetActive(true);

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


    //for alternate score 
    public void StartT2()
    {
        timer2 = 50;
        timing2 = true;
    }

    public void StartT3()
    {
        timer3= 50;
        timing3 = true;
        
    }
    public void StopT2()
    {
        timing2 = false;
        if (timer2 >= 0)
        {
            score2 += timer2;
            score3 += timer2;
        }
    }
    public void StoptT3()
    {
        Debug.Log(timer3);
        timing3 = false;
        if(timer3>=0)
            score3 += timer3;
    }


    public (int ,int ,int) GetScores()
    {
        return ((int)score, (int)score2, (int)score3);
    }
}
