using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Attack Boss Skill", menuName = "ScriptableObject/BossSkills/BossMeleeAttack")]
public class MeleeAttackBossSkill : BossSkillScriptableObject
{
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    protected bool firstEvent = true;

    public override bool CanUse(Boss boss, GameObject player)
    {
        if (base.CanUse(boss, player))
        {
            bool didCooldownEnd = castTime + cooldown < Time.time;
            
            bool canUse = !isCasting && didCooldownEnd;
            return canUse;
        }

        return false;
    }
    public override void Use(Boss boss, GameObject player)
    {
        base.Use(boss, player);
        boss.animator.SetTrigger("attack");
        boss.StartCoroutine(AnimateAimConstraintWeight(boss, 0.0f, 0.2f));
        boss.StartCoroutine(UseSkillCoroutine(boss, player));
    }

    public override bool InMinRange(Boss boss, GameObject player)
    {
        float distance = Vector3.Distance(boss.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void OnAnimationEvent(Boss boss, GameObject player)
    {
        base.OnAnimationEvent(boss, player);

        if(firstEvent)
        {
            firstEvent = false;
            boss.ActivateMouthCollider();
        }
        else
        {
            firstEvent = true;
            boss.DeactivateMouthCollider();
        }
    }

    private IEnumerator AnimateAimConstraintWeight(Boss boss, float targetWeight, float duration)
    {
        float elapsedTime = 0f;
        float initialWeight = boss.aimConstraint.weight;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            boss.aimConstraint.weight = Mathf.Lerp(initialWeight, targetWeight, elapsedTime / duration);
            yield return null;
        }
        
        boss.aimConstraint.weight = targetWeight;
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(Boss boss, GameObject player)
    {
        // Espera a que la animación comience.
        while (!boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack"))
        {
            yield return null;
        }
        
        while (boss.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
               && boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack"))
        {
            if(!firstEvent)
                boss.SlowlyRotateTowards(player.transform.position);
            else
                boss.RotateTowards(player.transform.position);
                
            yield return null;
        }
        // La animación ha finalizado.
        boss.StartCoroutine(AnimateAimConstraintWeight(boss, 1.0f, 0.5f));
        castTime = Time.time;
        isCasting = false;
    }

}