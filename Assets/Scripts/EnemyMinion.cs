using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
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

    private float attackTimer = 0;
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

    public float attackCooldown = 3.0f;
    private bool isAttacking = false;

    private bool isSeen = false;

    private bool isTaunting;
    
    public UnityEvent dieEvent;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        agent.acceleration = 100;
        agent.speed = speed;
        agent.angularSpeed = 1000;
        attackMesh.enabled = false;
        initialPosition = transform.position;
        isWaitAndMove = false;
        // Randomly select a weapon from the list and enable it
        int randomIndex = UnityEngine.Random.Range(0, weapons.Count);
        weapons[randomIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        // Distance to the player
        float distance = Vector3.Distance(transform.position, player.transform.position);
        // Angle to the player
        float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        // Throw a raycast to the player
        RaycastHit hit;
        // Check if the raycast hits the player
        // Origin
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        bool isHit = Physics.Raycast(origin, player.transform.position - origin, out hit, distance);
        // Check if the player is within view radius and if the player is within view angle
        isSeen = (distance < viewRadius  && angle < viewAngle / 2 
            && isHit && hit.collider.gameObject == player) || distance < (attackRange + 1);

        // If player is seen or in combat timer, move the agent towards the player
        if(isSeen){
            inCombat = true;
            timer = 0;
        } else {
            // Timer to reset the agent's position after a certain amount of time
            timer += Time.deltaTime;
            if(timer > timeToReset){
                // Reset the agent's position to the initial position
                agent.SetDestination(initialPosition);
                agent.speed = speed / 2;
                inCombat = false;
                // Reset the timer
                timer = 0;
            }
        }


        // ------ COMBATE -----
        if(inCombat){
            // ------ MOVIENDO ------
            if(!isAttacking)
            {
                agent.speed = speed;
                // Move the agent towards the player
                agent.SetDestination(player.transform.position);
                // Check if the player is within attack range
                if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    // Stop moving the agent
                    animator.SetBool("isRunning", false);
                    agent.isStopped = true;
                    // Attack the player after a certain amount of time
                    if(attackTimer >= attackCooldown)
                    {
                        attackTimer = 0;
                        StartAttack();
                    }
                    // Rotate slowly the agent towards the player
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(player.transform.position - transform.position), Time.deltaTime * 5);
                } else {
                    // Move the agent towards the player
                    agent.isStopped = false;
                    animator.SetBool("isRunning", true);
                }
            }
            else
            {
                agent.isStopped = true;
            }
        } 
        // ------ PATRULLANDO -----
        else
        {
            // Check if the agent is at the initial position
            float distanceToInitialPosition = Vector3.Distance(transform.position, initialPosition);
            if(distanceToInitialPosition < 3f) {
                // Move the agent randomly around the initial position and radius=3
                agent.speed = speed / 2;
                // Wait for a random amount of time before moving again
                if(!isWaitAndMove)
                    StartCoroutine(WaitAndMove());

            }
        }

        if(isTaunting)
        {
            agent.isStopped = true;
        }

        // Update the attack timer
        if(attackTimer < attackCooldown)
            attackTimer += Time.deltaTime;

        // Get rbody velocity and set the animator speed parameter
        animator.SetFloat("speed", agent.velocity.magnitude);
        animator.SetBool("inCombat", inCombat);
    }

    // Coroutine to wait for a random amount of time before moving again
    // The coroutine will wait for a random amount of time between 1 and 3 seconds and then move the agent to a random position within a radius of 3 units from the initial position
    IEnumerator WaitAndMove(){
        isWaitAndMove = true;
        // Wait for a random amount of time between 1 and 3 seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        // Move the agent to a random position within a radius of 3 units from the initial position
        agent.SetDestination(initialPosition + UnityEngine.Random.insideUnitSphere * 3);
        isWaitAndMove = false;
    }

    void OnDrawGizmos()
    {
        // Draw the view radius and view angle of the enemy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        // Draw the view angle of the enemy
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRadius);
        // Draw ray to the player
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        if(player != null)
            Gizmos.DrawRay(origin, player.transform.position - origin);
        // Draw initial position and radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(initialPosition, 3);
        
    }

    // Attack animation event
    public void AttackEvent(){
        Attack();
    }

    public void TauntStartEvent(){
        // Stop the agent and play the taunt animation
        isTaunting = true;
        Invoke("TauntEnd", 0.7f);
    }

    public void TauntEnd(){
        // Stop the agent and play the taunt animation
        isTaunting = false;
    }


    void StartAttack(){
        // Implement attack logic here
        animator.SetTrigger("attack");
        isAttacking = true;
    }

    void Attack(){
        attackMesh.enabled = true;
        // Set the attack mesh to false after 0.5 seconds
        Invoke("StopAttack", 0.2f);
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

    void StopAttack(){
        attackMesh.enabled = false;
        isAttacking = false;
    }
}
