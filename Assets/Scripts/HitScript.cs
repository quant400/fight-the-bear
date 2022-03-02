using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScript : MonoBehaviour
{
    int dammage;
    FightController myFC;
    BearController myBC;
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
            if (other.CompareTag("Bear") && myFC.GetState() == States.Attacking)
            {
               
                other.GetComponentInParent<BearController>().TakeDammage(dammage);
                myFC.UpdateValues();
            }
        } 
        else if (gameObject.CompareTag("Bear"))
        {
            if (other.CompareTag("Player") && myBC.GetState() == States.Attacking)
            {
               
                myFC.TakeDammage(dammage);
            }
        }
    }
}
