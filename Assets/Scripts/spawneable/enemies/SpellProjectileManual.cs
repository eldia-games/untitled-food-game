using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpellProjectileManual : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Destroy after 5 seconds

    void Start()
    {
        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward in the current direction
        Vector3 direction = transform.forward;
        direction.y = 0; // Ensure the spell moves horizontally
        direction.Normalize(); // Normalize to ensure consistent speed
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        // Set the initial direction of the spell
        transform.rotation = Quaternion.LookRotation(direction);
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
            //Debug.Log("Spell hit another object:" + other.name);
            Destroy(gameObject); // Destroy on any other collision
        }
    }
}
