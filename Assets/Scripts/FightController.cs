using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FightController : MonoBehaviour
{
    Animator playerAnim;
    GameObject bear;
    [SerializeField]
    GameObject fightCanvas;
    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    public void StartFight()
    {
        fightCanvas.SetActive(true);
        var CC = GetComponent<CharacterController>();
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorInputForLook = false;
        GetComponent<StarterAssets.StarterAssetsInputs>().cursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CC.enabled = false;
        CC.transform.position = bear.transform.GetChild(1).position;
        playerAnim.SetBool("Fight", true);
        bear.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>().Priority = 11;  

    }

    public void  Punch()
    {
        Debug.Log("pressed");
        int p = Random.Range(1, 3);
        playerAnim.SetInteger("Punch", p);
        Invoke("ResetAnim", 0.5f);
    }

    public void Kick()
    {
        int p = Random.Range(1, 3);
        playerAnim.SetInteger("Kick", p);
        Invoke("ResetAnim", 0.5f);

    }

    public void Block()
    {
        playerAnim.SetBool("Block", true);
        Invoke("ResetAnim", 0.5f);
    }


    public void TakeDammage()
    {

    }

    private void ResetAnim()
    {
        playerAnim.SetInteger("Punch", 0);
        playerAnim.SetInteger("Kick", 0);
        playerAnim.SetBool("Block", false);
        playerAnim.SetBool("Hit", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected");
        if(other.CompareTag("Bear"))
        {
            bear = other.gameObject;
            StartFight();
        }
    }



}
