using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //changes values to match scenes later

        if (other.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().buildIndex==1)
                MapController.MC.LoadLevel();
            else if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                //other.GetComponent<FightController>().MoveToNext();
                Destroy(other.gameObject);
                SceneManager.LoadScene(0);
            }
        }
    }
}
