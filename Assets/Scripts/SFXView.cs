using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SFXView : MonoBehaviour
{
    public static SFXView instance;
    [SerializeField]
    AudioClip pathAmbient, caveAmbient;
    [SerializeField]
    AudioSource audioSource;
    public bool sfxMuted =false;
    public bool musicMuted =false;
    [SerializeField]
    Button muteSfx, muteMusic;
    Image sfxButtonImage, musicButtonImage;

    float defaultMusicVol;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
          
        }
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
       
    }
    private void Start()
    {
        ObserveGameObverBtns();

        defaultMusicVol = audioSource.volume;
        sfxButtonImage = muteSfx.GetComponent<Image>();
        musicButtonImage = muteMusic.GetComponent<Image>();
        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetString("SFX") == "off")
            {
                sfxMuted = true;
                MuteSFX();
            }

        }
        if (PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetString("Music") == "off")
            {
                musicMuted = true;
                MuteMusic();
            }
        }

       
    }

    public void ObserveGameObverBtns()
    {

        muteSfx.OnClickAsObservable()
            .Do(_ => MuteSFX())
            .Where(_ => PlaySounds.instance != null)
            .Do(_ => PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);

        muteMusic.OnClickAsObservable()
            .Do(_ => MuteMusic())
            .Where(_ => PlaySounds.instance != null)
            .Do(_ => PlaySounds.instance.Play())
            .Subscribe()
            .AddTo(this);


    }



    public void SetSFX(string location)
    {
        switch (location)
        {
            case "Menu":
                audioSource.Pause();
                break;

            case "Path":
                audioSource.clip = pathAmbient;
                audioSource.Play();
                break;

            case "Bear":
                audioSource.clip = caveAmbient;
                audioSource.Play();
                break;
        }
    }

    public void MuteSFX()
    {
        if (PlayerPrefs.HasKey("SFX") && PlayerPrefs.GetString("SFX") == "off")
        {
            //SFX.volume = defaultMusicVol;
            sfxButtonImage.color = new Color(1f, 1f, 1f, 1f);
            sfxButtonImage.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f);
            PlayerPrefs.SetString("SFX", "on");
            SFXView.instance.sfxMuted = false;
        }
        else
        {
            sfxButtonImage.color = new Color(1f, 1f, 1f, 0.5f);
            sfxButtonImage.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            //SFX.volume = 0;
            PlayerPrefs.SetString("SFX", "off");
            SFXView.instance.sfxMuted = true;
        }

    }

    public void MuteMusic()
    {
        if (audioSource.volume == 0)
        {
            musicButtonImage.color = new Color(1f, 1f, 1f, 1f);
            musicButtonImage.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f);
            audioSource.volume = defaultMusicVol;
            PlayerPrefs.SetString("Music", "on");
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            musicButtonImage.color = new Color(1, 1, 1, 0.5f);
            musicButtonImage.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            audioSource.volume = 0;
            PlayerPrefs.SetString("Music", "off");
        }
    }

}
