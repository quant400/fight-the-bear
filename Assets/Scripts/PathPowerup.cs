using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPowerup : MonoBehaviour
{
    [SerializeField]
    GameObject powerup;
    [SerializeField]
    int numOfPowerups;
    int prevNum=100 ;
    int totalGems;
    public void SpawnPowerups()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag ("Gem");
        totalGems = gems.Length;
        for(int i=0;i<numOfPowerups;i++)
        {
            GameObject loc =gems[GetNum()];

            Instantiate(powerup, loc.transform.position, Quaternion.identity);
            loc.SetActive(false);
        }
    }

    int GetNum()
    {
        int res = Random.Range(0, totalGems);
        if (res != prevNum)
        {
            prevNum = res;
            return res;
        }
        else
            return GetNum();
    }
}
