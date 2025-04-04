using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Spiral Spell Skill", menuName = "ScriptableObject/Skills/Spiral")]
public class SpiralSpellSkill : SkillScriptableObject
{
    public float jumpAttackSpeed = 4.0f;
    public float minRange = 5.0f;
    public float maxRange = 14.0f;

    public float projectilesPerSec = 50.0f;
    public float duration = 2.0f;
    public float spiralSpeed = 1.0f;
    public GameObject spellPrefab;

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

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        // Animación de cast largo
        enemy.animator.SetInteger("attackType", 1);
        enemy.animator.SetTrigger("attack");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        // Animación de cast largo
        enemy.animator.SetTrigger("spellRaise");
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);
        enemy.StartCoroutine(Spiral(enemy, player));
    }

    private IEnumerator Spiral(BaseEnemyV2 enemy, GameObject player)
    {
        // Detenemos NavMeshAgent para que no se mueva
        enemy.StopMovement();
        enemy.RotateTowards(player.transform.position);
        enemy.animator.speed = 0f;

        float elapsedTime = 0f;
        float angle = 0f;
        float interval = 1f / projectilesPerSec;

        while (elapsedTime < duration)
        {
            enemy.StopMovement();
            enemy.RotateTowards(player.transform.position);
            float timningOffsetPercentage = Random.Range(-0.2f, 0.2f);
            float angleOffsetted = angle * (1f + timningOffsetPercentage);
            Vector3 direction = new Vector3(Mathf.Cos(angleOffsetted), 0f, Mathf.Sin(angleOffsetted));
            GameObject s = Instantiate(spellPrefab, enemy.transform.position + Vector3.up * 1f, Quaternion.identity);
            SpellProjectileManual sp = s.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {

                sp.SetDirection(direction);
            }

            angle += Mathf.PI * 2f * (spiralSpeed / projectilesPerSec);
            elapsedTime += interval * (1f + timningOffsetPercentage);
            yield return new WaitForSeconds(interval);
        }

        // Se deshabilita el daño y termina el ataque
        enemy.animator.speed = 1f;

        // Espera hasta que se complete la animacion (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        castTime = Time.time;
        isCasting = false;
    }
}