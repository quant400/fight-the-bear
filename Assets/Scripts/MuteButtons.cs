using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButtons : MonoBehaviour
{
    AudioSource SFX;
    AudioSource music;

    [SerializeField]
    Image sfxButton, musicButton;
    float defaultMusicVol;
    float defaultSFXVol;
    private void Start()
    {
        //SFX = transform.GetChild(0).GetComponent<AudioSource>();
        music = SFXController.instance.GetComponent<AudioSource>();
        defaultMusicVol = music.volume;
        Debug.Log(defaultMusicVol);
        //defaultSFXVol = SFX.volume;
        if (PlayerPrefs.HasKey("Music"))
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
        if (PlayerPrefs.HasKey("SFX") && PlayerPrefs.GetString("SFX")=="off")
        {
            //SFX.volume = defaultMusicVol;
            sfxButton.color = new Color(0.0509804f, 0.7254902f, 0.7254902f, 1f);
            PlayerPrefs.SetString("SFX", "on");
            SFXController.instance.sfxMuted = false;
        }
        else
        {
            sfxButton.color = new Color(1, 1, 1, 1);
            //SFX.volume = 0;
            PlayerPrefs.SetString("SFX", "off");
            SFXController.instance.sfxMuted = true;
        }

    }

    public void MuteMusic()
    {
        if (music.volume == 0)
        {
            musicButton.color = new Color(0.0509804f, 0.7254902f, 0.7254902f, 1f);
            music.volume = defaultMusicVol;
            PlayerPrefs.SetString("Music", "on");
            if (!music.isPlaying)
                music.Play();
        }
        else
        {
            musicButton.color = new Color(1, 1, 1, 1);
            music.volume = 0;
            PlayerPrefs.SetString("Music", "off");
        }
    }
}
