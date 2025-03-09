using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Rogue : BaseEnemy
{
    [Header("Flee Params")]
    public float fleeDistance = 7f;
    public float fleeRadius = 1f;
    public int rayCount = 12;
    public float checkRadius = 1f;

    public GameObject spellPrefab; 
    public Transform staffTip;

    // Ejemplo: Elegir ataque aleatorio
    private int randomNumber;

    protected override void Start()
    {
        base.Start();
        randomNumber = UnityEngine.Random.Range(0, 2);
    }

    protected override void HandleCombat()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Si estamos atacando, no nos movemos
        if (isAttacking)
        {
            agent.isStopped = true;
            return;
        }

        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);

        // Huir si el jugador está demasiado cerca
        if (distanceToPlayer <= fleeRadius)
        {
            Vector3 bestFlee = GetBestFleeDirection();
            if (bestFlee != Vector3.zero)
                agent.SetDestination(bestFlee);

            animator.SetBool("isRunning", true);
            return;
        }

        // Si está en rango de ataque
        if (distanceToPlayer < attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            RotateTowards(player.transform.position);

            if (attackTimer >= attackCooldown)
            {
                // Elegir ataque aleatorio
                randomNumber = UnityEngine.Random.Range(0, 2);
                animator.SetInteger("attackType", randomNumber);
                attackTimer = 0f;
                isAttacking = true;
                StartAttack();
            }
        }
        else
        {
            // Moverse hacia el jugador
            animator.SetBool("isRunning", true);
        }
    }

    // Podemos sobrescribir AttackEvent en vez de StartAttack para tener varios tipos
    public override void AttackEvent()
    {
        switch (randomNumber)
        {
            case 0:
                NormalAttack();
                break;
            case 1:
                ConeAttack();
                break;
            default:
                NormalAttack();
                break;
        }
    }

    private void StopAttackEvent()
    {
        Debug.Log("StopAttackEvent **********************");
        isAttacking = false;
    }

    public void AttackStartEvent()
    {
    }

    public override void AttackEndEvent()
    {
    }

    private void NormalAttack()
    {
        if (spellPrefab != null && staffTip != null)
        {
            GameObject spell = Instantiate(spellPrefab, staffTip.position, Quaternion.identity);
            ArrowProjectile arrow = spell.GetComponent<ArrowProjectile>();
            if (arrow != null)
            {
                // Ejemplo de predicción
                Vector3 predictedPos = PredictFuturePosition(arrow.speed);
                arrow.SetTargetPosition(predictedPos);
            }
        }
    }

    private void ConeAttack()
    {
        if (spellPrefab != null && staffTip != null)
        {
            int projectileCount = 5;
            float spreadAngle = 30f;
            Vector3 predictedPos = PredictFuturePosition(10f);

            Vector3 directionToTarget = (predictedPos - staffTip.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(directionToTarget) *
                                      Quaternion.Euler(0f, -spreadAngle / 2f, 0f);
            float angleStep = spreadAngle / (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = i * angleStep;
                Quaternion rot = baseRotation * Quaternion.Euler(0f, currentAngle, 0f);
                GameObject spell = Instantiate(spellPrefab, staffTip.position, rot);
                ArrowProjectile arrow = spell.GetComponent<ArrowProjectile>();
                if (arrow != null)
                {
                    // Ajustar velocidad, etc.
                    arrow.speed = 10f;
                }
            }
        }
    }

    private Vector3 GetBestFleeDirection()
    {
        float maxDistance = 0f;
        Vector3 bestDirection = Vector3.zero;

        // Revisamos varios rayos en círculo
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 potentialPos = transform.position + dir * fleeRadius;

            if (NavMesh.SamplePosition(potentialPos, out NavMeshHit hit, checkRadius, NavMesh.AllAreas))
            {
                float distFromPlayer = Vector3.Distance(hit.position, player.transform.position);
                if (distFromPlayer > maxDistance)
                {
                    maxDistance = distFromPlayer;
                    bestDirection = hit.position;
                }
            }
        }

        return bestDirection;
    }

    // (Opcional) OnDrawGizmos si quieres ver el debug de flee
}
