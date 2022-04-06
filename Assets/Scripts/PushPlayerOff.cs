using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayerOff : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Kick"))
        {
            other.GetComponentInParent<FightController>().PushBack(2);
        }
    }
}
