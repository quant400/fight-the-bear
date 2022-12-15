using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardScript : MonoBehaviour
{
    [SerializeField]
    GameObject[] boards;
    Texture[] avalaibleAds;

    private void Start()
    {
        if (Random.Range(0, 101) > 50)
            gameObject.SetActive(false);
        else
        {
            avalaibleAds = gameplayView.instance.ads;
            foreach (GameObject b in boards)
            {
                Debug.Log(1);
                b.GetComponent<MeshRenderer>().material.mainTexture= avalaibleAds[Random.Range(0, avalaibleAds.Length)];
            }
        }
    }
}
