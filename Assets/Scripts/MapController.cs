using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController MC;
    [SerializeField]
    int planesToSpawnBeforeBear;
    int current;
    [SerializeField]
    GameObject[] PlanePrefabs;
    GameObject previoustrrain;

    private void Awake()
    {
        if (MC != null)
        {
            Destroy(this);
            return;
        }
           
        else
            MC = this;

        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        SpawnStartingt();
    }
    void SpawnStartingt()
    {
        int t = UnityEngine.Random.Range(0, PlanePrefabs.Length - 1);
        previoustrrain = Instantiate(PlanePrefabs[t], Vector3.zero, Quaternion.identity);
    }

    public void SpawnNext()
    {
        if (planesToSpawnBeforeBear <= current)
            SceneManager.LoadScene(GetBearScene());
        else
        {
            int t = UnityEngine.Random.Range(0, PlanePrefabs.Length - 1);
            previoustrrain=Instantiate(PlanePrefabs[t], previoustrrain.transform.position + new Vector3(0, 0,previoustrrain.transform.localScale.z * 10), Quaternion.identity);
            current++;
        }

    }

    int GetBearScene()
    {
        return 1;
    }
    
}
