using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayerOff : MonoBehaviour
{
    BearController bc;
    private void Start()
    {
        bc = GetComponentInParent<BearController>();
    }
    private void OnTriggerEnter(Collider other)
    {
       
       
        if (other.CompareTag("Kick"))
        {
            bc.canFollow = false;
            other.GetComponentInParent<FightController>().PushBack(5);
            Invoke("FollowAgain", 1f);
        }
    }

    void FollowAgain()
    {
        bc.canFollow = true;
    }
}
