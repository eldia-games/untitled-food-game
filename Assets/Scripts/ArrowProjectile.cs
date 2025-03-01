using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Destroy after 5 seconds

    void Start()
    {
        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition + Vector3.up * 2f - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        // Move forward in the current direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it hits the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Spell hit the player!");
            
            // Optionally, deal damage to player
            // other.GetComponent<PlayerHealth>()?.TakeDamage(10);

            Destroy(gameObject); // Destroy the spell
        }
        else if (!other.CompareTag("Enemy")) // Prevent self-collision
        {
            Debug.Log("Arrow hit something else: " + other.gameObject.name);
            Destroy(gameObject); // Destroy on any other collision
        }
    }
}
