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
    TMP_Text input;
    
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
        input.text = s;
    }
}
