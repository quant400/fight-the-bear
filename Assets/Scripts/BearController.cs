using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    float attackInterval;
    FightController playerFC;
    [SerializeField]
    int bearAttack;
    [SerializeField]
    int bearHealth;

    public void StartFight()
    {
        StartCoroutine(Attack());
    }
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
    }

    public void TakeDammage(int dammage)
    {
        bearHealth -= dammage;
        if (bearHealth <= 0)
        {
            anim.SetBool("Die", true);
            Die();
            StopAllCoroutines();
        }
        else
        {
            anim.SetBool("Hit", true);
            StartCoroutine(ResetAnims());
        }
        
       
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackInterval);
        anim.SetFloat("Blend", Random.Range(0, 6));
        anim.SetBool("Attack", true);
        if(!playerFC.blocking)
        {
            playerFC.TakeDammage(bearAttack);
        }
        StartCoroutine(ResetAnims());
        StartCoroutine(Attack());
    }


    void Die()
    {
        playerFC.ExitFight();

    }

    public int GetBearHelth()
    {
        return bearHealth;
    }
    IEnumerator ResetAnims()
    {
        yield return new WaitForSeconds(1);
        anim.SetBool("Attack", false);
        anim.SetBool("Hit", false);
        anim.SetBool("Die", false);
        //anim.SetFloat("Blend", 0);
    }
}
