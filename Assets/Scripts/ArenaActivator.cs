using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaActivator : MonoBehaviour
{
    private void Awake()
    {
        int arena = Random.Range(0, transform.childCount);
        transform.GetChild(arena).gameObject.SetActive(true);
    }
}
