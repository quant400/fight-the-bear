using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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

    KeyCode[] currentSequence;
    int currentIndex;
    [SerializeField]
    TMP_Text inputDisplay;
    [SerializeField]
    float timeToReset;
    float timeleft;
    private bool takingIputs;
    private void Start()
    {
        playerAnim = GetComponent<Animator>();
        InputsToDisplay();
    }

    public void StartFight()
    {
        fightCanvas.SetActive(true);
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
        CC.transform.LookAt(new Vector3(bear.transform.position.x,CC.transform.position.y,bear.transform.position.z));
        playerAnim.SetBool("Fight", true);
        bear.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().Priority = 11;
        UpdateValues();

    }
    public void ExitFight()
    {
        fightCanvas.SetActive(false);
        playerAnim.applyRootMotion = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CC.enabled = true;
        playerAnim.SetBool("Fight", false);
        Debug.Log("called");
        bear.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().Priority = 9;
    }

    public void  Punch()
    {
        if (currentState == States.Idel)
        {
            currentState = States.Attacking;
            transform.DOMove((bear.transform.GetChild(2).position+(bear.transform.position - transform.position) * 0.15f),0.5f);
            int p = Random.Range(1, 3);
            playerAnim.SetFloat("PunchVal", p);
            playerAnim.SetTrigger("Punch");
            //StartCoroutine(ResetAnim());
            //bearC.TakeDammage(playerDammage);
            //UpdateValues();
        }
        else if(currentState == States.Blocking)
        {
            currentState = States.Idel;
            playerAnim.SetBool("Block", false);
            Punch();
        }
    }

    public void Kick()
    {
        if (currentState == States.Idel)
        { 
            currentState =States.Attacking;
            transform.DOMove((bear.transform.GetChild(2).position + (bear.transform.position - transform.position) * 0.15f), 0.5f);
            int p = Random.Range(1, 3);
            playerAnim.SetFloat("KickVal", p);
            playerAnim.SetTrigger("Kick");
            //StartCoroutine(ResetAnim());
            //bearC.TakeDammage(playerDammage);
            //UpdateValues();
        }
        else if (currentState == States.Blocking)
        {
            currentState = States.Idel;
            playerAnim.SetBool("Block", false);
            Kick();
        }
    }

    public void Block()
    {
        currentState = States.Blocking;
        playerAnim.SetBool("Block",true);
        //StartCoroutine(ResetAnim());
    }
    

    public void TakeDammage(int ammount)
    {
        if (currentState != States.Hit)
        {
            if (currentState != States.Blocking)
            {
                currentState = States.Hit;
                playerHelth -= ammount;
                playerAnim.SetTrigger("Hit");

                UpdateValues();
            }
            else
            {
                Debug.Log("Reached");
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
        transform.DOMove(bear.transform.GetChild(2).position, 0.5f);
        transform.DORotate(Vector3.zero, 0.5f);
        playerAnim.SetBool("Block", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bear"))
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

    //for new inputs
    public KeyCode[] validSequenceKeys = new[] 
    {
       KeyCode.A,KeyCode.B,KeyCode.C,KeyCode.D,KeyCode.E,KeyCode.F,KeyCode.G,KeyCode.H,KeyCode.I,KeyCode.J,KeyCode.K,KeyCode.L,
       KeyCode.M,KeyCode.N,KeyCode.O,KeyCode.P,KeyCode.Q,KeyCode.R,KeyCode.S,KeyCode.T,KeyCode.U,KeyCode.V,KeyCode.W,KeyCode.X,
       KeyCode.Y,KeyCode.Z
    };


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
        currentSequence = GenerateSequence(5);
        DisplayCurrentInput();
    }

    void DisplayCurrentInput()
    {
        takingIputs = true;
        timeleft = timeToReset;
        inputDisplay.text = currentSequence[currentIndex].ToString();
    }

    private void Update()
    {
        if(takingIputs)
        {
            if(timeleft>=0)
            {
                timeleft -= Time.deltaTime;
                if(Input.GetKeyDown(currentSequence[currentIndex]))
                {
                    currentIndex++;
                    DisplayCurrentInput();
                }
            }
        }
    }
}
