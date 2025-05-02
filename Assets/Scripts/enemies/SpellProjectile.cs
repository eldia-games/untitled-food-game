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
    TrailRenderer trail;
    float trailDuration;

    void Start()
    {
        // Stop steering after "steeringTime" seconds
        Invoke("StopSteering", steeringTime);
        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trailDuration = trail.time;
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
            // Deal damage to player
            other.GetComponent<PlayerCombat>()?.OnHurt(10, 0.2f, transform.position);
            BeginDestroySequence();
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile")) // Prevent self-collision
        {
            BeginDestroySequence();
        }
    }


    public void BeginDestroySequence()
    {
        // Desactivar la emisión del TrailRenderer
        trail.emitting = false;
        // Desactivar renderers, colliders y lógica propia
        DisableVisualsAndLogic();
        // Iniciar la corrutina que finalmente destruye el objeto
        StartCoroutine(DelayedDestroy());
    }

    void DisableVisualsAndLogic()
    {
        // Desactivar cualquier Renderer que no sea el TrailRenderer
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            if (rend is TrailRenderer) continue;
            rend.enabled = false;
        }

        // Desactivar colisiones
        foreach (var col in GetComponents<Collider>())
            col.enabled = false;
        foreach (var col2d in GetComponents<Collider2D>())
            col2d.enabled = false;

        // Desactivar físicas
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.isKinematic = true;

        // Desactivar otros scripts (opcional)
        foreach (var mb in GetComponents<MonoBehaviour>())
        {
            if (mb == this) continue;
            mb.enabled = false;
        }
    }

    IEnumerator DelayedDestroy()
    {
        // Espera a que el trail se desvanezca completamente
        yield return new WaitForSeconds(trailDuration);
        Destroy(gameObject);
    }

}
