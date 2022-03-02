using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().buildIndex==0)
                MapController.MC.LoadLevel();
            else if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                other.GetComponent<FightController>().MoveToNext();
            }
        }
    }
}
