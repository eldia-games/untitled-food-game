using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InteractionType
{
    None,
    NormalInteraction,
    FirePlaceInteraction
}
public class Interactor : MonoBehaviour
{
    private Interactable interactable;
    private int sum = 0;

    public void EnterInteractableRange(Interactable interact)
    {
        UIManager.Instance.ShowPopUpCanvas(interact.getAction());
        interactable = interact;
        sum++;
    }
    public void ExitUnteractableRange()
    {
        interactable = null;
        sum--;
        if (sum == 0)
        {
            UIManager.Instance.HidePopUpCanvas();
        }
    }
    public void interact()
    {
        if (interactable != null && interactable.isActive())
        {
            interactable.Interact();

            UIManager.Instance.HidePopUpCanvas();
            sum = 0;

        }
    }
    public InteractionType GetInteractionType()
    {
        if(interactable == null)
        {
            return InteractionType.None;
        }
        else
        {
            return interactable.getType();
        }
    }


}
