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
    public Animator anim;
    [SerializeField]
    float attackInterval;
    GameObject playerFC;
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
    public ReactiveProperty<bool> playerHitted = new ReactiveProperty<bool>();
    public float timeToHitAgain=2;
    public GameObject cinematicCamera;
    public void StartFight()
    {
        timeLeft = 15;
      
        
    }
    private void Awake()
    {
        FightModel.currentBear = gameObject;

        FightModel.CinematicCamera = cinematicCamera;

    }
    // Start is called before the first frame update
    private void Start()
    {

        cameraShake = GetComponent<shakeCamera>();
        instance = this;
        anim = GetComponent<Animator>();
        playerFC = FightModel.currentPlayer ;
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
                    if ((FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                    {
                        resetRageMode();
                        desState = destinationMode(FightModel.currentFightMode);
                        cameraShake.setShake(0);
                        hitted = false;
                        following = false;
                        anim.SetBool("Following", false);
                        Observable.Timer(TimeSpan.Zero)
                                               .Do(_ => anim.SetBool("IsIdle", true))
                                               .Delay(TimeSpan.FromSeconds(1.5f))
                                               .Do(_ => anim.SetBool("IsIdle", false))
                                               .Where(_ => (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                                               .Do(_ => FightModel.currentBearStatus.Value = desState)
                                               .Subscribe()
                            .AddTo(this);
                    }

                    break;
                case FightModel.bearFightModes.BearShortFollowing:
                    cameraShake.setShake(0);
                    Observable.Timer(TimeSpan.Zero)
                        .Delay(TimeSpan.FromSeconds(0.5f))
                        .Where(_ => (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
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
                    if (!hitted)
                    {
                        Observable.Timer(TimeSpan.Zero)
                                    .Do(_=> anim.SetFloat("BlendAttack", UnityEngine.Random.Range(0, 4)))
                                    .Do(_=> anim.SetTrigger("Attack"))
                                    .Delay(TimeSpan.FromSeconds(0.5f))
                                    .Do(_=> checkIfPlayerCloseToHit(attackRange-1, bearHead))
                                    .Do(_ => desState = destinationMode(FightModel.currentFightMode))
                                    .Delay(TimeSpan.FromSeconds(1f))
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
                    cameraShake.setShake(0);
                    following = false;
                    anim.SetBool("Following", false);
                    anim.SetTrigger("IsDistanceAttacking");
               
                    break;
                case FightModel.bearFightModes.BearKnokedShortly:
                    cameraShake.setShake(0);
                    anim.SetBool("IsStunned",true);
                    break;
                case FightModel.bearFightModes.BearDead:
                    anim.SetBool("IsDead", true);
                    anim.SetBool("OnDeath", true);
                    anim.Play("Dead", 0);
                    Observable.Timer(TimeSpan.Zero)
                                        .Do(_ => cinematicView.instance.setCamera(true,3))
                                        .Delay(TimeSpan.FromSeconds(3f))
                                        .Do(_ => cinematicView.instance.setCamera(false, 0))
                                        .Delay(TimeSpan.FromSeconds(1f))
                                  .Subscribe()
                                  .AddTo(this);
              

                    break;
                case FightModel.bearFightModes.BearTakeDamage:
                    cameraShake.setShake(0);
                    if ((FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
                    {
                        if (FightModel.rageModeValue.Value < 1)
                        {
                            FightModel.rageModeValue.Value += damageFromMode(FightModel.currentFightMode) / 30;
                        }
                        else
                        {
                            FightModel.rageModeValue.Value = 1;
                        }
                        FightModel.gameScore.Value += 2;
                        Debug.Log("bear Hitted");
                        following = false;
                        hitted = true;
                        Observable.Timer(TimeSpan.Zero)
                           .Do(_ => anim.SetFloat("hitBlend", Mathf.RoundToInt(UnityEngine.Random.Range(0, 3))))
                           .Do(_ => anim.SetTrigger("Hit"))
                           .Where(_ => FightModel.currentBearHealth.Value > 0)
                           .Do(_ => FightModel.currentBearHealth.Value -= damageFromMode(FightModel.currentFightMode))
                           .Do(_=> FightModel.currentBearStatus.Value = desState)
                           .Subscribe()
                           .AddTo(this);
                    }
                  



                    break;
                case FightModel.bearFightModes.BearRageMode:
                    if (!FightModel.bearIsStunned)
                    {
                        resetRageMode();
                        FightModel.currentFightMode = 3;
                        anim.SetInteger("RageMode", 1);
                        cameraShake.setShake(0);

                        Observable.Timer(TimeSpan.Zero)
                            .DelayFrame(1)
                            .Do(_ => anim.SetInteger("RageMode", 0))
                            .Delay(TimeSpan.FromSeconds(3f))
                            .Do(_ => cameraShake.setShake(2))
                            .Do(_ => checkPlayerDistance())
                            .Delay(TimeSpan.FromSeconds(3f))
                            .Do(_ => cameraShake.setShake(0))
                            .Where(_ => FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearRageMode)
                            .Do(_ => destinationRage = playerFC.transform.position)
                            .Do(_ => isRageFollow.Value = true)
                            .Do(_ => anim.SetInteger("RageMode", 2))
                            //.Do(_ => StartCoroutine(SmoothLerp(Vector3.Distance(transform.position, destinationRage) / 8, destinationRage)))
                            .Subscribe()
                            .AddTo(this);
                    }
                    break;


            }
        }
        this.UpdateAsObservable()
            .Where(_ => (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
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
                        .Where(_ => FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDead)
                        .Where(_ => _ == true)
                        .DelayFrame(2)
                        .Where(_ => _ == true)
                        .Where(_=> FightModel.currentBearStatus.Value == FightModel.bearFightModes.BearTakeDamage)
                        .Do(_ => FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle)
                        .Subscribe()
                        .AddTo(this);
        playerHitted
            .Where(_=>_==true)
            .Delay(TimeSpan.FromSeconds(timeToHitAgain/(FightModel.currentPlayerLevel+1)))
            .Do(_=> playerHitted.Value=false)
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
        float playerPushBackPostion = 10;
        float disDiff = Mathf.Abs(Vector3.Distance(transform.position, playerFC.transform.position));
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, CaveHitLayer))
        {
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
        else if (mode == 5)
        {
            m = FightModel.bearFightModes.BearKnokedShortly;
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
                anim.Play("Idle", 0);
                FightModel.bearIsStunned = true;
                anim.SetInteger("RageMode", 0);
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearKnokedShortly;
                FightModel.currentFightStatus.Value = FightModel.fightStatus.OnCloseDistanceFight;
                castRock = false;
                FightModel.currentFightMode = 5;
                StartCoroutine(backToState(4, "IsStunned"));
                return;
            }
            else
            {

                desState = destinationMode(FightModel.currentFightMode);
                FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearIdle;
                bearRageDes.Value = 1000;
                return;
            }

        }
    }
    IEnumerator backToState(float t,string anString)
    {
        yield return new WaitForSeconds(t);
        if (anString != null)
        {
            FightModel.currentFightMode = 1;
            FightModel.bearIsStunned = false;
            anim.SetInteger("RageMode", 0);
            anim.Play("Idle", 0);
            anim.SetBool(anString, false);
            FightModel.rageModeValue.Value = 0;
            bearRageDes.Value = 1000; 
            isRageFollow.Value = false;
        }


    }
    void observeBearCloseFromPlayer()
    {
        if (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDead
                  && Vector3.Distance(playerFC.transform.position, transform.position) <= attackRange)
        {
            FightModel.currentBearStatus.Value = FightModel.bearFightModes.BearShortAttacking;
            anim.SetBool("Following", false);

        }
    }
            
    private void LateUpdate()
    {
        if(isRageFollow.Value == false)
        {
            if ((FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon) && (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightLost))
            {
                Vector3 eulerRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
                if (playerFC != null && currentState != FightModel.bearFightModes.BearDead)
                    transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
            }
         
        }
        
    }
    private void Follow()
    {
        transform.LookAt(new Vector3(playerFC.transform.position.x, transform.position.y, playerFC.transform.position.z));
        transform.Translate(Vector3.forward * bearSpeed * Time.deltaTime);
    }

    public void checkIfPlayerCloseToHit(float distance,Transform bearHead)
    {
        if(!playerHitted.Value)
        {
            if (FightModel.currentBearStatus.Value != FightModel.bearFightModes.BearDead
                           && Vector3.Distance(playerFC.transform.position, bearHead.position) <= distance)
            {
                if (FightModel.currentPlayerStatus.Value != FightModel.PlayerFightModes.playerBlockShortAttack)
                {
                    playerHitted.Value = true;
                    FightModel.currentPlayerStatus.Value = FightModel.PlayerFightModes.playerTakeDamage;

                }
            }
        }
       


  
    }
      

   

    public void Die()
    {
        GameObject.FindGameObjectWithTag("Door").GetComponent<SlidingDoor>().OpenDoor();
        //playerFC.ExitFight();
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
