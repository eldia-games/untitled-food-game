using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Spike Formation Spell Skill", menuName = "ScriptableObject/Skills/SpikeFormation")]
public class SpikeFormationSpellSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 20.0f;
    
    // Número de proyectiles (se recomienda un número impar para tener un centro)
    public int projectileCount = 5;
    // Espaciado lateral entre cada proyectil
    public float lateralSpacing = 0.5f;
    // Factor de desplazamiento hacia atrás (los proyectiles laterales se posicionan un poco atrás para formar la punta)
    public float backwardOffsetFactor = 0.5f;
    // Velocidad de los proyectiles
    public float projectileSpeed = 15f; // Velocidad de los proyectiles
    
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
    
    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        // Configuramos la animación correspondiente para esta habilidad
        enemy.animator.SetTrigger("spell");
    }
    
    // Este método se llama desde el evento de animación
    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);
        enemy.StartCoroutine(SpikeFormation(enemy, player));
    }
    
    private IEnumerator SpikeFormation(BaseEnemyV2 enemy, GameObject player)
    {
        // Detenemos el movimiento y orientamos al enemigo hacia el jugador
        enemy.StopMovement();
        enemy.RotateTowards(player.transform.position);
        
        // Dirección base: desde el enemigo hacia el jugador
        Vector3 baseDirection = (player.transform.position - enemy.transform.position).normalized;
        
        // Definimos la posición de spawn. Si el enemigo es un MageV2 y tiene asignado un staffTip, lo usamos.
        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
        if (enemy is MageV2 mage && mage.staffTip != null)
        {
            spawnPos = mage.staffTip.position;
        }
        
        // Para obtener el vector "right" (lateral) relativo a la dirección base
        Vector3 right = Vector3.Cross(baseDirection, Vector3.up).normalized;
        
        // Se asume que projectileCount es impar para que el centro quede alineado.
        int halfCount = projectileCount / 2;
        
        for (int i = 0; i < projectileCount; i++)
        {
            int offsetIndex = i - halfCount;
            
            // Desplazamiento lateral: los proyectiles se separan a la derecha o izquierda según su índice.
            Vector3 lateralOffset = right * offsetIndex * lateralSpacing;
            
            // Desplazamiento hacia atrás: los proyectiles laterales se posicionan progresivamente más atrás para formar la "punta".
            float backwardOffset = Mathf.Abs(offsetIndex) * backwardOffsetFactor;
            Vector3 backwardOffsetVec = -baseDirection * backwardOffset;
            
            // Posición final del proyectil
            Vector3 projectileSpawnPos = spawnPos + lateralOffset + backwardOffsetVec;
            
            // Instanciamos el proyectil y lo orientamos en la dirección base
            GameObject projectile = Instantiate(spellPrefab, projectileSpawnPos, Quaternion.LookRotation(baseDirection));
            SpellProjectileManual sp = projectile.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {
                sp.SetDirection(baseDirection);
                sp.SetSpeed(projectileSpeed);
            }
        }
        
        yield return null;
        
        // Se restablece el estado del enemigo y se actualiza el cooldown
        castTime = Time.time;
        isCasting = false;
        enemy.StopAttack();
    }

    public override void Stop(BaseEnemyV2 enemy, GameObject player)
    {
        base.Stop(enemy, player);
        // Interrumpimos la corrutina si está en curso
        if (enemy.IsInvoking("SpikeFormation"))
        {
            enemy.StopCoroutine(SpikeFormation(enemy, player));
        }
    }
}
