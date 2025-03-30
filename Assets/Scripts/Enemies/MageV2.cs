using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MageV2 : BaseEnemyV2
{
    [Header("Sonidos")]
    public List<AudioClip> spellSounds;

    [Header("Flee Params")]
    public float fleeDistance = 7f;
    public float fleeRadius = 1f;
    public int rayCount = 12;
    public float checkRadius = 1f;

    public Transform staffTip;

    public SkillScriptableObject[] skills;
    private SkillScriptableObject currentSkill;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Initialize();
        }
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

            // Si no se está ejecutando una habilidad se intenta lanzar alguna
            if(!currentSkill)
                for (int i = 0; i < skills.Length; i++)
                {
                    if (skills[i].CanUseSkill(this, player))
                    {
                        currentSkill = skills[i];
                        skills[i].UseSkill(this, player);
                        return;
                    }
                }
        }
        else
        {
            // Moverse hacia el jugador
            animator.SetBool("isRunning", true);
        }
    }

    public void PlayRandomSpellSound()
    {
        if (spellSounds != null)
        {
            // Convertir la lista de sonidos de paso a un array
            AudioClip[] spellSoundsArray = spellSounds.ToArray();

            // Obtener un índice aleatorio dentro del rango del array
            int randomIndex = Random.Range(0, spellSoundsArray.Length);

            // Coger un sonido de paso aleatorio y reproducirlo
            AudioClip footstepSound = spellSoundsArray[randomIndex];
            // Pitch aleatorio para mayor variedad
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(footstepSound);
        }
    }

    public void OnSpellAnimationEvent()
    {
        if (currentSkill != null)
        {
            currentSkill.OnAnimationEvent(this, player);
            currentSkill = null;
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
}
