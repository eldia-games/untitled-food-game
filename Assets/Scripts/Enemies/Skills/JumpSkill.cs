using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Jump Skill", menuName = "ScriptableObject/Skills/Jump")]
public class JumpSkill : SkillScriptableObject
{
    private Vector3 chargeAttackTarget; 
    public float jumpAttackSpeed = 4.0f;
    public float minRange = 1.0f;
    public float maxRange = 9.0f;

    public override bool CanUseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        if (base.CanUseSkill(enemy, player))
        {
            float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
            bool inRange = distance > minRange && distance <= maxRange;
            bool didCooldownEnd = castTime + cooldown < Time.time;

            // Log del tiempo de cooldown: cuándo termina y el tiempo actual.
            Debug.Log($"[JumpSkill Debug] Cooldown termina en: {castTime + cooldown}, Tiempo actual: {Time.time}");
            
            bool canUse = !isCasting && didCooldownEnd && inRange;
            
            Debug.Log($"[JumpSkill Debug] Enemy: {enemy.name}, Player: {player.name}, Distance: {distance}, InRange: {inRange}, CooldownEnded: {didCooldownEnd}, IsCasting: {isCasting}, CanUse: {canUse}");
            return canUse;
        }

        return false;
    }

    public override void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        base.UseSkill(enemy, player);
        enemy.StartCoroutine(Jump(enemy, player));
    }

    private IEnumerator Jump(BaseEnemyV2 enemy, GameObject player)
    {
        // Detenemos NavMeshAgent para que no se mueva
        enemy.StopMovement();
        enemy.RotateTowards(player.transform.position);

        // Esperamos 1.5s
        yield return new WaitForSeconds(1.5f);

        // Elige la posición de destino para el salto en un círculo alrededor del jugador
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        float randomRadius = Random.Range(1f, 2f);
        Vector3 randomOffset = randomDirection * randomRadius;
        Vector3 targetPos = player.transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
        {
            chargeAttackTarget = hit.position; // Usa la posición correcta del NavMesh, que incluye la altura.
        }
        else
        {
            chargeAttackTarget = player.transform.position;
        }

        yield return null;
        
        // Dispara animación de “salto”
        enemy.animator.SetTrigger("jumpAttack");

        // Ahora hacemos un movimiento “parabólico” desde la posición actual al chargeAttackTarget.
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = chargeAttackTarget;
        float jumpHeight = 3f;   // altura máxima del arco
        float travelTime = 0.8f; // cuánto tarda en completarse el salto
        float t = 0f;

        // Desactivamos el NavMeshAgent mientras volamos, para no “pelearnos” con él
        enemy.agent.enabled = false;

        // Recorremos una curva parabólica entre startPos y endPos
        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            float heightCurve = 4f * jumpHeight * t * (1f - t);
            currentPos.y += heightCurve;
            enemy.transform.position = currentPos;
            Vector3 dir = endPos - startPos;
            dir.y = 0f;
            if (dir != Vector3.zero)
                enemy.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

            yield return null;
        }

        // Al aterrizar, podríamos activar un “hitbox” o llamar AttackEvent() si procede
        enemy.AttackEvent();

        // Reactivar NavMeshAgent
        enemy.agent.enabled = true;
        NavMeshHit nhit;
        if (NavMesh.SamplePosition(endPos, out nhit, 1f, NavMesh.AllAreas))
        {
            enemy.agent.Warp(nhit.position);
        }
        else
        {
            // Si no se encuentra una posición válida, usa la posición original o maneja el error
            enemy.agent.Warp(endPos);
        }

        enemy.AllowMovement();
        enemy.animator.SetBool("isJumpAttack", false);

        // Se deshabilita el daño y termina el ataque
        enemy.StopAttack();
        castTime = Time.time;
        isCasting = false;
    }
}