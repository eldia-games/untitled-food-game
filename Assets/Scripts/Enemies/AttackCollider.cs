using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderEvent : MonoBehaviour
{
    // Parámetros de daño que puedes ajustar según tus necesidades.
    public int damage = 10;
    public float knockback = 5f;

    // Este método se llama cuando otro collider entra en este trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto entrante tiene el tag "Player"
        if(other.CompareTag("Player"))
        {
            // Intenta obtener el script del jugador (asegúrate de que el nombre coincide con el de tu script)
            PlayerCombat playerScript = other.GetComponent<PlayerCombat>();
            if(playerScript != null)
            {
                playerScript.OnHurt(damage, knockback, transform.forward);
            }
        }
    }
}