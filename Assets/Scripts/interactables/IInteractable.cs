using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable: MonoBehaviour
{
    [SerializeField] private UnityEvent<GameObject> interactFunction;
    [SerializeField] private String actionName;
    [SerializeField] private InteractionType type; 
    private bool actived = true;

    public void Interact()
    {
        if (actived)
        {
            interactFunction.Invoke(gameObject);
        }
    }
    public void Active()
    {
        actived = true;
    }
    public void Desactive()
    {
        actived = false;
    }
    public bool isActive()
    {
        return actived;
    }
    public string getAction()
    {
        return actionName;
    }
    public InteractionType getType()
    {
        return type;
    }

    void  OnTriggerEnter(Collider other)
    {
        Interactor inter = other.GetComponent<Interactor>();
        if (inter != null)
        {
            inter.EnterInteractableRange(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Interactor inter = other.GetComponent<Interactor>();
        if (inter != null)
        {
            inter.ExitUnteractableRange();
        }
    }
}
