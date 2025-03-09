using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikesBehaviour : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isTriggered", true);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isTriggered", false);
        }
    }
}
