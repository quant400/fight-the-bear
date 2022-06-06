using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera mainCam;
    [SerializeField] bool needFlipForward = true;
    [SerializeField] bool rotateWithHead = false;

    public float PositionLerpSpeed = 5f;
    public float RotationLerpSpeed = 5f;
    // Update is called once per frame
    void Update()
    {
        if (mainCam == null)
            mainCam = Camera.main;
        else
            LookAtCam();
    }
    void ShowPanel()
    {
        GameObject losepanel;
        losepanel = GameObject.Find("gameover").gameObject;
        if (losepanel != null)
            losepanel.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void LookAtCam()
    {
        if (needFlipForward)
        {
            Vector3 lookpoint = (transform.position * 2 - mainCam.transform.position);
            //lookpoint = new Vector3(transform.position.x, lookpoint.y, lookpoint.z);
            transform.LookAt(lookpoint);
            if (rotateWithHead)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, mainCam.transform.localEulerAngles.z);

            }
        }
        else
        {
            transform.LookAt(mainCam.transform);
            if (rotateWithHead)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, mainCam.transform.localEulerAngles.z);

            }
        }
    }
}
