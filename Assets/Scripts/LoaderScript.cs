using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScript : MonoBehaviour
{
   public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
