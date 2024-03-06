using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnClickHandler = null;
    public Action OnDragHandler = null;

    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {

        if (OnClickHandler != null)
            OnClickHandler.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Manager.Sound.Play(Define.Sound.Effect, "ButtonClick");
        //Debug.Log("OnPointerDown");
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpHandler?.Invoke();
    }
}