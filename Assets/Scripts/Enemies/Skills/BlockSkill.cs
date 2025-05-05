using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Skill", menuName = "ScriptableObject/Skills/Block")]
public class BlockSkill : SkillScriptableObject
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
        enemy.animator.SetTrigger("block");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("block");
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
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(BaseEnemyV2 enemy, GameObject player)
    {
        enemy.isBlocking = true;
        // Espera a que la animación comience.
        while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {
            enemy.RotateTowards(player.transform.position);
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
               && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {
            enemy.LookAt(player.transform.position);
                
            yield return null;
        }
        enemy.isBlocking = false;
        // La animación ha finalizado.
        castTime = Time.time;
        isCasting = false;
    }

    public override void Stop(BaseEnemyV2 enemy, GameObject player)
    {
        base.Stop(enemy, player);
        // Interrumpimos la corrutina si está en curso
        if (enemy.IsInvoking("UseSkillCoroutine"))
        {
            enemy.StopCoroutine(UseSkillCoroutine(enemy, player));
        }
    }

}