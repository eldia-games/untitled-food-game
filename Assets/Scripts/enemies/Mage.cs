using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mage : BaseEnemy
{
    [Header("Sonidos")]
    public List<AudioClip> spellSounds;

    [Header("Flee Params")]
    public float fleeDistance = 7f;
    public float fleeRadius = 1f;
    public int rayCount = 12;
    public float checkRadius = 1f;

    public GameObject spellPrefab;
    public GameObject spellManualPrefab;
    public Transform staffTip;

    public float spiralSpellDuration = 1f;

    private int randomNumber;

    protected override void Start()
    {
        base.Start();
        randomNumber = 0;
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
                // Elegir ataque aleatorio entre 0 y 1, 0 es ataque simple, 1 es ataque especial
                // 0 tiene probabilidad 0.7 y 1 tiene probabilidad 0.3
                randomNumber = UnityEngine.Random.Range(0, 100) < 85 ? 0 : 1;
                animator.SetInteger("attackType", randomNumber);
                attackTimer = 0f;
                StartAttack();
            }
        }
        else
        {
            // Moverse hacia el jugador
            animator.SetBool("isRunning", true);
        }
    }

    public override void AttackEvent()
    {
        switch (randomNumber)
        {
            case 0:
                SimpleAttack();
                //PlayRandomSpellSound();
                Invoke(nameof(StopAttack), 0.2f);
                break;
            case 1:
                // Animación especial, ataque en espiral
                animator.speed = 0f; // Pausa anim
                //PlayRandomSpellSound();
                StartCoroutine(SpiralAttack(spellManualPrefab, staffTip.position, spiralSpellDuration, 1f, 80));
                Invoke(nameof(EndSpiral), 3f);
                break;
            case 2:
                SimpleAttack();
                //PlayRandomSpellSound();
                Invoke(nameof(StopAttack), 0.2f);
                break;
            default:
                SimpleAttack();
                //PlayRandomSpellSound();
                Invoke(nameof(StopAttack), 0.2f);
                break;
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
        }
    }

    private void SimpleAttack()
    {
        if (spellPrefab != null && player != null)
        {
            GameObject spell = Instantiate(spellPrefab, staffTip.position, Quaternion.identity);
            SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();

            if (spellScript != null)
            {
                spellScript.SetTarget(player.transform);
            }
        }
    }

    private IEnumerator SpiralAttack(GameObject spell, Vector3 origin, float duration, float spiralSpeed, int projPerSec)
    {
        float elapsedTime = 0f;
        float angle = 0f;
        float interval = 1f / projPerSec;

        while (elapsedTime < duration)
        {
            Debug.Log("Spiral Attack");
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            GameObject s = Instantiate(spell, origin - Vector3.up * 1f, Quaternion.identity);
            SpellProjectileManual sp = s.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {
                sp.SetDirection(direction);
            }

            angle += Mathf.PI * 2f * (spiralSpeed / projPerSec);
            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    private void EndSpiral()
    {
        animator.speed = 1f;
        StopAttack();
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
