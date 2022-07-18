using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class BearSpawnView : MonoBehaviour
{
    [SerializeField]
    GameObject[] bears;
    public GameObject bearObserveObject;
    private void Awake()
    {
        FightModel.bearObserveObj = bearObserveObject;
        Debug.Log(FightModel.currentPlayerLevel);
        int br = FightModel.currentPlayerLevel;
        switch (br)
        {
            case 0:
                Instantiate(bears[0], transform).transform.localScale = Vector3.one * 1.25f;
                break;
            case 1:
                Instantiate(bears[1], transform).transform.localScale = Vector3.one * 1.5f; ;
                break;
            case 2:
                Instantiate(bears[2], transform).transform.localScale = Vector3.one * 1.75f; ;
                break;
            case 3:
                Instantiate(bears[3], transform).transform.localScale = Vector3.one * 2f; ;
                break;

            default:
                int randomNum = Random.Range(0, 4);
                GameObject b = Instantiate(bears[randomNum], transform);
                b.transform.localScale = Vector3.one * 2f;
                b.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }


    }
}
