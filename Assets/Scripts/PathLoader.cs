using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Caller");
        SceneManager.LoadScene(1);
    }
}
