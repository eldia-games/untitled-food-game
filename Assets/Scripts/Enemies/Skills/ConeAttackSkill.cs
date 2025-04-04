using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Cone Attack Skill", menuName = "ScriptableObject/Skills/Cone")]
public class ConeAttackSkill : SkillScriptableObject
{
    public float minRange = 5.0f;
    public float maxRange = 10.0f;
    // En caso de que no exista ningun origen especifico, se usara este offset por defecto
    public Vector3 defaultOffset = Vector3.up * 1f;

    public float spreadAngle = 30f; // Angulo del cono
    public int projectileCount = 5; // Cantidad de proyectiles a disparar
    public float projectileSpeed = 10f; // Velocidad de los proyectiles

    public GameObject arrowPrefab;
    protected bool shooted = false;


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
            bool didCooldownEnd = castTime + cooldown < Time.time;
            
            bool canUse = !isCasting && didCooldownEnd;
            return canUse;
        }

        return false;
    }

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        enemy.animator.SetTrigger("shoot2H");
    }

    public override void Use(BaseEnemyV2 enemy, GameObject player)
    {
        base.Use(enemy, player);
        enemy.animator.SetTrigger("shoot2H");
        enemy.StartCoroutine(UseSkillCoroutine(enemy, player));
    }

    public override bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance <= maxRange;
    }

    public override void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        base.OnAnimationEvent(enemy, player);

        if (arrowPrefab != null)
        {
            Vector3 predictedPos = player.transform.position;

            Vector3 spawnPos = enemy.transform.position + defaultOffset;
            if (enemy.projectileSpawnPoint != null)
            {
                spawnPos = enemy.projectileSpawnPoint.transform.position;
            }

            Vector3 directionToTarget = (predictedPos - spawnPos).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(directionToTarget) *
                                      Quaternion.Euler(0f, -spreadAngle / 2f, 0f);
            float angleStep = spreadAngle / (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = i * angleStep;
                Quaternion rot = baseRotation * Quaternion.Euler(0f, currentAngle, 0f);
                GameObject arrowObject = Instantiate(arrowPrefab, spawnPos, rot);
                ArrowProjectile arrow = arrowObject.GetComponent<ArrowProjectile>();
                if (arrow != null)
                {
                    arrow.speed = projectileSpeed;
                } else {
                    Debug.LogError("El prefab de la flecha no tiene el componente ArrowProjectile.");
                }
            }
        } else {
            Debug.LogError("El prefab de la flecha no está asignado en el scriptable object.");
        }
        shooted = true;
    }

    // Corrrutina para usar la habilidad
    private IEnumerator UseSkillCoroutine(BaseEnemyV2 enemy, GameObject player)
    {
        // Espera a que la animación comience.
        while (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("2H Ranged Shoot"))
        {
            Debug.Log("Esperando a que comience la animación de ataque de cono.");
            yield return null;
        }
        
        // Una vez que ha comenzado, espera hasta que se complete (normalizedTime >= 1.0)
        while (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
               && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("2H Ranged Shoot"))
        {
            Debug.Log("Esperando a que termine la animación de ataque de cono.");
            if(!shooted)
                enemy.LookAt(player.transform.position);
            else
                enemy.SlowlyRotateTowards(player.transform.position);
                
            yield return null;
        }
        // La animación ha finalizado.
        castTime = Time.time;
        isCasting = false;
    }

}