using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetPos;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded += ResetPos;
    }
    // here temporarily for testing 
    public void ResetPos(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex==1)
        {
            transform.GetComponentInChildren<CharacterController>().enabled = false;
            transform.GetComponentInChildren<StarterAssets.ThirdPersonController>().enabled = false;
            transform.GetChild(2).position = new Vector3(0, 1, -35);
           
        }
            
    }

   
}
