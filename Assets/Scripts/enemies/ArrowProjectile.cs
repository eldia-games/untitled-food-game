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
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Ensure the arrow moves horizontally
        direction.Normalize(); // Normalize to ensure consistent speed
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        // Move forward in the current direction
        transform.position += speed * Time.deltaTime * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it hits the player
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Spell hit the player!");
            
            // Deal damage to player
            other.GetComponent<PlayerCombat>()?.OnHurt(10, 0.2f, transform.position);
            Destroy(gameObject); // Destroy the spell
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile")) // Prevent self-collision
        {
            //Debug.Log("Arrow hit something else: " + other.gameObject.name);
            Destroy(gameObject); // Destroy on any other collision
        }
    }
}
