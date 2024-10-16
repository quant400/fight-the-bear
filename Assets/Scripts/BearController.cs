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
    float sliderVal=1;
    float timer;
    float timeLeft;
    public bool canFollow=true;
 

    DamageDisplay bDD;

    BearSFXController bSFX;
    
   
    public void StartFight()
    {
        playerSeen = true;
        timeLeft = 15;
        bearAttack = 10 * (playerFC.GetBearNumber()+1);
        bearHealth = 100 + 25 * (playerFC.GetBearNumber());
        SetAttackRange(playerFC.GetBearNumber());
        playerFC.StartFight(gameObject);
        
        
    }

    private void SetAttackRange(int v)
    {
        switch (v)
        {
            case (0):
                attackRange = 2.9f;
                break;
            case (1):
                attackRange = 3.2f;
                break;
            case (2):
                attackRange = 3.7f;
                break;
            case (3):
                attackRange = 4.2f;
                break;
            default:
                attackRange = 4.2f;
                break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Start", Random.Range(0, 4));
        playerFC = MapView.instance.GetPlayer().GetComponent<FightController>();
        bDD = GetComponentInChildren<DamageDisplay>();
        bSFX = GetComponent<BearSFXController>();
    }

    private void Update()
    {
        if (playerSeen && playerFC.GetState()!=States.Dead && !playerFC.timeEnded && currentState!=States.Dead)
        {
            if (currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange && currentState == States.Idel && !playerFC.GetSpecialAttackStatus())
            {
                anim.SetBool("Follow", false);
                if (canAttackIn >= tempAttackTime)
                {
                    tempAttackTime = Random.Range(3, maxAttackInterval + 1);
                    canAttackIn = 0;
                    transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
                    if (Vector3.Distance(transform.position, playerFC.transform.position) <= attackRange) 
                        transform.Translate(Vector3.back*0.5f);
                    StartAttack(0);
                }



            }
            else if (canFollow && currentState != States.Dead && playerSeen && Vector3.Distance(playerFC.transform.position, transform.position) > attackRange && currentState == States.Idel && canAttackIn / tempAttackTime >= 0.5f)
            {
                Follow();
            }
            if (!playerFC.GetSpecialAttackStatus() && currentState!=States.Dead)
            {

                canAttackIn += Time.deltaTime;
                GameUIView.instance.SetAggression(canAttackIn / tempAttackTime);

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
        if(playerSeen && playerFC!=null && currentState!=States.Dead)
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
            bSFX.Playhit();
            if (stunned)
            {
                /*anim.SetBool("Stunned", false);
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
                    sliderVal = 0;*/

                anim.SetBool("Stunned", false);
                sliderVal = 2;
            }
            currentState = States.Hit;
            bearHealth -= (int)(dammage*sliderVal);
            playerFC.GivePoints((dammage * sliderVal));
            bDD.DisplayDamage((dammage * sliderVal));
            sliderVal = 1   ;
            if (bearHealth <= 0)
            {
                currentState = States.Dead;
                anim.SetTrigger("Die");
                //StopAllCoroutines();
            }
            else
            {
                knockBack();
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
        if(stunned)
        {
            anim.SetBool("Stunned", false);
            stunned = false;
            playerFC.DisableSpecialAttack();
        }
        currentState = States.Attacking;
        anim.SetFloat("Blend", Random.Range(0, 4));
  
        anim.SetTrigger("Attack");
        //StartCoroutine(Attack(delay));
    }

    public void StopTimer()
    {
        playerFC.StopTimer();
    }
   
    public void Stunned()
    {
        currentState = States.Idel;
        anim.SetBool("Stunned", true);
        stunned = true;
    }

    void knockBack()
    {
        transform.DOMove(transform.position + (transform.forward *-0.5f), 0.5f);
    }
    public void ResetAnim()
    {
        currentState = States.Idel;
    }

    public void Die()
    {
        currentState = States.Dead;
        GetComponent<BoxCollider>().enabled = false;
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
