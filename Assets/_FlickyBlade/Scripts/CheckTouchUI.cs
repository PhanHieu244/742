using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckTouchUI :MonoBehaviour, IPointerDownHandler,IPointerUpHandler {

    public void OnPointerDown(PointerEventData data)
    {
        UIManager.isTouchShareUI = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        UIManager.isTouchShareUI = false;
    }

}
