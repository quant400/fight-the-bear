using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SliderScript : MonoBehaviour
{ 
    [SerializeField]
    Slider slider;
    [SerializeField]
    GameObject[] displayKeys;
    public void StartSlider()
    {
        if (slider.value != 0)
            slider.value = 0;
        slider.DOValue(1f, 3f).OnComplete(() => slider.DOValue(0f, 3f).OnComplete(()=>StartSlider()));
    }
    
    public float StopSlider()
    {

        slider.DOKill();

        return slider.value;
    } 

    public void SetInput(string s)
    {
        switch (s)
        {
            case "Alpha1":
                displayKeys[0].SetActive(true);
                displayKeys[1].SetActive(false);
                displayKeys[2].SetActive(false);
                break;
            case "Alpha2":
                displayKeys[0].SetActive(false);
                displayKeys[1].SetActive(true);
                displayKeys[2].SetActive(false);
                break;
            case "Alpha3":
                displayKeys[0].SetActive(false);
                displayKeys[1].SetActive(false);
                displayKeys[2].SetActive(true);
                break;
            case null:
                displayKeys[0].SetActive(false);
                displayKeys[1].SetActive(false);
                displayKeys[2].SetActive(false);
                break;

        }

    }
}
