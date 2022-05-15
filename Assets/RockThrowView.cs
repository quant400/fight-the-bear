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
    // Start is called before the first frame update
    void Start()
    {
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
                        }
                    }
                    else
                    {
                        rocks = new GameObject[] { };
                    }

                    
                }
            }
           
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
                if (closesestRock != null)
                {
                    if (!rockPicked)
                    {
                        if (closestRock(rocks) != null)
                        {
                            rockPicked = true;

                            playerAnimator.SetBool("PickRock", true);
                        }
                    }
                    else
                    {

                        playerAnimator.SetBool("PickRock", false);
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
        Transform selectedRock = closesestRock;
        Vector3 startPosition = closesestRock.position;
        float randomTime = Random.Range(3, 5);
        Rigidbody rb = selectedRock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        selectedRock.position = FakeRock.position;
        Vector3 direction = Bear.position - player.position;
        FakeRock.gameObject.SetActive(false);
        selectedRock.gameObject.SetActive(true);
        rb.AddForce(force * direction);
        closesestRock = null;
        yield return new WaitForSeconds(1);
        rockPicked = false;
        yield return new WaitForSeconds(randomTime);
        rb.isKinematic = true;
        selectedRock.position = startPosition;
    }
}
