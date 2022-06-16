using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearStageSfx : MonoBehaviour
{
    AudioSource sfx;
    [SerializeField]
    AudioClip roar, attack;
    [SerializeField]
    AudioClip[] hit;
    private void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    public void PlayRoar()
    {
        sfx.clip = roar;
        if (!SFXView.instance.sfxMuted)
            sfx.Play();
    }

    public void Playattack()
    {
        sfx.clip = attack;
        if (!SFXView.instance.sfxMuted)
            sfx.Play();
    }

    public void Playhit()
    {
        sfx.clip = hit[Random.Range(0,hit.Length)];
        if (!SFXView.instance.sfxMuted)
            sfx.Play();
    }
}
