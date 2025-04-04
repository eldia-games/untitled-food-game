using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Homing Attack Skill", menuName = "ScriptableObject/Skills/Homing")]
public class HomingSpellSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 10.0f;
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    public Vector3 defaultOffset = Vector3.up * 1f;

    public GameObject spellPrefab;
    protected bool fireballShooted = false;

    public GameObject staffTip;

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
        Debug.Log("Usando habilidad de ataque homing");
        base.UseSkill(enemy, player);
        enemy.animator.SetTrigger("spellShoot");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("spellShoot");
        enemy.StartCoroutine(UseSkillCoroutine(enemy, player));
    }

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange && enemy.IsInLineOfSight(player.transform.position);;
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        Debug.Log("Ha salido el evento de animación de ataque homing");
        base.OnAnimationEvent(enemy, player);

        if (spellPrefab != null && player != null)
        {
            Vector3 spawnPos = enemy.transform.position + defaultOffset;
            if (enemy is MageV2 mage && mage.staffTip != null)
            {
                spawnPos = mage.staffTip.position;
            }

            GameObject spell = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();
            if (spellScript != null)
            {
                // Por ejemplo, si usas una función SetTarget en el proyectil:
                spellScript.SetTarget(player.transform);
            }
            
        }
        fireballShooted = true;
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(BaseEnemyV2 enemy, GameObject player)
    {
        // Espera a que la animación "Fireball Shoot" comience.
        while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Spell Shoot"))
        {
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
            enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Spell Shoot"))
        {
            if(!fireballShooted)
                enemy.RotateTowards(player.transform.position);
            else
                enemy.SlowlyRotateTowards(player.transform.position);
                
            yield return null;
        }

        Debug.Log("La habilidad ha terminado de usarse.");
        // La animación ha finalizado.
        castTime = Time.time;
        isCasting = false;
    }

}