using System.Collections;

using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Homing Skill", menuName = "ScriptableObject/BossSkills/Homing")]
public class BossHomingSpellSkill : BossSkillScriptableObject
{
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    public Vector3 defaultOffset = Vector3.up * 1f;

    public GameObject spellPrefab;
    protected bool fireballShooted = false;

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

    public override bool InMinRange(Boss boss, GameObject player)
    {
        float distance = Vector3.Distance(boss.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void Use(Boss boss, GameObject player)
    {
        fireballShooted = false;
        base.Use(boss, player);
        boss.animator.SetTrigger("fireball");
        boss.StartCoroutine(UseSkillCoroutine(boss, player));
    }

    public override void OnAnimationEvent(Boss boss, GameObject player)
    {
        base.OnAnimationEvent(boss, player);

        if (spellPrefab != null && player != null)
        {
            if(boss.mouth == null)
            {
                Debug.LogError("No existe el objeto boca en el boss.");
                return;
            }

            Debug.Log("Se lanza el proyectil desde la boca del boss.");
            
            Vector3 bossMouthPos = boss.mouth.transform.position;
            Vector3 spawnPos = bossMouthPos;

            GameObject spell = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();
            if (spellScript != null)
            {
                Debug.LogError("No existe el prefab del proyectil.");
                spellScript.SetTarget(player.transform);
            }

            fireballShooted = true;
            
        }
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(Boss boss, GameObject player)
    {
        // Espera a que la animación "Fireball Shoot" comience.
        while (!boss.animator.GetCurrentAnimatorStateInfo(0).IsName("Fireball Shoot"))
        {
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (boss.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            if(!fireballShooted)
                boss.RotateTowards(player.transform.position);
            else
                boss.SlowlyRotateTowards(player.transform.position);
                
            yield return null;
        }

        // La animación ha finalizado.
        castTime = Time.time;
        isCasting = false;
    }
}