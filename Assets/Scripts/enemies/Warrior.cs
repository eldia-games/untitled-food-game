using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class Warrior : BaseEnemy
{
    [Header("Configuración de Escudo")]
    public bool canBlock = true;      // Si puede o no bloquear
    public float blockChance = 0.5f;  // Probabilidad de que intente bloquear (0..1)
    public float blockDuration = 1.5f;// Cuánto dura el bloqueo
    public float blockCooldown = 5f;  // Tiempo de espera entre bloqueos

    public Collider shieldCollider; // El collider del escudo

    public bool isBlocking = false;
    private float currentBlockCooldown = 0f;

    [Header("Ataques Melee")]
    public Collider attackCollider; // El collider del ataque melee
    public int numberOfMeleeAttacks = 2; // Cuántos tipos de ataques melee tiene
    // (Ej. 0 = ataque horizontal, 1 = ataque vertical, etc.)

    public bool lookAtPlayerWhileAttacking = false;

    private Collider currentAttackCollider; // El collider del ataque actual

    protected override void Start()
    {
        base.Start();
        attackCollider.enabled = false;
    }

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
            if(lookAtPlayerWhileAttacking)
                RotateTowards(player.transform.position);
            agent.isStopped = true;
            agent.speed = 0;
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
                agent.isStopped = true;
                StartBlock();
            }
        }
        // Si el jugador está a rango de ataque, paramos y atacamos
        if (distanceToPlayer < attackRange && !isBlocking)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            
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
            agent.isStopped = false;
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
        animator.SetBool("isBlocking", true); // Mantiene la animación de bloqueo activa
        // Termina el bloqueo después de blockDuration segundos
        Invoke(nameof(StopBlock), blockDuration);
    }

    void StopBlock()
    {
        animator.SetBool("isBlocking", false);
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
    /// </summary>
    public override void AttackEvent()
    {
        // Elegimos un ataque melee de varios posibles
        // (como en Rogue o Mage eliges un tipo de proyectil, aquí eliges un combo).
        int randomAttack = Random.Range(0, numberOfMeleeAttacks);

       // Disparamos el ataque correspondiente
       lookAtPlayerWhileAttacking = true;
        if(isBlocking)
            currentAttackCollider = shieldCollider;
        else
            currentAttackCollider = attackCollider;

        currentAttackCollider.enabled = true;
        
    }

    public void AttackStopEvent()
    {
        lookAtPlayerWhileAttacking = false;
        currentAttackCollider.enabled = false;
    }

    public void AttackEndEvent()
    {
        isAttacking = false;
        StopAttack();
    }
}