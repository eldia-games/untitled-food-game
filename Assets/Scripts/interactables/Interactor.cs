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
        Debug.Log("can interact");
    }
    public void ExitUnteractableRange()
    {
        interactable = null;
        Debug.Log("cant interact");
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
