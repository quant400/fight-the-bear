using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform Button;

    private Vector3 origPosition;

    private bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        Button = gameObject.GetComponent<RectTransform>();

        origPosition = Button.localPosition;

        firstTime = false;
    }

    private void OnEnable()
    {
        if(!firstTime)
        {
            Button.DOLocalMove(origPosition, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Button.GetComponent<Animator>().Play("HoverOn");

        Button.DOLocalMove(new Vector3(origPosition.x - 5, origPosition.y + 5, origPosition.z), 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Button.GetComponent<Animator>().Play("HoverOff");

        Button.DOLocalMove(origPosition, 0.5f);
    }
}
