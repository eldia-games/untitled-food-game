using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float steeringStrength = 5f;
    public float lifetime = 5f; // Destroy after 5 seconds
    public float steeringTime = 1.5f; // Time before steering stops
    private Transform target;
    private bool canSteer = true; // Flag to control steering

    void Start()
    {
        // Stop steering after "steeringTime" seconds
        Invoke("StopSteering", steeringTime);
        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
        // Start rotation towards the target immediately
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (canSteer && target != null)
        {
            // Calculate direction towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            direction.Normalize(); // Normalize to ensure consistent speed

            // Smoothly rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, steeringStrength * Time.deltaTime);
            // Ensure the spell moves horizontally
        }

        // Move forward in the current direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void StopSteering()
    {
        canSteer = false; // Stop adjusting direction
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it hits the player
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Spell hit the player!");
            
            // Optionally, deal damage to player
            // other.GetComponent<PlayerHealth>()?.TakeDamage(10);

            Destroy(gameObject); // Destroy the spell
        }
        else if (!other.CompareTag("Enemy")) // Prevent self-collision
        {
            Destroy(gameObject); // Destroy on any other collision
        }
    }
}
