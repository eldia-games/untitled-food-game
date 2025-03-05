using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMage : MonoBehaviour
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
    public float fleeDistance = 7f; // Distance at which the enemy starts fleeing
    public float fleeRadius = 1f; // Maximum distance enemy can flee
    public int rayCount = 12; // How many directions to test
    public float checkRadius = 1f; // Distance to check for valid NavMesh positions
    private Vector3 bestFleePoint;
    private List<Vector3> debugPositions = new List<Vector3>(); // Store potential positions for Gizmos

    public Animator animator;

    public GameObject spellPrefab; // Assign spell prefab in inspector
    public GameObject spellManualPrefab; // Assign spell prefab in inspector
    public Transform staffTip; // Assign the tip of the staff in inspector

    // Parametros de vista
    public float viewRadius;
    public float viewAngle;

    private float timer = 0;
    private Vector3 initialPosition;
    private bool inCombat = false;

    public float timeToReset;
    private bool isWaitAndMove;

    public float attackCooldown;
    private float attackTimer = 0;
    private bool isAttacking = false;

    // List of possibles weapons
    public List<GameObject> weapons;

    private bool isSeen = false;

    public UnityEvent dieEvent;
    private bool isDead = false;
    // Random number from 0 to 2
    private int randomNumber = UnityEngine.Random.Range(0, 3);

    // Start is called before the first frame update
    void Start()
    {
        agent.acceleration = 100;
        agent.speed = speed;
        agent.angularSpeed = 1000;
        initialPosition = transform.position;
        isWaitAndMove = false;
        // Randomly select a weapon from the list and enable it
        int randomIndex = UnityEngine.Random.Range(0, weapons.Count);
        weapons[randomIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return;
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
        if(inCombat)
        {
            // ------ MOVIENDO ------
            if(!isAttacking)
            {
                agent.speed = speed;
                // Move the agent towards the player
                agent.SetDestination(player.transform.position);
                // Check if the player is within attack range
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if(distanceToPlayer < attackRange && distanceToPlayer > fleeRadius)
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
                    // Look at the player
                    // Rotate slowly the agent towards the player
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(player.transform.position - transform.position), Time.deltaTime * 5);
                    
                } else if (distanceToPlayer <= fleeRadius)
                {
                    // Move the agent from the player
                    agent.isStopped = false;
                    animator.SetBool("isRunning", true);
                    Vector3 bestFleeDirection = GetBestFleeDirection();
                    if (bestFleeDirection != Vector3.zero)
                    {
                        agent.SetDestination(bestFleeDirection);
                    }
                }
                else {
                    // Move the agent towards the player
                    agent.isStopped = false;
                    animator.SetBool("isRunning", true);
                }
            } 
            // ------ ATACANDO ------
            else 
            {
                // If the agent is attacking, reset the timer
                attackTimer = 0;
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

    Vector3 GetBestFleeDirection()
    {
        debugPositions.Clear(); // Clear previous positions for Gizmos
        Vector3 bestDirection = Vector3.zero;
        float maxDistance = 0f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 potentialPosition = transform.position + direction * fleeRadius;

            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, checkRadius, NavMesh.AllAreas))
            {
                debugPositions.Add(hit.position); // Store for Gizmos
                float distanceFromPlayer = Vector3.Distance(hit.position, player.transform.position);
                
                if (distanceFromPlayer > maxDistance)
                {
                    maxDistance = distanceFromPlayer;
                    bestDirection = hit.position;
                }
            }
        }

        return bestDirection;
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

        if (player == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 rayEnd = transform.position + direction * fleeRadius;
            Gizmos.DrawLine(transform.position, rayEnd); // Draw potential flee directions
        }

        Gizmos.color = Color.green;
        foreach (var pos in debugPositions)
        {
            Gizmos.DrawSphere(pos, 0.3f); // Draw valid flee positions
        }

        if (bestFleePoint != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, bestFleePoint); // Draw the chosen flee path
            Gizmos.DrawSphere(bestFleePoint, 0.5f);
        }
        
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
        randomNumber = UnityEngine.Random.Range(0, 2);
        // Implement attack logic here
        animator.SetInteger("attackType", randomNumber);
        animator.SetTrigger("attack");
        isAttacking = true;
    }

    void Attack(){
        switch(randomNumber){
            case 0:
                if (spellPrefab != null && player != null)
                {
                    GameObject spell = Instantiate(spellPrefab, staffTip.position, Quaternion.identity);
                    SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();

                    if (spellScript != null)
                    {
                        spellScript.SetTarget(player.transform);
                    }
                }
                // Set the attack mesh to false after 0.5 seconds
                Invoke("StopAttack", 0.2f);
               break;
            case 1:
                // Pause the animator
                animator.speed = 0f;
                agent.speed = 0f;
                StartCoroutine(SpiralAttack(spellManualPrefab, staffTip.position, 2f, 1f, 80));
                Invoke("StopAttack", 3f);
               break;
            case 2:
                // Cone attack logic here
                Invoke("StopAttack", 0.2f);
                break;
            default: break;
        }
    }

    public static IEnumerator SpiralAttack(GameObject spellPrefab, 
        Vector3 origin, float duration, float spiralSpeed, int projectilesPerSecond)
    {
        float elapsedTime = 0f;
        float angle = 0f;
        float interval = 1f / projectilesPerSecond;

        while (elapsedTime < duration)
        {
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            GameObject spell = Instantiate(spellPrefab, origin - Vector3.up * 0.5f, Quaternion.identity);
            SpellProjectileManual spellScript = spell.GetComponent<SpellProjectileManual>();
            if (spellScript != null)
            {
                spellScript.SetDirection(direction);
            }
            
            angle += Mathf.PI * 2 * (spiralSpeed / projectilesPerSecond);
            elapsedTime += interval;
            yield return new WaitForSeconds(interval);
        }
        
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
        timer = 0f;
        animator.speed = 1f;
        isAttacking = false;
    }
}
