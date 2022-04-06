using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController MC;
    [SerializeField]
    int maxPlanesBeforeBear;
    int planesToSpawnBeforeBear;
    int current=1;
    [SerializeField]
    GameObject[] PlanePrefabs;
    GameObject previoustrrain;
    [SerializeField]
    GameObject playerPrefab;
    Transform playerLoc;
    private void Awake()
    {
        if (MC != null)
        {
            Destroy(this.gameObject);
            return;
        }
           
        else
            MC = this;

        //DontDestroyOnLoad(this);
       
    }
    private void Start()
    {

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            playerLoc = Instantiate(playerPrefab, new Vector3(0, 0, -45), Quaternion.identity).transform;
        }
        else
            playerLoc = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnStartingt();
    }
    void SpawnStartingt()
    {
        planesToSpawnBeforeBear = Random.Range(2, maxPlanesBeforeBear+1);
        int t = UnityEngine.Random.Range(0, PlanePrefabs.Length);
        previoustrrain = Instantiate(PlanePrefabs[t], playerLoc.position+Vector3.forward*45, Quaternion.identity);
        previoustrrain.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void SpawnNext()
    {
        int t = UnityEngine.Random.Range(0, PlanePrefabs.Length);
        if (current == 0)
        {
            previoustrrain = Instantiate(PlanePrefabs[t], Vector3.zero, Quaternion.identity);
            previoustrrain.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }

        else if (current == planesToSpawnBeforeBear)
        {
            previoustrrain.transform.GetChild(1).gameObject.SetActive(true);
            previoustrrain.transform.GetChild(0).gameObject.SetActive(true);
        }

        else
        {
            previoustrrain = Instantiate(PlanePrefabs[t], previoustrrain.transform.position + new Vector3(0, 0, previoustrrain.transform.localScale.z * 10), Quaternion.identity);
            current++;
        }
    }

    public bool IsLast()
    {
        return (current == planesToSpawnBeforeBear);
    }
    public void ResetMap()
    {
        current = 0;
        SpawnStartingt();
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(GetBearScene());
    }

    int GetBearScene()
    {
        //change value later
        return 2;
    }
    
}
