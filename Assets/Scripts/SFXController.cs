using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXController : MonoBehaviour
{
    public static SFXController instance;
    [SerializeField]
    AudioClip pathAmbient, caveAmbient;
    AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SetSFX;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetSFX;
    }

    void SetSFX(Scene scene, LoadSceneMode mode)
    {
        switch(scene.name)
        {
            case "Menu":
                Debug.Log("0");
                audioSource.Pause();
                break;

            case "PathScene":
                Debug.Log("1");
                audioSource.clip = pathAmbient;
                audioSource.Play();
                break;

            case "BearScene":
                Debug.Log("2");
                audioSource.clip = caveAmbient;
                audioSource.Play();
                break;
        }
    }
}
