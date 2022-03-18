using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField]
    float time;

    private void Start()
    {
        Invoke("Dest",time);
    }

    void Dest()
    {
        Destroy(gameObject);
    }
}
