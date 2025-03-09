using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Stats generales")]
    public float speed = 4f;
    public float health = 100f;
    public float damage = 10f;

    [Header("Rango y visión")]
    public float attackRange;
    public float viewRadius = 20f;
    public float viewAngle = 90f;

    [Header("Otros parámetros")]
    public float timeToReset = 5f;
    public float attackCooldown = 3f;

    [Header("Referencias")]
    public GameObject player;
    public Rigidbody rb;
    public NavMeshAgent agent;
    public Animator animator;
    public List<GameObject> weapons;
    public UnityEvent dieEvent;

    [Header("Sonidos")]

    public AudioSource audioSource;

    public List<AudioClip> footstepWalkSounds;
    public List<AudioClip> footstepRunSounds;

    // Estado interno
    protected bool isDead = false;
    
    public bool isAttacking = false;
    protected bool inCombat = false;
    protected bool isSeen = false;
    protected bool isWaitAndMove = false;

    // Timers y posiciones
    protected float timer = 0f;         // Para volver a posición inicial
    protected float attackTimer = 0f;   // Para controlar cooldown de ataque
    protected Vector3 initialPosition;

    protected virtual void Start()
    {
        // Configuración del NavMeshAgent
        agent.acceleration = 100f;
        agent.speed = speed;
        agent.angularSpeed = 1000f;

        // Guardamos la posición inicial para volver
        initialPosition = transform.position;

        // Selección aleatoria de arma (si hay)
        if (weapons != null && weapons.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, weapons.Count);
            weapons[randomIndex].SetActive(true);
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    protected virtual void Update()
    {

        if (isDead) return;
        if (player == null) return; // Por seguridad

        // 1. Comprobar si vemos al jugador
        CheckPlayerVisibility();

        // 2. Controlar estado de combate o patrulla
        if (isSeen)
        {
            // Si lo hemos visto, establecemos combate
            inCombat = true;
            timer = 0f;
        }
        else
        {
            // Si NO lo vemos, contamos tiempo para volver a la posición original
            timer += Time.deltaTime;
            if (timer > timeToReset)
            {
                ReturnToInitialPosition();
                inCombat = false;
            }
        }

        // 3. Lógica de combate o patrullaje
        if (inCombat)
        {
            HandleCombat();
        }
        else
        {
            HandlePatrol();
        }

        // 4. Actualizar timers y animaciones
        UpdateTimers();
        UpdateAnimations();

        // No rotacion en el plano XZ
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del rango de visión y sin obstáculos
    /// </summary>
    protected virtual void CheckPlayerVisibility()
    {
        if(player == null) return; // Por seguridad
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        float distance = Vector3.Distance(origin, player.transform.position);
        float angle = Vector3.Angle(transform.forward, player.transform.position - origin);

        // Hacemos un raycast para verificar que no hay obstáculos
        RaycastHit hit;
        bool isHit = Physics.Raycast(origin, player.transform.position - origin, out hit, distance);

        // Se ve si: 
        //   a) está dentro del radio de visión, 
        //   b) el ángulo es menor a viewAngle/2, 
        //   c) el raycast golpea al jugador, 
        //   O si está muy cerca del rango de ataque
        isSeen = (distance < viewRadius && angle < viewAngle / 2 && isHit && hit.collider.gameObject == player)
                 || (distance < (attackRange + 1f));
    }

    /// <summary>
    /// Lógica de combate: moverse hacia el jugador, atacar, etc.
    /// </summary>
    protected virtual void HandleCombat()
    {
        // Atacando, no se mueve
        if(isAttacking){
            return;
        }

        // Si el jugado esta fuera del rango de ataque se mueve hacia él
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceToPlayer > attackRange)
        {
            // Si no estamos atacando, nos movemos
            agent.speed = speed;
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            animator.SetBool("isRunning", true);
            return;
        }

        // Si el jugador está en rango de ataque, paramos y atacamos
        if (distanceToPlayer < attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);

            // Rotamos para mirar al jugador
            RotateTowards(player.transform.position);

            // Comprobamos cooldown de ataque
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                StartAttack();
            }
        }
    }

    /// <summary>
    /// Lógica de patrullaje: cuando no estamos en combate, volvemos a la posición inicial o deambulamos
    /// </summary>
    protected virtual void HandlePatrol()
    {
        float distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        // Si ya casi estamos en posición inicial, podemos deambular
        if (distanceToInitial < 3f)
        {
            agent.speed = speed / 2;
            if (!isWaitAndMove)
            {
                StartCoroutine(WaitAndMove());
            }
        }
    }

    /// <summary>
    /// Patrullar aleatoriamente alrededor de la posición inicial.
    /// </summary>
    protected IEnumerator WaitAndMove()
    {
        isWaitAndMove = true;
        yield return new WaitForSeconds(Random.Range(1, 6));
        // Mover a una posición aleatoria dentro de un radio
        agent.SetDestination(initialPosition + Random.insideUnitSphere * 4f);
        isWaitAndMove = false;
    }

    /// <summary>
    /// Manda de vuelta al enemigo a su posición inicial y reduce velocidad
    /// </summary>
    protected virtual void ReturnToInitialPosition()
    {
        agent.SetDestination(initialPosition);
        agent.speed = speed / 2;
        timer = 0f;
    }

    /// <summary>
    /// Prepara el inicio del ataque (por ej. dispara un trigger de Animator)
    /// </summary>
    protected virtual void StartAttack()
    {
        animator.SetTrigger("attack");
        // Para que no se atasque en la animación de ataque
        Invoke("StopAttack", 1f);
        isAttacking = true;
    }

    /// <summary>
    /// Evento que llama la animación de ataque cuando impacta (por ej. AttackEvent).
    /// Se sobreescribe en clases hijas para distintos comportamientos.
    /// </summary>
    public virtual void AttackEvent()
    {
        // En la clase base, no hacemos nada. 
        // Las clases derivadas implementan la lógica específica (melee, proyectil, etc.)
    }

    /// <summary>
    /// Dejar de atacar tras la animación
    /// </summary>
    protected virtual void StopAttack()
    {
        isAttacking = false;
    }

    public virtual void AttackEndEvent()
    {
        Debug.Log("AttackEndEvent");
        StopAttack();
    }

    /// <summary>
    /// Actualiza el attackTimer y otros contadores que hagan falta.
    /// </summary>
    protected virtual void UpdateTimers()
    {
        if (attackTimer < attackCooldown)
            attackTimer += Time.deltaTime;
    }

    /// <summary>
    /// Sincroniza parámetros del Animator (velocidad, inCombat, etc.)
    /// </summary>
    protected virtual void UpdateAnimations()
    {
        animator.SetFloat("speed", agent.velocity.magnitude);
        animator.SetBool("inCombat", inCombat);
    }

    /// <summary>
    /// Rota el GameObject suavemente hacia un punto.
    /// </summary>
    protected virtual void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Recibe daño y knockback.
    /// </summary>
    protected virtual void OnHurt(float dmg, float knockback, Vector3 direction)
    {
        // Disparar animación "hurt" si se desea
        animator.SetTrigger("hurt");

        health -= dmg;

        // Aplica knockback
        if (rb != null)
        {
            rb.AddForce(direction * knockback, ForceMode.Impulse);
        }

        // Comprobamos muerte
        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Maneja la muerte del enemigo (animación, collider, rigidbody, etc.)
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("die");
        animator.SetBool("isDead", true);

        // Deshabilitar collider y rigidbody
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        if (rb) rb.isKinematic = true;

        agent.isStopped = true;

        // Llamar evento (si existe)
        if (dieEvent != null)
            dieEvent.Invoke();

        // Destruir el enemigo tras unos segundos
        Invoke(nameof(DestroyEnemy), 2f);
    }

    protected virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Llamado desde la animación (por ejemplo, TauntStartEvent en Animator).
    /// </summary>
    public virtual void TauntStartEvent()
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
    }

    /// <summary>
    /// Llamado desde la animación (TauntEndEvent).
    /// </summary>
    public virtual void TauntEndEvent()
    {
        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }

    public virtual void FootstepEvent()
    {
        // Convertir la lista de sonidos de paso a un array
        AudioClip[] footstepSoundsArray;
        if (!inCombat)
            footstepSoundsArray = footstepWalkSounds.ToArray();
        else
            footstepSoundsArray = footstepRunSounds.ToArray();

        // Obtener un índice aleatorio dentro del rango del array
        int randomIndex = Random.Range(0, footstepSoundsArray.Length);

        // Coger un sonido de paso aleatorio y reproducirlo
        AudioClip footstepSound = footstepSoundsArray[randomIndex];
        // Pitch aleatorio para mayor variedad
        //audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(footstepSound);
    }

    /// <summary>
    /// Predice la posición futura del jugador según su velocidad (para disparos a distancia).
    /// </summary>
    protected virtual Vector3 PredictFuturePosition(float projectileSpeed)
    {
        if (player == null) return transform.position;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
            return player.transform.position;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        float timeToImpact = distance / projectileSpeed;

        Vector3 futurePos = player.transform.position + playerRb.velocity * timeToImpact;
        // Ajuste de altura si fuese necesario
        futurePos.y = player.transform.position.y;
        return futurePos;
    }

    // Puedes sobrescribir OnDrawGizmos si quieres debug de visión, etc.
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * viewRadius;
        Quaternion leftRayRotation = Quaternion.Euler(0f, -viewAngle / 2f, 0f);
        Quaternion rightRayRotation = Quaternion.Euler(0f, viewAngle / 2f, 0f);
        Gizmos.DrawLine(transform.position, transform.position + leftRayRotation * forward);
        Gizmos.DrawLine(transform.position, transform.position + rightRayRotation * forward);
    }
    
}
