using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUPSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject [] powerups;
    [SerializeField]
    int numToSpawn;

    private void Start()
    {
        SpawnPowerUps();
    }
    void SpawnPowerUps()
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            int x = Random.Range(0, powerups.Length);
            Instantiate(powerups[x], transform.position+ new Vector3(Random.Range(-36, 36), 0, Random.Range(-36, 36)), Quaternion.identity);
        }
    }

}
