using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionV2 : BaseEnemyV2
{
    [Header("Sonidos")]
    public AudioClip attackSound;

    public Collider currentAttackCollider; // El collider del ataque actual

    public SkillScriptableObject[] skills;

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

        // Si estamos en un ataque cualquiera (animación) no nos movemos.
        if (isAttacking)
        {
            StopMovement();
            return;
        }

        // El ataque normal tiene prioridad respecto a las habilidades
        if (distanceToPlayer < attackRange)
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
            
            // Probando habilidades
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i].CanUseSkill(this, player))
                {
                    skills[i].UseSkill(this, player);
                    return;
                }
            }
            RunTowardsPlayer();
        }
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
}
