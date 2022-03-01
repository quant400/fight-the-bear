using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    States currentState;

    public void StartFight()
    {
        //StartCoroutine(Attack(attackInterval));
    }
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
    }

    public void TakeDammage(int dammage)
    {
        if (currentState == States.Idel)
        {
            currentState = States.Hit;
            bearHealth -= dammage;
            if (bearHealth <= 0)
            {
                anim.SetTrigger("Die");
                Die();
                StopAllCoroutines();
            }
            else
            {
                anim.SetTrigger("Hit");
            }
        }
        
       
    }

    public void StartAttack(float delay)
    {
        StartCoroutine(Attack(delay));
    }
   
    IEnumerator Attack(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = States.Attacking;
        anim.SetFloat("Blend", Random.Range(0, 4));
        anim.SetTrigger("Attack");
        //StartCoroutine(Attack(delay));
    }

    public void ResetAnim()
    {
        currentState = States.Idel;
    }

    void Die()
    {
        currentState = States.Dead;
        playerFC.ExitFight();

    }

    public int GetBearHelth()
    {
        return bearHealth;
    }
    public int GetBearDammage()
    {
        return bearAttack;
    }
    public States GetState()
    {
        return currentState;
    }


}
