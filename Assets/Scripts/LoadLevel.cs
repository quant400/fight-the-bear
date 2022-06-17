using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Location
{
    path,
    cave
}
public class LoadLevel : MonoBehaviour
{
    [SerializeField]
    Location location;
    private void OnTriggerEnter(Collider other)
    {
        //changes values to match scenes later

        if (other.CompareTag("Player"))
        {
            switch(location)
            {
                case Location.path:
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnEnterCave;
                    break;

                case Location.cave:
                    bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnPathLoad;
                    break;
            }


           /* if (SceneManager.GetActiveScene().buildIndex == 1)
                bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnEnterCave;
                //MapView.instance.LoadLevel();
            else if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                other.GetComponent<FightController>().MoveToNext();
                //Destroy(other.gameObject);
                //SceneManager.LoadScene(0);
            }*/
        }
    }
}
