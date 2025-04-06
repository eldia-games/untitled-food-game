using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpellProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float steeringStrength = 5f;
    public float lifetime = 5f; // Se destruye después de 5 segundos
    public float steeringTime = 1.5f; // Tiempo antes de detener el giro
    public float groundHeightOffset = 1f; // Altura deseada sobre el suelo
    public float descentSpeed = 5f;    // Velocidad a la que baja el proyectil

    private Transform target;
    private bool canSteer = true; // Controla el giro

    void Start()
    {
        Invoke("StopSteering", steeringTime);
        Destroy(gameObject, lifetime);
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        // Movimiento horizontal con dirección hacia el objetivo (sin afectar la altura)
        if (canSteer && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // Ignorar componente vertical
            direction.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, steeringStrength * Time.deltaTime);
        }
        transform.position += transform.forward * speed * Time.deltaTime;

        // Ajuste de altura: Raycast hacia abajo para detectar el suelo
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Calcula la altura objetivo (suelo + offset)
            float targetY = hit.point.y + groundHeightOffset;
            if (transform.position.y > targetY)
            {
                // Baja la posición Y gradualmente hasta el targetY
                float newY = Mathf.MoveTowards(transform.position.y, targetY, descentSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    void StopSteering()
    {
        canSteer = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCombat>()?.OnHurt(10, 0.2f, transform.position);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}
