using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;

public class bearView : MonoBehaviour
{
    public static bearView instance;
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
    public Transform bearHead;
    [SerializeField]
    float maxAttackInterval;
    float canAttackIn;
    float tempAttackTime;
    public FightModel.bearFightModes currentState;
    FightModel.bearFightModes desState;

    public bool actionDone;
    private bool stunned = false;
    float sliderVal = 0;
    float timer;
    float timeLeft;
    bool following;
    bool hitted;
    Vector3 destinationRage = Vector3.zero;
    public ReactiveProperty<bool> isRageFollow= new ReactiveProperty<bool>();
    public ReactiveProperty<float> bearRageDes = new ReactiveProperty<float>();
    public LayerMask headHitLayer;
    public LayerMask CaveHitLayer;

    shakeCamera cameraShake;
    ReactiveProperty<bool> isIdle = new ReactiveProperty<bool>();
    float jumpSpeedRage=2;
    public bool castRock;

    public void StartFight()
    {
        timeLeft = 15;
        bearAttack = 10 * (playerFC.GetBearNumber() + 1);
        bearHealth = 100 + 25 * (playerFC.GetBearNumber());
        playerFC.StartFight(gameObject);
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        cameraShake = GetComponent<shakeCamera>();
        instance = this;
        anim = GetComponent<Animator>();
        playerFC = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
        FightModel.currentFightMode = 1;
        desState = destinationMode(FightModel.currentFightMode);
        observeBearStatus();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(bearHead.position, transform.forward * 100);
    }
    void observeBearStatus()
    {
        FightModel.currentBearStatus
             .Subscribe(procedeBear)
               .AddTo(this);

        void procedeBear(FightModel.bearFightModes status)
        {
            switch (status)
            {
                case FightModel.bearFightModes.BearIdle:
                    resetRageMode();
                    desState = destinationMode(FightModel.currentFightMode);
                    cameraShake.setShake(0);
                    hitted = false;
                    following = false;
                    anim.SetBool("Following", false);
                    anim.Play("Idle", 0);
                    Observable.Timer(TimeSpan.Zero)
                                           .Do(_ => anim.SetBool("IsIdle", true))
                                           .Delay(TimeSpan.FromSeconds(1.5f))
                                           .Do(_ => anim.SetBool("IsIdle", false))
                                           .Do(_ => FightModel.currentBearStatus.Value = desState)
                                           .Subscribe()
                        .AddTo(this);
                    break;
                case FightModel.bearFightModes.BearShortFollowing:
                    Observable.Timer(TimeSpan.Zero)  
                        .Do(_ => following = true)
                        .Do(_ => anim.SetBool("Following", true))
                        .Do(_ => anim.SetTrigger("Follow"))
                        .Subscribe()
                        .AddTo(this);

                    break;
                case FightModel.bearFightModes.BearShortAttacking:
                    following = false;
                    anim.SetBool("Following", false);
                    transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
                    if (Vector3.Distance(transform.position, playerFC.transform.position) <= 2f)
                    {
                       transform.Translate(Vector3.back,transform );
                    }
                    if (!hitted)
                    {
                        Observable.Timer(TimeSpan.Zero)
                                     .Do(_=> anim.SetFloat("BlendAttack", UnityEngine.Random.Range(0, 4)))
                                     .Do(_=> anim.SetTrigger("Attack"))
                                    .Delay(TimeSpan.FromSeconds(0.5f))
                                    .Do(_=> checkIfPlayerCloseToHit(attackRange-1, bearHead))
                                    .Do(_ => desState = destinationMode(FightModel.currentFightMode))
                                    .Delay(TimeSpan.FromSeconds(0.5f))
                                    .Do(_=>FightModel.currentBearStatus.Value= FightModel.bearFightModes.BearIdle)
                                    .Subscribe()
                                    .AddTo(this);
                    }
                    else
                    {
                        Observable.Timer(TimeSpan.Zero)
                                  .Do(_ => desState = destinationMode(FightModel.currentFightMode))
                                  .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
                                  .Subscribe()
                                  .AddTo(this);
                    }
                    
                    break;
                case FightModel.bearFightModes.BearDistanceAttacking:
                    following = false;
                    anim.SetBool("Following", false);
                    desState = destinationMode(FightModel.currentFightMode);
                    anim.SetTrigger("IsDistanceAttacking");
                    anim.Play("DistanceAttackBear", 0);
                    break;
                case FightModel.bearFightModes.BearKnokedShortly:
                    cameraShake.setShake(0);
                    anim.SetBool("IsStunned",true);
                    break;
                case FightModel.bearFightModes.BearTakeDamage:
                      
                        following = false;
                        hitted = true;
                        Observable.Timer(TimeSpan.Zero)
                           .Do(_=> anim.SetFloat("hitBlend",Mathf.RoundToInt(UnityEngine.Random.Range(0,3))))
                           .Do(_ => anim.SetTrigger("Hit"))
                           .Where(_=> FightModel.currentBearHealth.Value>0)
                           .Do(_=>FightModel.currentBearHealth.Value-= damageFromMode(FightModel.currentFightMode))
                           .Subscribe()
                           .AddTo(this);
                

                    break;
                case FightModel.bearFightModes.BearRageMode:
                    resetRageMode();
                    FightModel.currentFightMode = 3;
                    anim.SetInteger("RageMode", 1);
                    cameraShake.setShake(0);

                    Observable.Timer(TimeSpan.Zero)
                        .DelayFrame(1)
                        .Do(_ => anim.SetInteger("RageMode", 0))

                        .Delay(TimeSpan.FromSeconds(1f))
                        .Do(_ => cameraShake.setShake(2))
                        .Do(_=> checkPlayerDistance())
                        .Delay(TimeSpan.FromSeconds(3f))
                        .Do(_ => cameraShake.setShake(0))
                        .Where(_ => FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearRageMode)
                        .Do(_ => destinationRage = playerFC.transform.position)
                        .Do(_ => isRageFollow.Value=true)
                        .Do(_ => anim.SetInteger("RageMode", 2))
                        //.Do(_ => StartCoroutine(SmoothLerp(Vector3.Distance(transform.position, destinationRage) / 8, destinationRage)))
                        .Subscribe()
                        .AddTo(this);
                    break;


            }
        }
        this.UpdateAsObservable()
            .Do(_=> rageFollow())
            .Do(_ => isIdle.Value = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            .Do(_=> currentState=FightModel.currentBearStatus.Value)
            .Where(_=> FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearShortFollowing)
            .Do(_ => observeBearCloseFromPlayer())
            .Where(_ => following)
            .Do(_ => Follow())
            .Subscribe()
            .AddTo(this);
       
        bearRageDes
                        .Where(_=> FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearRageMode)
                        .Where(_ => _ < 10)
                        .Where(_=> isRageFollow.Value)
                        .Do(_ => anim.SetInteger("RageMode", 3))
                        .Do(_ => jumpSpeedRage = 3)
                        .Delay(TimeSpan.FromMilliseconds(1000))
                        .Do(_ => jumpSpeedRage = 2)
                        .Do(_ => performRageHit(Mathf.Abs(Vector3.Distance(transform.position, playerFC.transform.position))))
                        .Delay(TimeSpan.FromMilliseconds(500))
                        .Do(_ => isRageFollow.Value = false)
                        .Subscribe()
                        .AddTo(this);
        isIdle
                        .Where(_ => _ == true)
                        .DelayFrame(2)
                        .Where(_ => _ == true)
                        .Where(_=> FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearTakeDamage)
                        .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
                        .Subscribe()
                        .AddTo(this);

    }
    void rageFollow()
    {
        if (isRageFollow.Value)
        {
            if (Mathf.Abs(Vector3.Distance(transform.position, destinationRage)) > 1)
            {
                bearRageDes.Value = Mathf.Abs(Vector3.Distance(transform.position, destinationRage));
                transform.LookAt(new Vector3(destinationRage.x, transform.position.y, destinationRage.z));
                transform.Translate(Vector3.forward * bearSpeed * jumpSpeedRage * Time.deltaTime);
            }
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, headHitLayer))
            {
                if (hit.transform.tag == "Rock")
                {
                    if (Mathf.Abs(Vector3.Distance(hit.point, transform.position)) < 6)
                    {

                        castRock = true;
                    }
                    else
                    {

                        castRock = false;
                    }
                }
            }
        }
    }
    void resetRageMode()
    {
        isRageFollow.Value = false;
        bearRageDes.Value = 1000;
    }
    void checkPlayerDistance()
    {
        float playerPushBackPostion = 20;
        float disDiff = Mathf.Abs(Vector3.Distance(transform.position, playerFC.transform.position));
        Debug.Log(disDiff);
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, CaveHitLayer))
        {
            Debug.Log(Mathf.Abs(Vector3.Distance(hit.point, playerFC.transform.position)));
            if (Mathf.Abs(Vector3.Distance(hit.point, playerFC.transform.position)) < playerPushBackPostion-4)
            {
                if (disDiff < playerPushBackPostion)
                {
                    playerFC.GetComponent<CharacterController>().enabled = false;
                    playerFC.transform.Translate(new Vector3(0, 0, Vector3.Distance(hit.point, playerFC.transform.position) / 2), transform);
                    playerFC.GetComponent<CharacterController>().enabled = true;
                }
                
            }
            else
            {
                if (disDiff < playerPushBackPostion)
                {
                    playerFC.GetComponent<CharacterController>().enabled = false;
                    playerFC.transform.Translate(new Vector3(0, 0, playerPushBackPostion), transform);
                    playerFC.GetComponent<CharacterController>().enabled = true;
                }
            }
           

        }
        else
        {
            if (disDiff < playerPushBackPostion)
            {
                playerFC.GetComponent<CharacterController>().enabled = false;
                playerFC.transform.Translate(new Vector3(0, 0, playerPushBackPostion), transform);
                playerFC.GetComponent<CharacterController>().enabled = true;
            }
        }
        
    }
    public FightModel.bearFightModes destinationMode(int mode)
    {
        FightModel.bearFightModes m = FightModel.bearFightModes.BearIdle;
        if (mode == 1)
        {
            m= FightModel.bearFightModes.BearShortFollowing;
        }
        else if(mode == 2)
        {
            m =FightModel.bearFightModes.BearDistanceAttacking;
        }
        else if (mode == 3)
        {
            m = FightModel.bearFightModes.BearRageMode;
        }
        else if (mode == 4)
        {
            m = FightModel.bearFightModes.BearShockingPlayer;
        }
        return m;



    }
    float damageFromMode(int mode)
    {
        float m = 1;
        if (mode == 1)
        {
            m = FightModel.playerCloseHitValue;
        }
        else if (mode == 2)
        {
            m = FightModel.playerDistanceHitValue;
        }
        else if (mode == 3)
        {
            m = FightModel.playerDistanceHitValue;
        }
        else if (mode == 4)
        {
            m = FightModel.playerDistanceHitValue;
        }
        return m;
    }
    private IEnumerator SmoothLerp(float time , Vector3 dest)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = transform.position + (transform.forward * 5);
        float elapsedTime = 0;
     
                    while (elapsedTime < time)
        {
         
                transform.position = Vector3.Lerp(startingPos, dest, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            if (Mathf.Abs(Vector3.Distance(transform.position, dest)) < 9){
                
                dest = destinationRage;
            }
            else
            {
                dest = playerFC.transform.position;
                destinationRage = dest;
            }
            bearRageDes.Value = Mathf.Abs(Vector3.Distance(transform.position, dest));
            yield return null;
        }
    }
    void performRageHit(float distance)
    {
        FightModel.currentFightMode = 3;

        if (distance < 4)
        {
            checkIfPlayerCloseToHit(attackRange, bearHead);
            isRageFollow.Value = false;
            desState = destinationMode(FightModel.currentFightMode);
            FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
            bearRageDes.Value = 1000;
        }
        else
        {
            anim.SetInteger("RageMode", 0);

            if (castRock)
            {
                Debug.Log("hit rock");
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearKnokedShortly;
                StartCoroutine(backToState( 5, "IsStunned"));
                isRageFollow.Value = false;
                return;
            }
            else
            {
                Debug.Log("player Far");

                desState = destinationMode(FightModel.currentFightMode);
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                bearRageDes.Value = 1000;
                return;
            }

        }
    }
    IEnumerator backToState(float t,string anString)
    {
        FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
        yield return new WaitForSeconds(t);
        if (anString != null)
        {
            anim.SetBool(anString, false);
        }
        FightModel.fightStatusValue.Value = 1;
        bearRageDes.Value = 1000;

    }
    void observeBearCloseFromPlayer()
    {
        if (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDead
                 && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange )
        {
            FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearShortAttacking;
            anim.SetBool("Following", false);

        }
    }
            
            private void updatingState()
    {
        if ( playerFC.GetState() != States.Dead && !playerFC.timeEnded && currentState != FightModel.bearFightModes.BearDead)
        {
            if (currentState != FightModel.bearFightModes.BearDead 
                 && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange 
                && currentState == FightModel.bearFightModes.BearIdle && !playerFC.GetSpecialAttackStatus())
            {
                anim.SetBool("Follow", false);
                if (canAttackIn >= tempAttackTime)
                {
                    tempAttackTime =UnityEngine.Random.Range(3, maxAttackInterval + 1);
                    canAttackIn = 0;
                    transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
                    if (Vector3.Distance(transform.position, playerFC.transform.position) <= 2f)
                        transform.Translate(Vector3.back * 0.5f);
                    StartAttack(0);
                }



            }
            else if (currentState != FightModel.bearFightModes.BearDead  
                && Vector3.Distance(playerFC.transform.position, transform.position) > attackRange 
                && currentState == FightModel.bearFightModes.BearIdle && canAttackIn / tempAttackTime >= 0.5f)
            {
                Follow();
            }
            if (!playerFC.GetSpecialAttackStatus() && currentState != FightModel.bearFightModes.BearDead)
            {

                canAttackIn += Time.deltaTime;

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
        if(isRageFollow.Value == false)
        {
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
            if (playerFC != null && currentState != FightModel.bearFightModes.BearDead)
                transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
        }
        
    }
    private void Follow()
    {
        transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
        transform.Translate(Vector3.forward * bearSpeed * Time.deltaTime);
    }

    public void TakeDammage(float dammage)
    {
        if (currentState == FightModel.bearFightModes.BearIdle)
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
            currentState = FightModel.bearFightModes.BearTakeDamage;
            bearHealth -= (int)(dammage + sliderVal);
            sliderVal = 0;
            if (bearHealth <= 0)
            {
                currentState = FightModel.bearFightModes.BearDead;
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
    public void checkIfPlayerCloseToHit(float distance,Transform bearHead)
    {
        if (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDead
                && Vector3.Distance(playerFC.transform.position, bearHead.position) <= distance )
        {
            if (FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerBlockShortAttack)
            FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerTakeDamage;
        }
    }
    public void StartAttack(float delay)
    {
        StartCoroutine(Attack(delay));
    }

    IEnumerator Attack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (stunned)
        {
            anim.SetBool("Stunned", false);
            stunned = false;
        }
    }
      

    public void StopTimer()
    {
        playerFC.StopTimer();
    }

    public void Stunned()
    {
        currentState = FightModel.bearFightModes.BearIdle;
        anim.SetBool("Stunned", true);
        stunned = true;
    }

    void knockBack()
    {
        transform.DOMove(transform.position + (transform.forward * -0.5f), 0.5f);
    }
    public void ResetAnim()
    {
        currentState = FightModel.bearFightModes.BearIdle;
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
    public FightModel.bearFightModes GetState()
    {
        return currentState;
    }
    public void SetState(FightModel.bearFightModes s)
    {
        currentState = s;
    }

}
