using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

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
    public bool blocking;
    [SerializeField]
    TMP_Text bearHealth, playerHelthDisplay;
    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    public void StartFight()
    {
        fightCanvas.SetActive(true);
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
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = true;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = true;
        GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CC.enabled = true;
        playerAnim.SetBool("Fight", false);
        Debug.Log("called");
        bear.transform.GetChild(1).GetComponent<CinemachineVirtualCamera>().Priority = 9;
        //UpdateValues();
    }

    public void  Punch()
    {
        Debug.Log("pressed");
        int p = Random.Range(1, 3);
        playerAnim.SetInteger("Punch", p);
        Invoke("ResetAnim", 0.5f);
        bearC.TakeDammage(playerDammage);
        UpdateValues();
    }

    public void Kick()
    {
        int p = Random.Range(1, 3);
        playerAnim.SetInteger("Kick", p);
        Invoke("ResetAnim", 0.5f);
        bearC.TakeDammage(playerDammage);
        UpdateValues();

    }

    public void Block()
    {
        blocking = true;
        playerAnim.SetBool("Block", true);
        Invoke("ResetAnim", 0.5f);
    }


    public void TakeDammage(int ammount)
    {
        playerHelth -= ammount;
        playerAnim.SetBool("Hit", true);
        Invoke("ResetAnim", 0.5f);
        UpdateValues();
    }

    public void UpdateValues()
    {
        playerHelthDisplay.text = playerHelth.ToString();
        bearHealth.text = bearC.GetBearHelth().ToString();
    }

    private void ResetAnim()
    {
        playerAnim.SetInteger("Punch", 0);
        playerAnim.SetInteger("Kick", 0);
        playerAnim.SetBool("Block", false);
        playerAnim.SetBool("Hit", false);
        blocking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bear"))
        {
            bear = other.gameObject;
            StartFight();
        }
        if (other.CompareTag("NextTrigger"))
        {
            other.enabled = false;
            MapController.MC.SpawnNext();
        }
    }



}
