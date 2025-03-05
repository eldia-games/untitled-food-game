using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverMessageHelpCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject message;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        message.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        message.SetActive(false);
    }
}
