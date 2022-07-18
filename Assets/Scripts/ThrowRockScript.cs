using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRockScript : MonoBehaviour
{
    public bool Thrown=false;
    [SerializeField]
    ParticleSystem rockSmash;
    ParticleSystem particle;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player") && !other.CompareTag("Floor") && Thrown)
        {
            particle = Instantiate(rockSmash, transform.position,Quaternion.identity);
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            Invoke("Dest", 1f);
        }
    }


    private void Dest()
    {
        Destroy(particle.gameObject);

    }
}
