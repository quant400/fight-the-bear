using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if(SceneManager.GetActiveScene().buildIndex==1)
            GetComponent<FightController>().StartT3();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPos;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetPos;
    }
    // here temporarily for testing 
    public void ResetPos(Scene scene, LoadSceneMode mode)
    {
        //change to 1 later
        if(scene.buildIndex==1)
        {
            
            GetComponent<FightController>().StartT3();
        }
        if(scene.buildIndex==2)
        {
            GetComponent<FightController>().StoptT3();
            transform.GetComponent<CharacterController>().enabled = false;
            transform.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
            transform.position = new Vector3(0, 1, -30);
           
        }
            
    }

   
}
