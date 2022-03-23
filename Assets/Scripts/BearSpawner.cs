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
        int randomNum = Random.Range(0, 101);
        if (randomNum <= chanceForRare)
        {
            Instantiate(bears[3], transform);
        }
        else
        {
            Instantiate(bears[br], transform);
        }
    }
}
