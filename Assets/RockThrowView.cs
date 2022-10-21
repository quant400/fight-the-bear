using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowView : MonoBehaviour
{
    public Transform player;
    public Animator playerAnimator;
    public Transform playerHand;
    public Transform FakeRock;
    public Transform Bear;
    public GameObject[] rocks;
    public List<GameObject> rocksList = new List<GameObject>();

    public Transform closesestRock;
    public float pickUpDistance;
    Transform localRock;
    public bool rockPicked =false;
    public GameObject pickRockCanvas;
    public GameObject ThrowRockText;
    public GameObject PickwRockText;
    
    // Start is called before the first frame update
    void Start()
    {
        ThrowRockText.gameObject.SetActive(false);
        PickwRockText.gameObject.SetActive(true);
        Invoke("GrabRocksAtStart", 0.5f);
        rocks = GameObject.FindGameObjectsWithTag("ThrowRocks");
    }
    private void OnEnable()
    {
        Invoke("GrabRocksAtStart", 0.5f);

    }
    void GrabRocksAtStart()
    {
        rocks = GameObject.FindGameObjectsWithTag("ThrowRocks");
    }

    public void findRocks(bool checkBear)
    {
        if (checkBear)
        {
            Bear = FightModel.currentBear.transform;
        }
        rocks = GameObject.FindGameObjectsWithTag("ThrowRocks");

    }
    // Update is called once per frame
    void Update()
    {
        if (!rockPicked)
        {
            if (rocks != null)
            {
                if (rocks.Length >0)
                {
                    if (rocks[0] != null) 
                    {
                        if (isClose())
                        {
                            closesestRock = closestRock(rocks);
                            pickRockCanvas.transform.position = closesestRock.position+ new Vector3(0,0.8f,0);
                            pickRockCanvas.SetActive(true);
                            GameUIView.instance.RockPickButton.SetActive(true);
                        }
                        else
                        {
                            pickRockCanvas.SetActive(false);
                            GameUIView.instance.RockPickButton.SetActive(false);
                        }
                    }
                    else
                    {
                        rocks = new GameObject[] { };
                        pickRockCanvas.SetActive(false);
                        GameUIView.instance.RockPickButton.SetActive(false);
                    }
                }
            }
           
        }
        if (Input.GetButtonDown("Fire3"))
        {
            if (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon)
            {
                if (closesestRock != null)
                {
                    if (!rockPicked)
                    {
                        if (closestRock(rocks) != null)
                        {
                            rockPicked = true;

                            playerAnimator.SetBool("PickRock", true);
                            //  ThrowRockText.gameObject.SetActive(true);

                            PickwRockText.gameObject.SetActive(false);
                            FightModel.isHoldingRock = true;

                        }
                    }

                }
            }
                
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (closesestRock != null)
            {
                if (rockPicked)
                {
                    playerAnimator.SetBool("PickRock", false);
                   // ThrowRockText.gameObject.SetActive(false);
                    PickwRockText.gameObject.SetActive(true);
                    pickRockCanvas.SetActive(false);

                }
            }
        }

    }
    public void pickUp()
    {
        if (closestRock(rocks) != null)
        {
            FakeRock.gameObject.SetActive(true);
            closesestRock.gameObject.SetActive(false);
        }
    }
    public void throwRock(float force)
    {
        FakeRock.gameObject.SetActive(false);
        if (closesestRock != null)
        {
            StartCoroutine(throwAndSetBack(force));
        }
    }
    public void throwRockWon(float force,float time)
    {
        FakeRock.gameObject.SetActive(false);
        if (closesestRock != null)
        {
            StartCoroutine(throwAndSetBackDelay(force, time));
        }
    }
    public Transform closestRock(GameObject[] rocks)
    {
        float closestDisttance = pickUpDistance;
        
        foreach (GameObject v in rocks)
        {
            if (Mathf.Abs( Vector3.Distance(v.transform.position, player.position)) < closestDisttance)
            {
                localRock = v.transform;
                closestDisttance = Mathf.Abs(Vector3.Distance(v.transform.position, player.position));
            }
        }
        return localRock;
    } 
    public bool isClose()
    {
        bool state = false;
        float closestDisttance = pickUpDistance;
        foreach (GameObject v in rocks)
        {
            if (v != null)
            {
                if (Mathf.Abs(Vector3.Distance(v.transform.position, player.position)) < closestDisttance)
                {
                    localRock = v.transform;
                    closestDisttance = Mathf.Abs(Vector3.Distance(v.transform.position, player.position));
                }
            }
          
        }
        if (localRock != null)
        {
            if (Mathf.Abs(Vector3.Distance(localRock.transform.position, player.position)) > pickUpDistance)
            {
                localRock = null;
                state = false;
            }
            else
                state= true;
        }

        return state;
    }
    public IEnumerator throwAndSetBack(float force)
    {
        yield return new WaitForSeconds(0.05f);
        FightModel.isHoldingRock = false;
        Transform selectedRock = closesestRock;
        //added for rock smash 
        ThrowRockScript trs = selectedRock.GetComponent<ThrowRockScript>();
        trs.Thrown = true;
        //ended
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false; 
        //added for rock smash 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        selectedRock.position = FakeRock.position;
        Vector3 direction = player.forward;
        /*if (Bear != null)
        {
            direction = Bear.position - player.position;
        }*/
        Vector3 aimDirection =(direction.normalized+player.forward.normalized).normalized;
        FakeRock.gameObject.SetActive(false);
        selectedRock.gameObject.SetActive(true);
        rb.AddForce(force * 20 * aimDirection);
        closesestRock = null;
        yield return new WaitForSeconds(1);
        rockPicked = false;
        yield return new WaitForSeconds(randomTime);
        rb.isKinematic = true;
       selectedRock.position = startPosition;
        //added for rock smash
        selectedRock.GetComponent<MeshRenderer>().enabled = true;
        trs.Thrown = false;
        selectedRock.GetComponent<SphereCollider>().enabled = true;

    }
    public IEnumerator throwAndSetBackDelay(float force, float wait)
    {
        yield return new WaitForSeconds(0.05f);
        FightModel.isHoldingRock = false;
        Transform selectedRock = closesestRock;
        //added for rock smash 
        ThrowRockScript trs = selectedRock.GetComponent<ThrowRockScript>();
        trs.Thrown = true;
        //ended
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        //added for rock smash 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        selectedRock.position = FakeRock.position;
        Vector3 direction = player.forward;
        /*if (Bear != null)
        {
            direction = Bear.position - player.position;
        }*/
        Vector3 aimDirection = (direction.normalized + player.forward.normalized).normalized;
        FakeRock.gameObject.SetActive(false);
        selectedRock.gameObject.SetActive(true);
        rb.AddForce(force * 20 * aimDirection);
        closesestRock = null;
        yield return new WaitForSeconds(wait);
        rockPicked = false;
        yield return new WaitForSeconds(1);
        rb.isKinematic = true;
        selectedRock.position = startPosition;
        //added for rock smash 
        selectedRock.GetComponent<MeshRenderer>().enabled = true;
        trs.Thrown = false;
        selectedRock.GetComponent<SphereCollider>().enabled = true;
        playerAnimator.SetBool("PickRock", false);

    }
    public void throwAndSetBackDirect(float force)
    {
        //moved here to fix glitch when entering cave with rock
        rockPicked = false;
        PickwRockText.gameObject.SetActive(true);
        pickRockCanvas.SetActive(false);

        FightModel.isHoldingRock = false;
        Transform selectedRock = closesestRock;
        //added for rock smash 
        ThrowRockScript trs = selectedRock.GetComponent<ThrowRockScript>();
        trs.Thrown = true;
        //ended
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        //added for rock smash 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        selectedRock.position = FakeRock.position;
        Vector3 direction = player.forward;
        /*if (Bear != null)
        {
            direction = Bear.position - player.position;
        }*/
        Vector3 aimDirection = (direction.normalized + player.forward.normalized).normalized;
        FakeRock.gameObject.SetActive(false);
        selectedRock.gameObject.SetActive(true);
        rb.AddForce(force * 20 * aimDirection);
        closesestRock = null;
        //rockPicked = false; moved up to fix rock throw script
        rb.isKinematic = true;
        selectedRock.position = startPosition;
        //added for rock smash
        selectedRock.GetComponent<MeshRenderer>().enabled = true;
        trs.Thrown = false;
        selectedRock.GetComponent<SphereCollider>().enabled = true;
    }


    public void ThrowRock()
    {
        if (closesestRock != null)
        {
            if (rockPicked)
            {
                playerAnimator.SetBool("PickRock", false);
                // ThrowRockText.gameObject.SetActive(false);
                PickwRockText.gameObject.SetActive(true);
                pickRockCanvas.SetActive(false);

            }
        }
    }

    public void PickUpRock()
    {
        if (FightModel.currentFightStatus.Value != FightModel.fightStatus.OnFightWon)
        {
            if (closesestRock != null)
            { 
                if (!rockPicked)
                {
                    if (closestRock(rocks) != null)
                    {
                        rockPicked = true;

                        playerAnimator.SetBool("PickRock", true);
                        //  ThrowRockText.gameObject.SetActive(true);

                        PickwRockText.gameObject.SetActive(false);
                        FightModel.isHoldingRock = true;
                        
                    }
                }

            }
        }
    }
}
