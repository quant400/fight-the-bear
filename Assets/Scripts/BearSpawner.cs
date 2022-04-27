using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] bears;
    FightController fc;
    float chanceForRare;
    private void Awake()
    {
        fc = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
        int br = fc.GetBearNumber();
        switch (br)
        {
            case 0:
                Instantiate(bears[0], transform);
                break;
            case 1:
                Instantiate(bears[1], transform);
                break;
            case 2:
                Instantiate(bears[2], transform);
                break;
            case 3:
                Instantiate(bears[3], transform);
                break;
            case 4:
                Instantiate(bears[0], transform);
                break;
            case 5:
                Instantiate(bears[1], transform);
                break;
            case 6:
                Instantiate(bears[2], transform);
                break;
            case 7:
                Instantiate(bears[3], transform);
                break;
            default:
                int randomNum = Random.Range(0, 4);
                Instantiate(bears[randomNum], transform);
                break;

        }
            
        
    }
}
