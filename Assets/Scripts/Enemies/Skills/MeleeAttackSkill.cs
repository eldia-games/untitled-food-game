using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Attack Skill", menuName = "ScriptableObject/Skills/MeleeAttack")]
public class MeleeAttackSkill : SkillScriptableObject
{
    public float minRange = 2.0f;
    public float maxRange = 2.0f;
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    protected bool firstEvent = true;

    public override bool CanUseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        if (base.CanUseSkill(enemy, player))
        {
            float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
            bool inRange = distance > minRange && distance <= maxRange;
            bool didCooldownEnd = castTime + cooldown < Time.time;
            
            bool canUse = !isCasting && didCooldownEnd && inRange;
            return canUse;
        }

        return false;
    }

    public override bool CanUse(BaseEnemyV2 enemy, GameObject player)
    {
        if (base.CanUse(enemy, player))
        {
            bool didCooldownEnd = castTime + cooldown < Time.time;
            
            bool canUse = !isCasting && didCooldownEnd;
            return canUse;
        }

        return false;
    }

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        enemy.animator.SetTrigger("attack");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("attack");
        enemy.StartCoroutine(UseSkillCoroutine(enemy, player));
    }

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);

        Collider attackCollider = enemy.meleeAttackCollider;
        if(firstEvent)
        {
            firstEvent = false;
            if (attackCollider != null)
            {
                attackCollider.enabled = true;
            }
        }
        else
        {
            firstEvent = true;
            if (attackCollider != null)
            {
                attackCollider.enabled = false;
            }
        }
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(BaseEnemyV2 enemy, GameObject player)
    {
        // Espera a que la animación comience.
        while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("1H Melee Horizontal"))
        {
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
               && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("1H Melee Horizontal"))
        {
            if(!firstEvent)
                enemy.RotateTowards(player.transform.position);
            else
                enemy.SlowlyRotateTowards(player.transform.position);
                
            yield return null;
        }
        // La animación ha finalizado.
        castTime = Time.time;
        isCasting = false;
    }

}