using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Warrior : BaseEnemy
{
    [Header("Configuración de Escudo")]
    public bool canBlock = true;      // Si puede o no bloquear
    public float blockChance = 0.5f;  // Probabilidad de que intente bloquear (0..1)
    public float blockDuration = 1.5f;// Cuánto dura el bloqueo
    public float blockCooldown = 5f;  // Tiempo de espera entre bloqueos

    private bool isBlocking = false;
    private float currentBlockCooldown = 0f;

    [Header("Ataques Melee")]
    public int numberOfMeleeAttacks = 2; // Cuántos tipos de ataques melee tiene
    // (Ej. 0 = ataque horizontal, 1 = ataque vertical, etc.)

    protected override void Update()
    {
        // Llama primero a la lógica principal (detección, combate, etc.)
        base.Update();

        // Si no está bloqueando, vamos reduciendo el cooldown del próximo bloqueo.
        if (!isBlocking && currentBlockCooldown > 0f)
        {
            currentBlockCooldown -= Time.deltaTime;
            if (currentBlockCooldown < 0f) currentBlockCooldown = 0f;
        }
    }

    protected override void HandleCombat()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Si estamos en medio de la animación de ataque, no nos movemos
        if (isAttacking)
        {
            agent.isStopped = true;
            return;
        }

        // Comportamiento de movimiento estándar
        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);

        // Intento de bloqueo si el jugador está cerca y no estamos en cooldown
        // Puedes ajustarlo para que bloquee justo antes de recibir un golpe, 
        // o de forma aleatoria cuando el jugador se acerca.
        if (canBlock && !isBlocking && currentBlockCooldown <= 0f)
        {
            // Por ejemplo, 50% de probabilidad de bloquear cuando la distancia < 3
            if (distanceToPlayer < 3f && Random.value < blockChance)
            {
                StartBlock();
            }
        }

        // Si el jugador está a rango de ataque, paramos y atacamos
        if (distanceToPlayer < attackRange && !isBlocking)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);

            // Mirar al jugador
            RotateTowards(player.transform.position);

            // Comprobar cooldown de ataque
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                // Preparar ataque
                StartAttack();
            }
        }
        else
        {
            // Animación de correr si aún no estamos en rango
            animator.SetBool("isRunning", true);
        }
    }

    // ---------------------------------------------------------------------
    //                          BLOQUEO
    // ---------------------------------------------------------------------
    void StartBlock()
    {
        isBlocking = true;
        currentBlockCooldown = blockCooldown; // Reinicia el cooldown de bloqueo
        animator.SetTrigger("block");         // Dispara animación de bloqueo
        // Termina el bloqueo después de blockDuration segundos
        Invoke(nameof(StopBlock), blockDuration);
    }

    void StopBlock()
    {
        isBlocking = false;
    }

    /// <summary>
    /// Sobrescribimos OnHurt para anular o reducir el daño cuando está bloqueando.
    /// </summary>
    protected override void OnHurt(float dmg, float knockback, Vector3 direction)
    {
        if (isBlocking)
        {
            // Aquí decides cómo afecta el bloqueo:
            //   - dmg = 0: anulas el daño totalmente
            //   - dmg *= 0.5f: reduce el daño a la mitad, etc.
            dmg = 0;
            Debug.Log("¡Ataque bloqueado con el escudo!");
        }

        base.OnHurt(dmg, knockback, direction);
    }

    // ---------------------------------------------------------------------
    //                          ATAQUE
    // ---------------------------------------------------------------------
    /// <summary>
    /// Se llama cuando la animación de ataque alcanza el punto de impacto
    /// (ver tu Animator -> Animation Event -> AttackEvent).
    /// </summary>
    public override void AttackEvent()
    {
        // Elegimos un ataque melee de varios posibles
        // (como en Rogue o Mage eliges un tipo de proyectil, aquí eliges un combo).
        int randomAttack = Random.Range(0, numberOfMeleeAttacks);

        // Decidimos qué hacer en cada tipo de ataque
        switch (randomAttack)
        {
            case 0:
                // Ataque 1
                Debug.Log("Ataque melee tipo 0 (horizontal).");
                // Aquí podrías habilitar un collider de arma o un mesh que haga daño
                break;
            case 1:
                // Ataque 2
                Debug.Log("Ataque melee tipo 1 (golpe en arco).");
                break;
            // etc.
        }

        // Llamamos a StopAttack() un poco después, 
        // o usamos la animación para desactivar colisiones.
        // Por defecto, BaseEnemy llama StopAttack() cuando acaba la animación
        // o puedes hacerlo con un Invoke.
        Invoke(nameof(StopAttack), 0.3f);
    }
}