using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScript : MonoBehaviour
{
    public static LoaderScript instance;
    public int bearNumber;

    private void Awake()
    {
        if (LoaderScript.instance == null)
            LoaderScript.instance = this;
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
    }
    public void  LoadM1()
    {
        bearNumber = 0;
        SceneManager.LoadScene(1);
    }

    public void LoadM2()
    {
        bearNumber = 1;
        SceneManager.LoadScene(1);
    }

    public void LoadM3()
    {
        bearNumber = 2;
        SceneManager.LoadScene(1);
    }
}
