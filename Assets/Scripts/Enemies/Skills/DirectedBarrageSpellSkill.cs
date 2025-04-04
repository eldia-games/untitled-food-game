using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Directed Barrage Spell Skill", menuName = "ScriptableObject/Skills/DirectedBarrage")]
public class DirectedBarrageSpellSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 20.0f;
    public float projectilesPerSec = 10.0f;
    public float duration = 1.5f;
    public GameObject spellPrefab;

    public override bool CanUseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        if (base.CanUseSkill(enemy, player))
        {
            float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
            bool inRange = distance > minRange && distance <= maxRange;
            bool didCooldownEnd = castTime + cooldown < Time.time;
            return !isCasting && didCooldownEnd && inRange;
        }
        return false;
    }

    public override bool CanUse(BaseEnemyV2 enemy, GameObject player)
    {
        if (base.CanUseSkill(enemy, player))
        {
            bool didCooldownEnd = castTime + cooldown < Time.time;
            return !isCasting && didCooldownEnd;
        }
        return false;
    }

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("spellCast");
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);
        enemy.StartCoroutine(Barrage(enemy, player));
    }

    private IEnumerator Barrage(BaseEnemyV2 enemy, GameObject player)
    {
        // Detener el movimiento del enemigo y pausar la animación para mayor control
        enemy.StopMovement();

        float elapsedTime = 0f;
        float interval = 1f / projectilesPerSec;

        while (elapsedTime < duration)
        {
            // Actualizamos la dirección apuntando hacia la posición actual del jugador
            enemy.RotateTowards(player.transform.position);
            Vector3 direction = (player.transform.position - enemy.transform.position).normalized;

            // Añadir una pequeña variación para simular imprecisión
            float angleOffset = Random.Range(-5f, 5f);
            direction = Quaternion.Euler(0, angleOffset, 0) * direction;

            // Instanciar el proyectil, se puede ajustar el punto de spawn
            Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
            GameObject projectile = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            SpellProjectileManual sp = projectile.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {
                sp.SetDirection(direction);
            }

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }

        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (
            enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Spell Cast") &&
            enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
            )
        {
            yield return null;
        }

        // Restaurar el estado del enemigo al finalizar el ataque
        castTime = Time.time;
        isCasting = false;
    }
}