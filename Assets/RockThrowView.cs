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
        rocks = GameObject.FindGameObjectsWithTag("ThrowRocks");
    }
    private void OnEnable()
    {
        rocks = GameObject.FindGameObjectsWithTag("ThrowRocks");

    }
    public void findRocks()
    {
        Bear = FightModel.currentBear.transform;
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
                        }
                        else
                        {
                            pickRockCanvas.SetActive(false);
                        }
                    }
                    else
                    {
                        rocks = new GameObject[] { };
                        pickRockCanvas.SetActive(false);

                    }
                }
            }
           
        }
        if (Input.GetKeyDown(KeyCode.E))
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (closesestRock != null)
            {
                if (rockPicked)
                {
                    playerAnimator.SetBool("PickRock", false);
                   // ThrowRockText.gameObject.SetActive(false);
                    PickwRockText.gameObject.SetActive(true);
                    pickRockCanvas.SetActive(false);
                    FightModel.isHoldingRock = false;

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
        FightModel.isHoldingRock = false;
        Transform selectedRock = closesestRock;
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        selectedRock.position = FakeRock.position;
        Vector3 direction = Bear.position - player.position;
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

    }
    public IEnumerator throwAndSetBackDelay(float force,float wait)
    {
        FightModel.isHoldingRock = false;
        Transform selectedRock = closesestRock;
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        selectedRock.position = FakeRock.position;
        Vector3 direction = Bear.position - player.position;
        Vector3 aimDirection = (direction.normalized + player.forward.normalized).normalized;
        FakeRock.gameObject.SetActive(false);
        selectedRock.gameObject.SetActive(true);
        rb.AddForce(force* 20 * aimDirection);
        closesestRock = null;
        yield return new WaitForSeconds(wait);
        rockPicked = false;
        yield return new WaitForSeconds(1);
        rb.isKinematic = true;
        selectedRock.position = startPosition;
        playerAnimator.SetBool("PickRock", false);

    }


}
