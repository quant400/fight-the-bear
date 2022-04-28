using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    Sprite[] hitEffects;
    [SerializeField]
    Image img;
    Transform cam;
    private void Awake()
    {
        int effect = Random.Range(0, hitEffects.Length);
        img.sprite = hitEffects[effect];
        cam = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 0.5f);
        int dir=Random.Range(0, 2);
        if (dir == 0)
            transform.DOLocalMoveX(transform.position.x + 0.75f, 0.5f);
        else if (dir == 1)
            transform.DOLocalMoveX(transform.position.x - 0.75f, 0.5f);
    }

    private void LateUpdate()
    {
        transform.LookAt(cam);

    }
}
