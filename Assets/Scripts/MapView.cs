using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapView : MonoBehaviour
{
    public static MapView instance;
    [SerializeField]
    int maxPlanesBeforeBear;
    int planesToSpawnBeforeBear;
    int current=1;
    [SerializeField]
    GameObject[] PlanePrefabs;
    GameObject previoustrrain;
    Transform playerLoc;
    Transform pathholder;
    CaveCutscene caveCutScene;
    [SerializeField]
    GameObject CavePrefab,currentCave;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
           
        else
            instance = this;

        //DontDestroyOnLoad(this);
       
    }
    public void SpawnPlayer()
    {
        if (playerLoc == null)
        {
            //chosenNFTName = NameToSlugConvert(gameplayView.instance.chosenNFT.name);
            string n = gameplayView.instance.chosenNFT.name;
            GameObject resource = Resources.Load(Path.Combine("SinglePlayerPrefabs/Characters", NameToSlugConvert(n))) as GameObject;
            //GameObject temp = Instantiate(resource, spawnPoint.position, Quaternion.identity);
            playerLoc = Instantiate(resource, new Vector3(0, 0, -45), Quaternion.identity).transform;
            //Instantiate(playerPrefab, new Vector3(0, 0, -45), Quaternion.identity).transform;
        }
        else
        {
            //playerLoc = GameObject.FindGameObjectWithTag("Player").transform;
            playerLoc.gameObject.SetActive(true);
        }
        bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnGameRunning;
        //SpawnStartingt();
    }
    public void SpawnStarting()
    {
        if(currentCave!=null)
        {
            Destroy(currentCave);
            currentCave = null;
            playerLoc.gameObject.SetActive(false);
            playerLoc.transform.position = new Vector3(0, 0, -45);
        }
        pathholder = new GameObject("PathHolder").transform;
        planesToSpawnBeforeBear = Random.Range(1, maxPlanesBeforeBear+1);
        int t = UnityEngine.Random.Range(0, PlanePrefabs.Length);
        previoustrrain = Instantiate(PlanePrefabs[t], Vector3.zero, Quaternion.identity);
        previoustrrain.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        previoustrrain.transform.parent = pathholder;
        SpawnNext();
        
    }

    public void SpawnNext()
    {
        int t = UnityEngine.Random.Range(0, PlanePrefabs.Length);
        if (current == 0)
        {
            previoustrrain = Instantiate(PlanePrefabs[t], Vector3.zero, Quaternion.identity);
            previoustrrain.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            previoustrrain.transform.parent = pathholder;
        }

        else if (current == planesToSpawnBeforeBear)
        {
            previoustrrain.transform.GetChild(1).gameObject.SetActive(true);
            previoustrrain.transform.GetChild(0).gameObject.SetActive(true);
            caveCutScene = previoustrrain.transform.GetChild(0).GetComponent<CaveCutscene>();
            caveCutScene.last = true;
            previoustrrain.transform.parent = pathholder;
            current = 1;
            bearGameModel.gameCurrentStep.Value = bearGameModel.GameSteps.OnPathLoaded;
           
        }

        else
        {
            previoustrrain = Instantiate(PlanePrefabs[t], previoustrrain.transform.position + new Vector3(0, 0, previoustrrain.transform.localScale.z * 10), Quaternion.identity);
            current++;
            previoustrrain.transform.parent = pathholder;
            SpawnNext();
        }
    }

    public void StartCutsene()
    {
        caveCutScene.StartScene();
    }

    public void GoIntoCave()
    {
        playerLoc.gameObject.SetActive(false);
        if (pathholder.gameObject != null)
            Destroy(pathholder.gameObject);
        currentCave = Instantiate(CavePrefab);
        playerLoc.position = new Vector3(0, 0, -20f);
        playerLoc.gameObject.SetActive(true);

    }


    public bool IsLast()
    {
        return false;
    }
    public void ResetMap()
    {
        current = 0;
        SpawnStarting();
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

    public GameObject GetPlayer()
    {
        return playerLoc.gameObject;
    }

    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;

    }
}
