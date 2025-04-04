using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class BaseEnemyV2 : BaseEnemy
{
    public GameObject projectileSpawnPoint;
    public Collider meleeAttackCollider;
    public bool isBlocking = false;
    public void RunTowardsPlayer(){
        //animator.SetBool("isRunning", true);
        if(agent != null && agent.isActiveAndEnabled)
            agent.SetDestination(player.transform.position);
    }

    public void StopMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
        //animator.SetBool("isRunning", false);
    }

    public void AllowMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }

    public override void StopAttack(){
        base.StopAttack();
        AllowMovement();
    }

    public virtual void SlowlyRotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
    }

}
