using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FireworksButtonUnity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    RectTransform tr;

    Vector2 dragstart;
    Vector2 dragorigin;
    bool dragging = false;

    void Start()
    {
        tr = GetComponent<RectTransform>();
    }

    void Update()
    {

    }

    public void OnPointerUp(PointerEventData data)
    {
        if (dragging)
        {
            dragging = false;
        }
        else
        {
            //onclick
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        dragorigin = tr.position;
        dragstart = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        dragging = true;
        Vector2 pos = dragorigin + data.position - dragstart;
        tr.position = new Vector2(Mathf.Clamp(pos.x, 0, Screen.width - tr.rect.width), Mathf.Clamp(pos.y, 0, Screen.height - tr.rect.height));
    }
}
