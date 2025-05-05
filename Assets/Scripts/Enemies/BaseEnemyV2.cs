using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class BaseEnemyV2 : BaseEnemy
{
    public GameObject projectileSpawnPoint;
    public Collider meleeAttackCollider;
    [Header("Trail")]
    public TrailRenderer trail;
    public bool isBlocking = false;
    public bool debug = false;
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

    public virtual bool IsInLineOfSight(Vector3 targetPos)
    {
        RaycastHit hit;
        Vector3 direction = (targetPos - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }


    protected override void Start()
    {
        base.Start();
    }


}
