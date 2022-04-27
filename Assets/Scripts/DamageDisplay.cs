using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    Vector3 originalPos;
    TMP_Text text;
    bool active;
    Transform cam;
    GameObject image;
    private void Start()
    {
        originalPos = transform.localPosition;
        text = GetComponentInChildren<TMP_Text>();
        cam = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        image = transform.GetChild(0).gameObject;
    }


    public void DisplayDamage(float damage)
    {
        active = true;
        text.text = damage.ToString();
        image.SetActive(true);
        float upVal = 1;
        if (transform.parent.CompareTag("Player"))
            upVal = 0.25f;
        transform.DOLocalMoveY(transform.localPosition.y+upVal, 1f).OnComplete(()=>
        {
            text.text = "";
            transform.localPosition = originalPos;
            image.SetActive(false);
            active = false;
        });
    }
    private void LateUpdate()
    {
        if(active)
        {
            transform.LookAt(cam);
        }
    }

}
