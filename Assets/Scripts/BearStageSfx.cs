using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearStageSfx : MonoBehaviour
{
    AudioSource sfx;
    private void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    public void PlayRoar()
    {
        sfx.Play();
    }
}
