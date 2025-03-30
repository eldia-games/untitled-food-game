using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Minion : BaseEnemy
{
    [Header("Sonidos")]
    public AudioClip attackSound;
    
    public bool canJumpAttack;       // Controla si este enemigo está habilitado para saltar
    public bool chargingJumpAttack;  // Indica si estamos en fase de cargar el salto
    public bool jumpingAttack;       // Indica si ya estamos “en el aire” saltando
    public Vector3 chargeAttackTarget; 
    public float jumpAttackSpeed;
    public float jumpAttackRange;    // Rango para decidir que conviene usar el salto

    public bool isJumpAttack = false;
    public float jumpAttackCooldown = 8f; // el tiempo mínimo entre saltos
    private float jumpAttackTimer = 0f;   // para contar el tiempo desde el último salto

    public Collider currentAttackCollider; // El collider del ataque actual

    protected override void Start()
    {
        base.Start();
    }

    protected override void HandleCombat()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Si estamos en un ataque cualquiera (animación) no nos movemos.
        if (isAttacking)
        {
            StopMovement();
            return;
        }
        
        canJumpAttack = distanceToPlayer < jumpAttackRange &&
            jumpAttackTimer >= jumpAttackCooldown;

        // Primero chequeamos si queremos saltar.
        // (Por ejemplo, si 'canJumpAttack' está activo y el jugador está en jumpAttackRange).
        if (canJumpAttack)
        {
            // Si no estamos ya cargando un salto, ni saltando, podemos hacerlo
            if (!chargingJumpAttack && !jumpingAttack)
            {
                // Reiniciamos el temporizador
                jumpAttackTimer = 0f;

                // Iniciamos la corrutina de salto
                StartCoroutine(JumpAttackRoutine());
            }

            RunTowardsPlayer();
        } 
        else if (distanceToPlayer < attackRange)
        {
            // Parar y mirar al jugador
            StopMovement();
            RotateTowards(player.transform.position);

            // Comprobamos cooldown
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                StartAttack(); // ataque normal (animación melee, por ejemplo)
            }
        } else {
            RunTowardsPlayer();
        }

        if (chargingJumpAttack || jumpingAttack)
            StopMovement();
    }

    public void RunTowardsPlayer(){
        AllowMovement();
        animator.SetBool("isRunning", true);
        if(agent != null && agent.isActiveAndEnabled)
            agent.SetDestination(player.transform.position);
    }

    public void StopMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
        animator.SetBool("isRunning", false);
    }

    public void AllowMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }

    private IEnumerator JumpAttackRoutine()
    {
        isJumpAttack = true;
        // Fase de carga
        chargingJumpAttack = true;
        // Detenemos NavMeshAgent para que no se mueva
        StopMovement();
        RotateTowards(player.transform.position);

        // Esperamos 1.5s
        yield return new WaitForSeconds(1.5f);

        // Elige la posición de destino para el salto en un círculo alrededor del jugador
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        float randomRadius = Random.Range(1f, 3f);
        Vector3 randomOffset = randomDirection * randomRadius;
        Vector3 targetPos = player.transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
        {
            chargeAttackTarget = hit.position; // Usa la posición correcta del NavMesh, que incluye la altura.
        }
        else
        {
            chargeAttackTarget = player.transform.position;
        }

        yield return new WaitForSeconds(0.1f);

        chargingJumpAttack = false;
        jumpingAttack = true;
        
        // Dispara animación de “salto”
        animator.SetTrigger("jumpAttack");

        // Ahora hacemos un movimiento “parabólico” desde la posición actual al chargeAttackTarget.
        Vector3 startPos = transform.position;
        Vector3 endPos = chargeAttackTarget;
        float jumpHeight = 3f;   // altura máxima del arco
        float travelTime = 0.8f; // cuánto tarda en completarse el salto
        float t = 0f;

        // Desactivamos el NavMeshAgent mientras volamos, para no “pelearnos” con él
        agent.enabled = false;

        // Recorremos una curva parabólica entre startPos y endPos
        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;

            // Interpolación horizontal
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            // “Arco” en Y: 4 * altura * t * (1 - t) crea una parábola en el rango [0..1]
            float heightCurve = 4f * jumpHeight * t * (1f - t);
            currentPos.y += heightCurve;

            transform.position = currentPos;

            // (Opcional) Mirar hacia delante (para no rotar bruscamente en el aire)
            Vector3 dir = endPos - startPos;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

            yield return null;
        }

        // Al aterrizar, podríamos activar un “hitbox” o llamar AttackEvent() si procede
        AttackEvent();

        // Reactivar NavMeshAgent
        agent.enabled = true;
        NavMeshHit nhit;
        if (NavMesh.SamplePosition(endPos, out nhit, 1f, NavMesh.AllAreas))
        {
            agent.Warp(nhit.position);
        }
        else
        {
            // Si no se encuentra una posición válida, usa la posición original o maneja el error
            agent.Warp(endPos);
        }
        AllowMovement();

        jumpingAttack = false;
        animator.SetBool("isJumpAttack", false);
        isJumpAttack = false;
        StopAttack(); // Indicamos que finalizó el ataque
    }

    public override void AttackEvent()
    {
        base.AttackEvent();

        currentAttackCollider.enabled = true;

        // Reproduce el sonido de ataque
        if (attackSound != null && audioSource != null)
           audioSource.PlayOneShot(attackSound);
    }

    private void StopAttackMesh()
    {
        StopAttack(); 
    }

    public void AttackStopEvent()
    {
        currentAttackCollider.enabled = false;
        isAttacking = false;
        StopAttack();
    }


    protected override void UpdateTimers()
    {
        base.UpdateTimers();  // mantiene la lógica de cooldown de tu ataque normal

        // Sumar tiempo al cooldown de salto
        if (jumpAttackTimer < jumpAttackCooldown)
        {
            jumpAttackTimer += Time.deltaTime;
        }
    }
}
