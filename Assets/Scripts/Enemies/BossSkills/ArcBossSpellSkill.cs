using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Arc Boss Spell Skill", menuName = "ScriptableObject/BossSkills/BossArc")]
public class BossArcSpellSkill : BossSkillScriptableObject
{
    
    // Número de proyectiles de la ráfaga (se recomienda un número impar para que el centro quede alineado)
    public int projectileCount = 5;
    // Ángulo máximo (en grados) que se aplicará a los proyectiles laterales respecto a la dirección central
    public float maxAngleOffset = 30f;
    public GameObject spellPrefab;
    public float speed = 10.0f;

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
        // Configura la animación que corresponda a esta habilidad (por ejemplo, attackType 3)
        boss.animator.SetTrigger("scream");
    }

    // Este método se invoca desde el evento de animación
    public override void OnAnimationEvent(Boss boss, GameObject player)
    {
        base.OnAnimationEvent(boss, player);
        boss.StartCoroutine(Arc(boss, player));
    }

    private IEnumerator Arc(Boss boss, GameObject player)
    {
        // Se detiene el movimiento y se orienta el enemigo hacia el jugador
        boss.RotateTowards(player.transform.position);

        // Calculamos la dirección central (directamente hacia el jugador)
        Vector3 baseDirection = (player.transform.position - boss.transform.position).normalized;

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
            // Añadir una variación aleatoria para simular imprecisión
            float randomAngleOffset = Random.Range(-5f, 5f);
            projectileDirection = Quaternion.Euler(0, randomAngleOffset, 0) * projectileDirection;

            // Se define la posición de spawn. Si el enemigo es un MageV2 y tiene asignado un staffTip, se usa esa posición
            Vector3 spawnPos = boss.transform.position + Vector3.up * 1f;
            // Añadir offset aleatorio para la posición de spawn
            spawnPos += Random.insideUnitSphere * 0.5f; // Ajusta el valor según sea necesario
            

            // Instancia el proyectil y se le asigna la dirección
            GameObject projectile = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
            BossSpellProjectileManual sp = projectile.GetComponent<BossSpellProjectileManual>();
            if (sp != null)
            {
                sp.SetSpeed(speed);
                sp.SetDirection(projectileDirection);
            }
        }

        // Si se desea se puede esperar un frame o un tiempo específico
        yield return new WaitForSeconds(0.3f);

        // Se restablece el estado del enemigo y se marca el tiempo de lanzamiento
        castTime = Time.time;
        isCasting = false;
    }
}