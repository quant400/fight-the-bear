using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSFXController : MonoBehaviour
{
    AudioSource SFX;
    AudioSource music;

    [SerializeField]
    Image sfxButton, musicButton;
    float defaultMusicVol;
    float defaultSFXVol;
    private void Start()
    {
        SFX = transform.GetChild(0).GetComponent<AudioSource>();
        music = transform.GetChild(1).GetComponent<AudioSource>();
        defaultMusicVol = music.volume;
        defaultSFXVol = SFX.volume;
        //Debug.Log((PlayerPrefs.GetString("SFX"), PlayerPrefs.GetString("Music"), gameplayView.instance.GetSFXMuted()));
        if(PlayerPrefs.HasKey("Music"))
        {
            if (PlayerPrefs.GetString("Music") == "off")
            {
                MuteMusic();
            }
        }

        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetString("SFX") == "off")
            {
                MuteSFX();
            }
        }
    }


    public void MuteSFX()
    {
        if (SFX.volume==0)
        {
            SFX.volume = defaultMusicVol;
            sfxButton.color += new Color(0, 0, 0, 0.5f);
            sfxButton.transform.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 0.5f);
            PlayerPrefs.SetString("SFX", "on");
            gameplayView.instance.SetSFXMuted(false);
        }
        else
        {
            sfxButton.color -= new Color(0, 0, 0, 0.5f);
            sfxButton.transform.GetChild(0).GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
            SFX.volume = 0;
            PlayerPrefs.SetString("SFX", "off");
            gameplayView.instance.SetSFXMuted(true);
        }

    }

    public void MuteMusic()
    {
        if(music.volume==0)
        {
            musicButton.color += new Color(0, 0, 0, 0.5f);
            musicButton.transform.GetChild(0).GetComponent<Image>().color += new Color(0, 0, 0, 0.5f);
            music.volume = defaultMusicVol;
            PlayerPrefs.SetString("Music", "on");
        }
        else
        {
            musicButton.color -= new Color(0, 0, 0, 0.5f);
            musicButton.transform.GetChild(0).GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
            music.volume = 0;
            PlayerPrefs.SetString("Music", "off");
        }
    }
}
