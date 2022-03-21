using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScript : MonoBehaviour
{
    float dammage;
    FightController myFC;
    BearController myBC;
    [SerializeField]
    ParticleSystem enemyHit;
    [SerializeField]
    ParticleSystem specialkAttack;
    private void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            myFC = GetComponentInParent<FightController>();
            dammage = myFC.GetDammage();
        }
        else if(gameObject.CompareTag("Bear"))
        {
            myFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
            myBC= GetComponentInParent<BearController>();
            dammage = myBC.GetBearDammage();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (other.CompareTag("BearHitBox") && myFC.GetState() == States.Attacking)
            {
                var bearC = other.GetComponentInParent<BearController>();
                if (bearC.GetState() != States.Attacking)
                {
                    PlayHit();
                    other.GetComponentInParent<BearController>().TakeDammage(dammage);
                    myFC.UpdateValues();
                }
            }
        } 
        else if (gameObject.CompareTag("Bear"))
        {
            if (other.CompareTag("Player") && myBC.GetState() == States.Attacking)
            {
                myBC.SetState(States.Idel);
                myFC.TakeDammage(dammage);
            }
        }
    }

    public void PlayHit()
    {
        if(myFC.canHit)
        {
            myFC.canHit = false;
            if(myFC.GetSpecialAttackStatus())
            {
                Instantiate(specialkAttack, transform.position + new Vector3(0.5f,0f,0), Quaternion.Euler(-60,0,0));
            }
            else
            {
                Instantiate(enemyHit, transform.position, Quaternion.identity);
            }
        }
            
        
       
    }
}
