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
        SceneManager.sceneLoaded -= ResetPos;
    }
    // here temporarily for testing 
    public void ResetPos(Scene scene, LoadSceneMode mode)
    {
        //change to 1 later
       
        if(scene.buildIndex==2)
        {
            
            transform.GetComponent<CharacterController>().enabled = false;
            transform.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
            transform.position = new Vector3(0, 1, -30);
           
        }
            
    }

   
}
