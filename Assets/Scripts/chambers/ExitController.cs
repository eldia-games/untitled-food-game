using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.AI;

public class ExitController : MonoBehaviour
{
    public float speed;
    public float health;
    public float damage;
    // Get the player object
    public GameObject player;
    // Rigidbody component for physics
    public Rigidbody rb;
    public NavMeshAgent agent;
    // Rango del ataque
    public float attackRange;
    public Animator animator;

    public MeshRenderer attackMesh;

    // Parametros de vista
    public float viewRadius;
    public float viewAngle;

    private float timer = 0;
    private Vector3 initialPosition;
    private bool inCombat = false;

    public float timeToReset;
    public bool isWaitAndMove;

    // List of possibles weapons
    public List<GameObject> weapons;

    public UnityEvent dieEvent;

    private bool isSeen = false;

    public bool isDead = false;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("sale");
        //TODO salir
    }

    // Attack animation event
    public void AttackEvent(){
        Attack();
    }

    public void TauntStartEvent(){
        // Stop the agent and play the taunt animation
        agent.isStopped = true;
    }

    public void TauntEndEvent(){
        // Stop the agent and play the taunt animation
        agent.isStopped = false;
    }


    void StartAttack(){
        // Implement attack logic here
        animator.SetTrigger("attack");
    }

    void Attack(){
        attackMesh.enabled = true;
        // Set the attack mesh to false after 0.5 seconds
        Invoke("StopAttack", 0.2f);
    }

    void StopAttack(){
        attackMesh.enabled = false;
    }
    
    void OnHurt(float damage, float knockback, Vector3 direction){
        animator.SetTrigger("hurt");
        // Implement hurt logic here
        health -= damage;
        // Apply knockback force
        rb.AddForce(direction * knockback, ForceMode.Impulse);
        // Check if the enemy is dead
        if (health <= 0) {
            Die();
        }
    }

    void Die(){
        animator.SetTrigger("die");
        animator.SetBool("isDead", true);
        // Implement death logic here
        // Disable the enemy's collider and rigidbody
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        agent.isStopped = true;
        // Destroy the enemy after 2 seconds
        Invoke("DestroyEnemy", 2f);
        if(dieEvent != null)
           dieEvent.Invoke();
        isDead = true;
    }

    void DestroyEnemy(){
        Destroy(gameObject);
        // Implement any additional cleanup here
    }

}
