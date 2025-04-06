using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpellProjectileManual : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f; // Se destruye después de 5 segundos
    public float groundHeightOffset = 1f; // Altura deseada sobre el suelo
    public float descentSpeed = 5f;    // Velocidad a la que baja el proyectil

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Movimiento horizontal
        Vector3 direction = transform.forward;
        direction.y = 0; // Solo movimiento horizontal
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;

        // Ajuste de altura: Raycast hacia abajo para detectar el suelo
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float targetY = hit.point.y + groundHeightOffset;
            if (transform.position.y > targetY)
            {
                float newY = Mathf.MoveTowards(transform.position.y, targetY, descentSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    public void SetDirection(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Ensure the arrow moves horizontally
        direction.Normalize(); // Normalize to ensure consistent speed
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
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
