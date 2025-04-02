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
    private bool activated = true;

    public void Interact()
    {
        if (activated)
        {
            interactFunction.Invoke(gameObject);
        }
    }
    public void Activate()
    {
        activated = true;
    }
    public void Deactivate()
    {
        activated = false;
    }
    public bool isActive()
    {
        return activated;
    }
    public string getAction()
    {
        return actionName!=null? actionName : "";
    }
    public InteractionType getType()
    {
        return type;
    }
    public void SetAction(UnityEvent<GameObject> callback, String name, InteractionType type)
    {
        interactFunction = callback;
        actionName = name;
        this.type = type;
    }

    void  OnTriggerEnter(Collider other)
    {
        Debug.Log("puede interactuar");
        Interactor inter = other.GetComponent<Interactor>();
        if (inter != null)
        {
            inter.EnterInteractableRange(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("no puede interactuar");
        Interactor inter = other.GetComponent<Interactor>();
        if (inter != null)
        {
            inter.ExitUnteractableRange();
        }
    }
}
