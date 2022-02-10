using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerainSpawner : MonoBehaviour
{
    public static TerainSpawner instance;
    [SerializeField]
    GameObject[] terrains;
  
    GameObject previoustrrain;

    public int toSpawnBeforeBear;
    int current;
   
    private void Start()
    {
        //SpawnStartingt();
        
    }

    void  SpawnStartingt()
    {
        int t = UnityEngine.Random.Range(0, terrains.Length-1);
        previoustrrain = Instantiate(terrains[t], Vector3.zero, Quaternion.identity);
        current++;
    }

    void SpawnNext()
    {
        if(toSpawnBeforeBear<=current)
            SceneManager.LoadScene(1);
        else
        {
            Debug.Log("yes");
            int t = UnityEngine.Random.Range(0, terrains.Length - 1);
           Instantiate(terrains[t], transform.parent.position + new Vector3(0, 0, transform.parent.localScale.z*10), Quaternion.identity);
            current++;
            GetComponent<TerainSpawner>().enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SpawnNext();
        }
    }
}
