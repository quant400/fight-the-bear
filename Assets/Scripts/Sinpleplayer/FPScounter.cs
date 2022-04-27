using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPScounter : MonoBehaviour
{
    [SerializeField]
    TMP_Text counter,averageFps,minFps;
    float min = Mathf.Infinity;
    int framesPassed;
    float totalFps;
    void Update()
    {
        float fps = 1 / Time.unscaledDeltaTime;
        counter.text = "FPS:" + (int)fps;
        totalFps += fps;
        framesPassed++;
        averageFps.text = "Avg:" + (int)(totalFps / framesPassed);
        if (fps < min && framesPassed > 10)
        {
            min = fps;
            minFps.text = "Min: " + (int)min;
        }
    }

}
