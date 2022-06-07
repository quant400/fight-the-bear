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
    public bool sfxMuted =false;
    public bool musicMuted =false;
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
        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetString("SFX") == "off")
                sfxMuted = true;
        }
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetString("Music") == "off")
                musicMuted = true;
        }
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
        switch (scene.name)
        {
            case "Menu":
                Debug.Log("0");
                audioSource.Pause();
                break;

            case "PathScene":
                Debug.Log("1");
                audioSource.clip = pathAmbient;
                if (!musicMuted)
                    audioSource.Play();
                break;

            case "BearScene":
                Debug.Log("2");
                audioSource.clip = caveAmbient;
                if (!musicMuted)
                    audioSource.Play();
                break;
        }
    }


   
}
