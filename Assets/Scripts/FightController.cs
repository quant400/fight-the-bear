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
    public KeyCode[] validSequenceKeys = new KeyCode[] { KeyCode.P, KeyCode.K, KeyCode.B };
    KeyCode[] currentSequence;
    int currentIndex;
    [SerializeField]
    TMP_Text inputDisplay;
    [SerializeField]
    float timeToReset;
    float timeleft;
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
    public bool canHit;

    float score;
    [SerializeField]
    TMP_Text scoreDisplay;
    float timeCounter;
    [SerializeField]
    float totalAllowedTime;
    private void Start()
    {
        bearNumber = LoaderScript.instance.bearNumber;
        playerAnim = GetComponent<Animator>();
        //InputsToDisplay();
        //slider.StartSlider();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void StartFight()
    {
        bearNumber++;
        inFight = true;
        ActivateInputs();
        bearC = bear.GetComponent<BearController>();
        bearC.StartFight();
        UpdateValues();



    }
    public void ExitFight()
    {
        fightCanvas.SetActive(false);
        playerAnim.applyRootMotion = false;
        playerAnim.SetBool("Fight", false);
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        inFight = false;
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
        CC = GetComponent<CharacterController>();
        playerAnim.applyRootMotion = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CC.enabled = false;
        CC.transform.position = bear.transform.GetChild(5).position;
        CC.transform.LookAt(new Vector3(bear.transform.position.x, CC.transform.position.y, bear.transform.position.z));
        playerAnim.SetBool("Fight", true);
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 11;
        
        InputsToDisplay();
        gaugeInputs.SetActive(true);
    }
   public void DisableSpecialAttack()
    {
        specialAttack = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CC.enabled = true;
        bear.transform.GetChild(4).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        gaugeInputs.SetActive(false);
    }

    public void Punch()
    {
        if (currentState != States.Hit && currentState != States.Dead)
        {
            if (currentState != States.Blocking)
            {
                currentState = States.Attacking;
                currentAttackingState = AttackingStates.punching;
                int p = Random.Range(1, 3);
                if (specialAttack)
                    p = 10;
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

                currentState = States.Attacking;
                currentAttackingState = AttackingStates.kicking;
                int p = Random.Range(1, 3);
                if (specialAttack)
                    p = 10;
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
        playerAnim.SetBool("Fight", true);
        currentState = States.Blocking;
        playerAnim.SetBool("Block",true);
        //StartCoroutine(ResetAnim());
    }


    public void TakeDammage(float ammount)
    {
        if (currentState != States.Hit)
        {
            playerAnim.SetBool("Fight", true);
            if (currentState != States.Blocking)
            {
                currentState = States.Hit;
                playerHelth -= ammount;
                score -= ammount;
                UpdateValues();
                if (playerHelth <= 0)
                {
                    playerAnim.SetBool("Dead", true);
                    currentState = States.Dead;
                    //ExitFight();
                }
                else
                {

                    playerAnim.SetTrigger("Hit");
                }

            }
            else
            {

                EnableSpecialAttack();
                bearC.Stunned();

                currentState = States.Hit;
                playerHelth -= (ammount - 10);
                score -= (ammount - 10);
                //playerHelth -= ((ammount/2) * (1-(def/100)));
                UpdateValues();
                if (playerHelth <= 0)
                {
                    playerAnim.SetBool("Dead", true);
                    currentState = States.Dead;
                    //ExitFight();
                }
                else
                {
                    playerAnim.SetTrigger("Hit");
                }
            }
        }
    }

    public void UpdateValues()
    {
        playerHelthDisplay.fillAmount = playerHelth/100f;
        bearHealth.fillAmount = bearC.GetBearHelth()/(100f+25f*(float)(bearNumber-1));
        scoreDisplay.text = "Score : " + score;
    }

    public void ResetAnim()
    {
       
        currentState = States.Idel;
        canHit = true;
        playerAnim.SetBool("Block", false);
    }

    public void GivePoints(float val)
    {
        score += val;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bear") && other.GetComponentInChildren<BearController>()!=null && other.GetComponentInChildren<BearController>().GetState()!=States.Dead && !inFight)
        {
            bear = other.transform.GetChild(0).gameObject;
            other.enabled = false;
            StartFight();
        }
        if (other.CompareTag("NextTrigger"))
        {
            other.enabled = false;
            MapController.MC.SpawnNext();
        }
    }
    
    public void ResetGame()
    {
        //disabled for testing
        /* playerHelth = 100;
         currentState = States.Idel;
         playerAnim.SetBool("Dead", false);
         transform.position = new Vector3(0, 0, -45);
         ExitFight();*/
        
        SceneManager.LoadScene(0);
        MapController.MC.ResetMap();
        //enabled for testing
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
    }

    public void DisplayCurrentInput()
    {
        takingIputs = true;
        if(specialAttack)
        {
            currentKey = GenerateSequence(1)[0];
            slider.SetInput(currentKey.ToString());
            slider.StartSlider();
        }

    }

    private void Update()
    {
        if (inFight)
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
            else
            {
                if (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1))
                {
                    playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Fight", true);
                    Kick();
                }
                if (Input.GetKeyDown(KeyCode.P) || Input.GetMouseButtonDown(0))
                {
                    playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Fight", true);
                    Punch();
                }
                if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    playerAnim.applyRootMotion = true;
                    playerAnim.SetBool("Fight", true);
                    Block();
                }
            }
        }
    }

    private void PlaySingleActions()
    {
        if (Mathf.Abs(currentValue - 0.5f) < 0.2f)
        {
            if (currentKey.ToString() == "P")
            {
                Debug.Log("1");
                transform.DOMove((bear.transform.GetChild(5).position + (bear.transform.position - transform.position) *0.2f), 0.5f);
                Punch();
            }
            else if (currentKey.ToString() == "K")
            {
                Debug.Log("1");
                transform.DOMove((bear.transform.GetChild(5).position + (bear.transform.position - transform.position) * 0.2f), 0.5f);
                Kick();
            }
            else if (currentKey.ToString() == "B")
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
        if (inFight && !specialAttack)
        {
            playerAnim.applyRootMotion = false;
            playerAnim.SetBool("Fight", false);
        }
    }

    public void PushBack(float dist)
    {
        CC.enabled = false;
        transform.position += (Vector3.back * dist);
        CC.enabled = true;
    }
   


    void Die()
    {

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
}
