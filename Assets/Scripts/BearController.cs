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
    float tempAttackTime;
    States currentState;
    public bool actionDone;
    bool playerSeen=false;
    private bool stunned = false;
    float sliderVal=0;
    float timer;
    float timeLeft;
    bool following;
    public void StartFight()
    {   
        playerFC.StartFight(gameObject);
        playerSeen = true;
        timeLeft = 15;
        bearAttack = 10 * playerFC.GetBearNumber();
        bearHealth = 100 + 25 * (playerFC.GetBearNumber()-1);
        
    }
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
        
    }

    private void Update()
    {
        if (playerSeen)
        {
            if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange && currentState == States.Idel && !playerFC.GetSpecialAttackStatus())
            {
                anim.SetBool("Follow", false);
                if (canAttackIn >= tempAttackTime)
                {
                    tempAttackTime = Random.Range(3, maxAttackInterval + 1);
                    canAttackIn = 0;
                    transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
                    if (Vector3.Distance(transform.position, playerFC.transform.position) < 2) 
                        transform.Translate(Vector3.back);
                    StartAttack(0);
                }



            }
            else if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) > attackRange && currentState == States.Idel && canAttackIn / tempAttackTime >= 0.5f)
            {
                Follow();
            }
            if (!playerFC.GetSpecialAttackStatus() &&currentState!=States.Hit && currentState!=States.Dead)
            {

                canAttackIn += Time.deltaTime;
                playerFC.SetAggression(canAttackIn / tempAttackTime);

            }
            timer += Time.deltaTime;
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                bearAttack += 10;
                bearSpeed += 0.1f;
                timeLeft = 15;
            }
        }

    }
    private void LateUpdate()
    {
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
        if(playerSeen && playerFC!=null)
            transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
    }
    private void Follow()
    {
        anim.SetBool("Follow", true); 
        transform.LookAt(new Vector3(playerFC.transform.position.x,transform.position.y,playerFC.transform.position.z));
        transform.Translate(Vector3.forward * bearSpeed * Time.deltaTime);
    }

    public void TakeDammage(float dammage)
    {
        if (currentState == States.Idel)
        {
            if (stunned)
            {
                anim.SetBool("Stunned", false);
                stunned = false;
                sliderVal = Mathf.Abs(playerFC.GetCurrentSliderVal() - 0.5f);
                if (sliderVal >= 0.15 && sliderVal <= 0.35)
                {
                    sliderVal = 5;
                }
                else if (sliderVal <= 0.15)
                {
                    sliderVal = 10;
                }
                else
                    sliderVal = 0;
            }
            currentState = States.Hit;
            bearHealth -= (int)(dammage+sliderVal);
            playerFC.GivePoints((dammage + sliderVal));
            sliderVal = 0;
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
/*
 *  if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange && currentState == States.Idel && !playerFC.GetSpecialAttackStatus())
        {
            anim.SetBool("Follow", false);
            if (canAttackIn <= 0)
            {
                canAttackIn = Random.Range(3, maxAttackInterval + 1);
                tempAttackTime = canAttackIn;
                transform.LookAt(playerFC.transform);
                StartAttack(0);
            }
            else
            {
                canAttackIn -= Time.deltaTime;
            }


        }
        else if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) > attackRange && currentState == States.Idel)
        {
            Follow();
        }*/