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
    int playerHelth;
    [SerializeField]
    int playerDammage;
    States currentState;
    [SerializeField]
    Image bearHealth, playerHelthDisplay;
    int bearNumber=0;

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
    private void Start()
    {
        bearNumber = LoaderScript.instance.bearNumber;
        playerAnim = GetComponent<Animator>();
        //InputsToDisplay();
        //slider.StartSlider();
    }

    public void StartFight()
    {
        bearNumber++;
        if (bearNumber == 3)
        {
            ActivateInputs();
            bearC = bear.GetComponentInChildren<BearController>();
            bearC.StartFight();
            UpdateValues();

        }
        else
        {
            ActivateInputs();
            playerAnim.applyRootMotion = true;
            bearC = bear.GetComponentInChildren<BearController>();
            bearC.StartFight();
            CC = GetComponent<CharacterController>();
            GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = false;
            GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
            GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CC.enabled = false;
            CC.transform.position = bear.transform.GetChild(2).position;
            CC.transform.LookAt(new Vector3(bear.transform.position.x, CC.transform.position.y, bear.transform.position.z));
            playerAnim.SetBool("Fight", true);
            bear.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().Priority = 11;
            UpdateValues();

            InputsToDisplay();
        }

    }
    public void ExitFight()
    {
        fightCanvas.SetActive(false);
        playerAnim.applyRootMotion = false;
        if (bearNumber != 3)
        {
            GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
            GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
            GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CC.enabled = true;
        }
        playerAnim.SetBool("Fight", false);
        bear.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().Priority = 9;
    }

    void ActivateInputs()
    {
        fightCanvas.SetActive(true);
        if (bearNumber == 1)
        {
            currentString = new string[sequenceLength];
            actionList = new bool[sequenceLength];
            sequenceInputs.SetActive(true);
            gaugeInputs.SetActive(false);
        }
        else if (bearNumber == 2)
        {
            gaugeInputs.SetActive(true);
            sequenceInputs.SetActive(false);
        }
        else if(bearNumber==3)
        {
            gaugeInputs.SetActive(false);
            sequenceInputs.SetActive(false);
        }
    }
    public void Punch()
    {
        if (currentState != States.Hit && currentState != States.Dead)
        {
            if (currentState != States.Blocking)
            {
                currentState = States.Attacking;
                int p = Random.Range(1, 3);
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
                int p = Random.Range(1, 3);
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


    public void TakeDammage(int ammount)
    {
        if (currentState != States.Hit)
        {
            playerAnim.SetBool("Fight", true);
            if (currentState != States.Blocking)
            {
                currentState = States.Hit;
                playerHelth -= ammount;
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
                currentState = States.Hit;
                playerAnim.SetTrigger("Hit");
            }


        }

    }

    public void UpdateValues()
    {
        playerHelthDisplay.fillAmount = playerHelth/100f;
        bearHealth.fillAmount = bearC.GetBearHelth()/100f;
    }

    public void ResetAnim()
    {
       
        currentState = States.Idel;
        if (bearC.GetState() != States.Dead && bearNumber != 3)
        {
            transform.DOMove(bear.transform.GetChild(2).position, 0.5f);
            transform.DORotate(Vector3.zero, 0.5f);
        }
        playerAnim.SetBool("Block", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bear") && other.GetComponentInChildren<BearController>()!=null && other.GetComponentInChildren<BearController>().GetState()!=States.Dead)
        {
            bear = other.gameObject;
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
        CC.enabled = false;
        transform.position = new Vector3(0, 0, -45);
        CC.enabled = true;
        SceneManager.LoadScene(0);
        MapController.MC.ResetMap();
    }
    public int GetDammage()
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

    public BearController GetBear()
    {
        return bearC;
    }
    public int GetBearNumber()
    {
        return bearNumber;
    }

    //for new inputs

    KeyCode[] GenerateSequence(int length)
    {
        KeyCode[] sequence = new KeyCode[length];
        for (int i = 0; i < length; i++)
        {
            var key = validSequenceKeys[Random.Range(0, validSequenceKeys.Length)];
            sequence[i] = key;
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
        if (bearNumber == 1)
        {
            timer.fillAmount = 1;
            timeleft = timeToReset;
            currentIndex = 0;
            actionIndex = 0;
            string tempdisp = "";
            int index = 0;
            foreach (KeyCode k in currentSequence)
            {
                tempdisp += k.ToString();
                currentString[index] = k.ToString();
                actionList[index] = false;
                index++;
            }
            inputDisplay.text = tempdisp;
        }
        else if(bearNumber==2)
        {
            currentKey = GenerateSequence(1)[0];
            slider.SetInput(currentKey.ToString());
            slider.StartSlider();
        }

    }

    private void Update()
    {
        if(takingIputs)
        {
            if (bearNumber == 1)
            {
                if (timeleft >= 0 && currentIndex < sequenceLength)
                {
                    timeleft -= Time.deltaTime;
                    timer.fillAmount = timeleft / timeToReset;
                    //Debug.Log(timeleft);
                    if (Input.GetKeyDown(currentSequence[currentIndex]))
                    {
                        string[] tempDisp = currentString;
                        for (int i = 0; i < currentSequence.Length; i++)
                        {
                            if (i == currentIndex)
                            {
                                tempDisp[i] = "<color=green>" + currentSequence[i] + "</color>";
                            }
                            else
                            {
                                tempDisp[i] = currentString[i];
                            }
                        }
                        actionList[currentIndex] = true;
                        currentIndex++;
                        inputDisplay.text = ConvertToString(tempDisp);
                    }
                    else if (Input.anyKeyDown && !Input.GetKeyDown(currentSequence[currentIndex]))
                    {
                        string[] tempDisp = currentString;
                        for (int i = 0; i < currentSequence.Length; i++)
                        {
                            if (i == currentIndex)
                            {
                                tempDisp[i] = "<color=red>" + currentSequence[i] + "</color>";
                            }
                            else
                            {
                                tempDisp[i] = currentString[i];
                            }
                        }
                        actionList[currentIndex] = false;
                        currentIndex++;
                        inputDisplay.text = ConvertToString(tempDisp);

                    }
                }
                else
                {
                    takingIputs = false;
                    PlayActions();

                }
            }

            else if(bearNumber==2 )
            {
                if(Input.GetKeyDown(currentKey))
                {
                    currentValue = slider.StopSlider();
                    PlaySingleActions();
                }
            }

            
        }
        if (bearNumber == 3)
        {
           
            if (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1))
            {
                playerAnim.applyRootMotion = true;
                playerAnim.SetBool("Fight", true);
                Kick();
            }
            if(Input.GetKeyDown(KeyCode.P) || Input.GetMouseButtonDown(0))
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

    private void PlaySingleActions()
    {
        if (Mathf.Abs(currentValue - 0.5f) < 0.2f)
        {
            if (currentKey.ToString() == "P")
            {
                transform.DOMove((bear.transform.GetChild(2).position + (bear.transform.position - transform.position) * 0.15f), 0.5f);
                Punch();
            }
            else if (currentKey.ToString() == "K")
            {
                transform.DOMove((bear.transform.GetChild(2).position + (bear.transform.position - transform.position) * 0.15f), 0.5f);
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
    }

    public void PlayActions()
    {
        if (bearNumber == 1)
        {
            if (actionIndex < sequenceLength)
            {
                if (!actionList[actionIndex])
                {
                    bearC.StartAttack(0.5f);
                }
                else
                {
                    if (currentSequence[actionIndex].ToString() == "P")
                    {
                        transform.DOMove((bear.transform.GetChild(2).position + (bear.transform.position - transform.position) * 0.15f), 0.5f);
                        Punch();
                    }
                    else if (currentSequence[actionIndex].ToString() == "K")
                    {
                        transform.DOMove((bear.transform.GetChild(2).position + (bear.transform.position - transform.position) * 0.15f), 0.5f);
                        Kick();
                    }
                    else if (currentSequence[actionIndex].ToString() == "B")
                    {
                        Block();
                        bearC.StartAttack(0.5f);
                    }
                }
                actionIndex++;
            }
            else
            {
                InputsToDisplay();
            }
        }

        if (bearNumber == 2)
        {
            InputsToDisplay();
        }
        if (bearNumber == 3)
        {
            playerAnim.applyRootMotion = false;
            playerAnim.SetBool("Fight", false);
        }
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
/* old method 
 *  if(takingIputs)
        {
            if(timeleft>=0)
            {
                timeleft -= Time.deltaTime;
                timer.fillAmount = timeleft/timeToReset;
                //Debug.Log(timeleft);
                if(Input.GetKeyDown(currentSequence[currentIndex]))
                {
                    takingIputs = false;
                    inputDisplay.text = "<color=green>" + inputDisplay.text + "</color>";
                    if (currentSequence[currentIndex].ToString() == "P")
                    {
                        Punch();
                    }
                    else if (currentSequence[currentIndex].ToString() == "K")
                    {
                        Kick();
                    }
                    else if (currentSequence[currentIndex].ToString() == "B")
                    {
                        Block();
                        bearC.StartAttack(0.5f);
                    }
                    currentIndex++;
                    
                }
                else if (Input.anyKeyDown && !Input.GetKeyDown(currentSequence[currentIndex]))
                {
                    takingIputs = false;
                    inputDisplay.text = "<color=red>" + inputDisplay.text + "</color>";
                    currentIndex++;
                    bearC.StartAttack(0.5f);
                    
                }
            }
            else
            {
                takingIputs = false;
                inputDisplay.text = "<color=red>" + inputDisplay.text + "</color>";
                currentIndex++;
                bearC.StartAttack(0.5f);
            }
        }
    }
*/