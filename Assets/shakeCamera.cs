using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class shakeCamera : MonoBehaviour
{
    // Start is called before the first frame update
    CinemachineVirtualCamera cineMcamera;
    void Start()
    {
        GameObject mainCam = GameObject.FindGameObjectWithTag("playerCam");
        cineMcamera = mainCam.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setShake(float value)
    {
        cineMcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = value;
    }
}
