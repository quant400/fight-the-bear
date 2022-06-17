using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScript : MonoBehaviour
{
    float dammage;
    FightController myFC;
    BearController myBC;
    [SerializeField]
    GameObject playerHit;
    [SerializeField]
    GameObject bearHit;
    private void Start()
    {
        if (gameObject.CompareTag("Punch") || gameObject.CompareTag("Kick"))
        {
            myFC = GetComponentInParent<FightController>();
            dammage = myFC.GetDammage();
        }
        else if(gameObject.CompareTag("Bear"))
        {
            myFC = MapView.instance.GetPlayer().GetComponent<FightController>();
            myBC= GetComponentInParent<BearController>();
          
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log(collision.gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Kick"))
        {
            if (other.CompareTag("BearHitBox") && myFC.GetState() == States.Attacking && myFC.GetAttackingState()==AttackingStates.kicking)
            {
                var bearC = other.GetComponentInParent<BearController>();
                if (bearC.GetState() == States.Idel)
                {
                    PlayHitPlayer();
                    int additionalDamage = 0;
                    if (myFC.FightStyle == "mma")
                        additionalDamage = 10;
                    other.GetComponentInParent<BearController>().TakeDammage(dammage + additionalDamage);
                    myFC.UpdateValues();
                }
            }
        } 
        else if (gameObject.CompareTag("Punch"))
        {
            if (other.CompareTag("BearHitBox") && myFC.GetState() == States.Attacking && myFC.GetAttackingState() == AttackingStates.punching)
            {
              
                var bearC = other.GetComponentInParent<BearController>();
                if (bearC.GetState() == States.Idel)
                {
                    PlayHitPlayer();
                    int additionalDamage = 0;
                    if (myFC.FightStyle == "boxing")
                        additionalDamage = 10;
                    other.GetComponentInParent<BearController>().TakeDammage(dammage+additionalDamage);
                    myFC.UpdateValues();
                }
            }
        }
        else if (gameObject.CompareTag("Bear"))
        {
            if (other.CompareTag("Player") && myBC.GetState() == States.Attacking)
            {
                myBC.SetState(States.Idel);
                PlayHitBear();
                myFC.TakeDammage(myBC.GetBearDammage()); ;
            }
        }
    }

    public void PlayHitPlayer()
    {
        if(myFC.canHit)
        {
            myFC.canHit = false;
            
            
                Instantiate(playerHit, transform.position, Quaternion.identity);
            

            myFC.pSFXC.PlayPunch();
        }



    }
    public void PlayHitBear()
    {
        if(myFC.GetState()!=States.Hit)
            Instantiate(bearHit, transform.position, Quaternion.identity);

    }
}
