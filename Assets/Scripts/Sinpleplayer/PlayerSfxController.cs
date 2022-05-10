using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class PlayerSfxController : MonoBehaviour
{
    [SerializeField]
    AudioClip[] grassWalk;
    [SerializeField]
    AudioClip Jump, land, waterWalk, waterEnter, waterExit ,bump, powerUp;

   
    AudioSource playerAudio;

    CharacterController cC;
    ThirdPersonController tPC;

    bool inWater=false;
  
    private float m_StepCycle;
    private float m_NextStep;
    [SerializeField] private float m_StepInterval;
    private void Start()
    {
        cC = GetComponent<CharacterController>();
        tPC = GetComponent<ThirdPersonController>();
        playerAudio = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        if (Jump != null && !gameplayView.instance.GetSFXMuted())
        {
            playerAudio.clip = Jump;
            playerAudio.Play();
        }
    }

    public void PlayLand()
    {
        playerAudio.clip = land;
        playerAudio.Play();
    }

    public void PlayBump()
    {
        playerAudio.clip = bump;
        playerAudio.Play();
    }

    public void PlayWaterEnter()
    {
        if (!gameplayView.instance.GetSFXMuted())
        {
            playerAudio.clip = waterEnter;
            playerAudio.Play();
        }
    }

    public void PlayeWaterExit()
    {

        if (!gameplayView.instance.GetSFXMuted())
        {
            playerAudio.clip = waterExit;
            playerAudio.Play();
        }
    }

   
    public void PlayStep()
    {
        if (!tPC.Grounded)
        {
            return;
        }
        if(inWater)
        {
            //playerAudio.clip = waterWalk;
            //playerAudio.PlayOneShot(playerAudio.clip);
        }
        else
        {

            if (!gameplayView.instance.GetSFXMuted())
            {
                int n = Random.Range(1, grassWalk.Length);
                playerAudio.clip = grassWalk[n];
                playerAudio.PlayOneShot(playerAudio.clip);

                grassWalk[n] = grassWalk[0];
                grassWalk[0] = playerAudio.clip;
            }
        }
       
    }

    public void ProgressStepCycle(float speed,float x, float y)
    {
       
        if (cC.velocity.sqrMagnitude > 0 && (x!=0||y!=0))
        {
            m_StepCycle += (cC.velocity.magnitude+speed) *Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayStep();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && !inWater)
        {
            PlayWaterEnter();
            inWater = true;
        }

        else if (other.CompareTag("Bump"))
        {
            PlayBump();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Water") && inWater)
        {
            PlayeWaterExit();
            inWater = false;
        }
    }
}
