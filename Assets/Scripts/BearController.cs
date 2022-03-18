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
    [SerializeField]
    float bearSpeed;
    [SerializeField]
    float attackRange;
    [SerializeField]
    float maxAttackInterval;
    float canAttackIn;
    States currentState;
    public bool actionDone;
    bool playerSeen=false;
    private bool stunned = false;
    float sliderVal=1;
    public void StartFight()
    {
        playerSeen = true;
    }
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
    }

    private void Update()
    { if (playerFC.GetBearNumber() == 3)
        {
            if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange && currentState==States.Idel && !playerFC.GetSpecialAttackStatus() )
            {
                anim.SetBool("Follow", false);
                if (canAttackIn <= 0)
                {
                    canAttackIn = Random.Range(3, maxAttackInterval + 1);
                    StartAttack(0);
                }
                else
                    canAttackIn -= Time.deltaTime;


            }
            else if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) > attackRange && currentState == States.Idel)
            {
                Follow();
            }
        }
        
    }

    private void Follow()
    {
        anim.SetBool("Follow", true);
        transform.LookAt(playerFC.transform);
        transform.Translate(Vector3.forward * bearSpeed * Time.deltaTime);
    }

    public void TakeDammage(int dammage)
    {
        if (currentState == States.Idel)
        {
            if (stunned)
            {
                anim.SetBool("Stunned", false);
                stunned = false;
                sliderVal = Mathf.Abs(playerFC.GetCurrentSliderVal() - 0.5f)+1;
            }
            currentState = States.Hit;
            bearHealth -= (int)(dammage*sliderVal);
            sliderVal = 1;
            if (bearHealth <= 0)
            {
                currentState = States.Dead;
                anim.SetTrigger("Die");
                //StopAllCoroutines();
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

    public void Stunned()
    {
        currentState = States.Idel;
        anim.SetBool("Stunned", true);
        stunned = true;
    }
    public void ResetAnim()
    {
        currentState = States.Idel;
    }

    public void Die()
    {
        GameObject.FindGameObjectWithTag("Door").GetComponent<SlidingDoor>().OpenDoor();
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
    public void SetState(States s)
    {
        currentState =s;
    }

}
