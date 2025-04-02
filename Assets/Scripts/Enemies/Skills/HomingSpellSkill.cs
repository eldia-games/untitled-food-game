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

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        enemy.animator.SetTrigger("spell");
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
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
        isCasting = false;
        castTime = Time.time;
    }


}