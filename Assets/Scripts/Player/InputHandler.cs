using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputHandler : MonoBehaviour
{
    public Vector2 input { get; private set; }
    public bool attack { get; private set; }

    public bool heal { get; private set; }

    public bool interact { get; private set; }

    //public bool jump { get; private set; }
    //public Vector2 mouse { get; private set; }

    //public bool hurt { get; private set; }
    
    //public bool elevator { get; private set; }

    public bool slide { get; private set; }
    public void onMove(InputAction.CallbackContext context)
    {
        input=context.ReadValue<Vector2>();
        //print("me muevo");    
    }
    public void onAction(InputAction.CallbackContext context)
    {
        attack = context.performed;
    }

    public void onSlide(InputAction.CallbackContext context)
    {
        slide = context.performed;
    }

    public void onHeal(InputAction.CallbackContext context)
    {
        heal = context.performed;
    }

    public void onInteract(InputAction.CallbackContext context)
    {
        interact = context.performed;
    }
}