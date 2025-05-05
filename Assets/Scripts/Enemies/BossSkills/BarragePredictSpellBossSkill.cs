using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Barrage Predict Spell Skill", menuName = "ScriptableObject/BossSkills/BossBarragePredict")]
public class BarragePredictSpellBossSkill : BossSkillScriptableObject
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
        boss.animator.SetTrigger("fireballCast");
    }

    public override void HandleMovement(Boss boss, GameObject player){
        // Perseguir al jugador durante el ataque
        boss.AllowMovement();
        boss.agent.SetDestination(player.transform.position);
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
        

        float elapsedTime = 0f;
        float interval = 1f / projectilesPerSec;

        // Pausar la animación para mayor control
        boss.animator.SetFloat("layer1Speed", 0.1f);

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
            BossSpellProjectileManual sp = projectile.GetComponent<BossSpellProjectileManual>();
            if (sp != null)
            {
                Vector3 predictedPos = boss.PredictFuturePosition(sp.speed);
                predictedPos += Random.insideUnitSphere * 1f; // Añadir un pequeño offset aleatorio
                sp.SetTargetPosition(predictedPos);
            }

            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }

        // Restaurar el estado del enemigo al finalizar el ataque
        boss.animator.SetFloat("layer1Speed", 1f);

        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (boss.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
            boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Spell Cast"))
        {
            yield return null;
        }

        castTime = Time.time;
        isCasting = false;
    }
}