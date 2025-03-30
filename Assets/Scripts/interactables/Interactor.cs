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

    public void EnterInteractableRange(Interactable interact)
    {
        interactable = interact;
    }
    public void ExitUnteractableRange()
    {
        interactable = null;
    }
    public void interact()
    {
        if (interactable != null && interactable.isActive())
        {
            interactable.Interact();
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
