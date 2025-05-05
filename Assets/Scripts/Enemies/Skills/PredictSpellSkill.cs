using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Predict Attack Skill", menuName = "ScriptableObject/Skills/Predict")]
public class PredictSpellSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 10.0f;
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    public Vector3 defaultOffset = Vector3.up * 1f;

    public float projectileSpeed = 10f;

    public GameObject spellPrefab;
    protected bool shooted = false;
    protected Vector3 predictedPos;

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
            float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
            bool tooClose = distance < minRange;
            bool didCooldownEnd = castTime + cooldown < Time.time;
            
            bool canUse = !isCasting && didCooldownEnd && !tooClose;
            return canUse;
        }

        return false;
    }

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        enemy.animator.SetTrigger("shoot1H");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("shoot1H");
        enemy.StartCoroutine(UseSkillCoroutine(enemy, player));
    }

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange && enemy.IsInLineOfSight(player.transform.position);
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);

        if (spellPrefab != null)
        {

            Vector3 spawnPos = enemy.transform.position + defaultOffset;
            if (enemy.projectileSpawnPoint != null)
            {
                spawnPos = enemy.projectileSpawnPoint.transform.position;
            }

            GameObject spell = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            ArrowProjectile arrow = spell.GetComponent<ArrowProjectile>();
            if (arrow != null)
            {
                // Ejemplo de predicción
                arrow.speed = projectileSpeed;
                predictedPos = enemy.PredictFuturePosition(arrow.speed);
                arrow.SetTargetPosition(predictedPos);
            }
        }
        shooted = true;
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(BaseEnemyV2 enemy, GameObject player)
    {
        // Espera a que la animación comience.
        while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("1H Ranged Shoot"))
        {
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
               && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("1H Ranged Shoot"))
        {
            if(!shooted)
                enemy.LookAt(player.transform.position);
            else
                enemy.SlowlyRotateTowards(predictedPos);
                
            yield return null;
        }
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