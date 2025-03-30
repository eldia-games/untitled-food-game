using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Arc Spell Skill", menuName = "ScriptableObject/Skills/Arc")]
public class ArcSpellSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 20.0f;
    
    // Número de proyectiles de la ráfaga (se recomienda un número impar para que el centro quede alineado)
    public int projectileCount = 5;
    // Ángulo máximo (en grados) que se aplicará a los proyectiles laterales respecto a la dirección central
    public float maxAngleOffset = 30f;
    public GameObject spellPrefab;
    public float speed = 10.0f;

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
        // Configura la animación que corresponda a esta habilidad (por ejemplo, attackType 3)
        enemy.animator.SetTrigger("spell");
        enemy.StopMovement();
    }

    // Este método se invoca desde el evento de animación
    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);
        enemy.StartCoroutine(Arc(enemy, player));
    }

    private IEnumerator Arc(BaseEnemyV2 enemy, GameObject player)
    {
        // Se detiene el movimiento y se orienta el enemigo hacia el jugador
        enemy.RotateTowards(player.transform.position);

        // Calculamos la dirección central (directamente hacia el jugador)
        Vector3 baseDirection = (player.transform.position - enemy.transform.position).normalized;

        // Se asume que projectileCount es impar para lograr simetría
        int halfCount = projectileCount / 2;
        // Se determina el incremento angular para los proyectiles laterales
        float angleStep = projectileCount > 1 ? maxAngleOffset / halfCount : 0f;

        // Se instancia cada proyectil
        for (int i = 0; i < projectileCount; i++)
        {
            // El índice relativo al centro: el central tendrá offset 0,
            // los de la izquierda tendrán índices negativos y los de la derecha positivos
            int offsetIndex = i - halfCount;
            float angleOffset = offsetIndex * angleStep;
            
            // Se rota la dirección base en torno al eje Y según el ángulo calculado
            Vector3 projectileDirection = Quaternion.Euler(0, angleOffset, 0) * baseDirection;

            // Se define la posición de spawn. Si el enemigo es un MageV2 y tiene asignado un staffTip, se usa esa posición
            Vector3 spawnPos = enemy.transform.position + Vector3.up * 1f;
            if (enemy is MageV2 mage && mage.staffTip != null)
            {
                spawnPos = mage.staffTip.position;
            }

            // Instancia el proyectil y se le asigna la dirección
            GameObject projectile = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            SpellProjectileManual sp = projectile.GetComponent<SpellProjectileManual>();
            if (sp != null)
            {
                sp.SetSpeed(speed);
                sp.SetDirection(projectileDirection);
            }
        }

        // Si se desea se puede esperar un frame o un tiempo específico
        yield return new WaitForSeconds(0.3f);

        enemy.AllowMovement();

        // Se restablece el estado del enemigo y se marca el tiempo de lanzamiento
        castTime = Time.time;
        isCasting = false;
        enemy.StopAttack();
    }
}