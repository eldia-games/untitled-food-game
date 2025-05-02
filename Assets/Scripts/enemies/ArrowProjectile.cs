using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Destroy after 5 seconds
    public TrailRenderer trail;
    float trailDuration;
    void Start()
    {
        // Automatically destroy after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trailDuration = trail.time;
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
            other.GetComponent<PlayerCombat>()?.OnHurt(10, 0.2f, transform.position);
            BeginDestroySequence();
        }
        // Prevenir chocar consimismo
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
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
