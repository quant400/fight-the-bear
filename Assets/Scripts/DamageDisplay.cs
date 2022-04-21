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
    private void Start()
    {
        originalPos = transform.localPosition;
        text = GetComponent<TMP_Text>();
        cam = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }


    public void DisplayDamage(float damage)
    {
        active = true;
        text.text = damage.ToString();
        transform.DOLocalMoveY(transform.localPosition.y+1, 1f).OnComplete(()=>
        {
            text.text = "";
            transform.localPosition = originalPos;
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
