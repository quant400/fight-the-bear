using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerSFXController : MonoBehaviour
{
    AudioSource playerAudio;
    CharacterController cC;
    ThirdPersonController tPC;

    [SerializeField]
    AudioClip punch;
    [SerializeField]
    AudioClip swoosh;
    [SerializeField]
    AudioClip hit;

    [SerializeField]
    AudioClip[] walk;

    private float m_StepCycle;
    private float m_NextStep;
    [SerializeField] private float m_StepInterval;
    void Start()
    {
        cC = GetComponent<CharacterController>();
        tPC = GetComponent<ThirdPersonController>();
        playerAudio = GetComponent<AudioSource>();
    }

    public void PlayPunch()
    {
        playerAudio.clip = punch;
        playerAudio.Play();
    }
    public void PlaySwoosh()
    {
        playerAudio.clip = swoosh;
        playerAudio.Play();
    }

    public void PlayStep()
    {
        if (!tPC.Grounded)
        {
            return;
        }
        else
        {
            /*if (!gameplayView.instance.GetSFXMuted())
            {*/
            int n = Random.Range(1, walk.Length);
            playerAudio.clip = walk[n];
            playerAudio.PlayOneShot(playerAudio.clip);

            walk[n] = walk[0];
            walk[0] = playerAudio.clip;
            // }
        }

    }

    public void ProgressStepCycle(float speed, float x, float y)
    {

        if (cC.velocity.sqrMagnitude > 0 && (x != 0 || y != 0))
        {
            m_StepCycle += (cC.velocity.magnitude + speed) * Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayStep();
    }
}
