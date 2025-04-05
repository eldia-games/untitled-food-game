using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Barrage Spell Skill", menuName = "ScriptableObject/BossSkills/BossBarrage")]
public class BarrageSpellBossSkill : BossSkillScriptableObject
{
    public float projectilesPerSec = 10.0f;
    public float duration = 1.5f;
    public GameObject spellPrefab;

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
        // Trigger de la animación de ataque (puedes definir un attackType distinto para esta habilidad)
        boss.animator.SetTrigger("fireball");
    }

    public override void OnAnimationEvent(Boss boss, GameObject player)
    {
        base.OnAnimationEvent(boss, player);
        boss.StartCoroutine(Barrage(boss, player));
    }

    private IEnumerator Barrage(Boss boss, GameObject player)
    {
        // Detener el movimiento del enemigo y pausar la animación para mayor control
        boss.StopMovement();
        //enemy.animator.speed = 0f;

        float elapsedTime = 0f;
        float interval = 1f / projectilesPerSec;

        boss.animator.speed = 0f; // Pausar la animación durante el ataque

        while (elapsedTime < duration)
        {
            // Actualizamos la dirección apuntando hacia la posición actual del jugador
            Vector3 bossMouthPos = boss.mouth.transform.position;
            Vector3 spawnPos = bossMouthPos;
            boss.SlowlyRotateTowards(player.transform.position);
            Vector3 direction = (player.transform.position - spawnPos).normalized;

            // Añadir una pequeña variación para simular imprecisión
            float angleOffset = Random.Range(-5f, 5f);
            direction = Quaternion.Euler(1, angleOffset, 1) * direction;

            GameObject projectile = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            SpellProjectileManual sp = projectile.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {
                sp.SetDirection(direction);
            }

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }

        // Restaurar el estado del enemigo al finalizar el ataque
        boss.animator.speed = 1f;

        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (boss.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        castTime = Time.time;
        isCasting = false;
    }
}