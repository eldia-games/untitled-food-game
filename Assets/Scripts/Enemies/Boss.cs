using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Boss : MonoBehaviour
{
    [Header("Stats generales")]
    public float speed = 4f;
    public float health = 100f;
    public float damage = 10f;

    [Header("Rango y visión")]
    public float attackRange;
    public float viewRadius = 20f;
    public float viewAngle = 90f;
    protected bool isSeen = false;

    [Header("Otros parámetros")]
    public float timeToReset = 5f;
    public float attackCooldown = 3f;

    [Header("Referencias")]
    public GameObject player;
    public Rigidbody rb;
    public NavMeshAgent agent;
    public Animator animator;
    public UnityEvent dieEvent;

    [Header("Sonidos")]

    public AudioSource audioSource;

    public List<AudioClip> footstepWalkSounds;
    public List<AudioClip> footstepRunSounds;

    [Header("Habilidades")]
    public BossSkillScriptableObject[] skills;
    protected BossSkillScriptableObject currentSkill;

    [Header("Habilidades")]
    public GameObject mouth;
    public Collider attackCollider;
    private Quaternion lastRotation;

    // Estado interno
    protected bool isDead = false;
    protected bool onGround = true;
    
    protected EnemyState currentState = EnemyState.Spawn;

    protected virtual void Start()
    {
        // Inicializa la última rotación con la rotación actual
        lastRotation = transform.rotation;
        // Configuración del NavMeshAgent
        agent.acceleration = 100f;
        agent.speed = speed;
        agent.angularSpeed = 1000f;

        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Initialize();
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

        switch(currentState)
        {
            case EnemyState.Spawn:
                // Lógica de spawn
                // Espera a que el jugador esté cerca, grita y se pone en combate
                StopMovement();
                if (Vector3.Distance(transform.position, player.transform.position) < viewRadius)
                {
                    currentState = EnemyState.Taunt;
                    animator.SetTrigger("scream");
                }
                break;
            
            case EnemyState.Taunt:
                // Lógica de taunt
                // Espera a que el taunt termine y se ponga en combate
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"))
                {
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Idle:
                // Comportamiento de espera
                // Si no se está ejecutando una habilidad se intenta lanzar alguna
                StopMovement();
                SlowlyRotateTowards(player.transform.position);

                if(!currentSkill)
                    for (int i = 0; i < skills.Length; i++)
                    {
                        if (skills[i].CanUse(this, player))
                        {
                            currentSkill = skills[i];
                        }
                    }
                
                if(currentSkill)
                    if(currentSkill.InMinRange(this, player))
                    {
                        if(!currentSkill.isCasting)
                            currentSkill.Use(this, player);
                        currentState = EnemyState.UsingAbility;
                    }
                    else
                    {
                    // Reposicionarse si el jugador está lejos
                        currentState = EnemyState.Chase;
                    }
                break;

            case EnemyState.Chase:
                // Comportamiento de persecución
                AllowMovement();
                agent.SetDestination(player.transform.position);

                if(currentSkill.InMinRange(this, player))
                {
                    if(!currentSkill.isCasting)
                        currentSkill.Use(this, player);
                    currentState = EnemyState.UsingAbility;
                    break;
                }
                break;

            case EnemyState.UsingAbility:
                StopMovement();
                if(!currentSkill.isCasting)
                {
                    currentSkill = null;
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Dead:
                StopMovement();
                break;
        }

        UpdateAnimations();


        // Forzamos que el jefe solo rote en Y (para evitar inclinaciones)
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);    }

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

        isSeen = (distance < viewRadius && angle < viewAngle / 2 && isHit && hit.collider.gameObject == player)
                 || (distance < (attackRange + 1f));
    }

    /// <summary>
    /// Lógica de combate: moverse hacia el jugador, atacar, etc.
    /// </summary>
    protected virtual void HandleCombat()
    {

    }

    /// <summary>
    /// Evento que llama la animación de ataque cuando impacta
    /// Se sobreescribe en clases hijas para distintos comportamientos.
    /// </summary>
    public virtual void OnAttackStartAnimationEvent()
    {
        if(currentSkill != null)
        {
            currentSkill.OnAnimationEvent(this, player);
        }
    }

    public virtual void OnAttackEndAnimationEvent()
    {
    }

    /// <summary>
    /// Sincroniza parámetros del Animator (velocidad, inCombat, etc.)
    /// </summary>
    protected virtual void UpdateAnimations()
    {
        // Cálculo de la velocidad angular (grados/segundo)
        float angularSpeed = Quaternion.Angle(lastRotation, transform.rotation) / Time.deltaTime;
        lastRotation = transform.rotation;

        animator.SetFloat("angularSpeed", angularSpeed);
        animator.SetFloat("speed", agent.velocity.magnitude);
        animator.SetBool("onGround", onGround);
    }

    /// <summary>
    /// Rota el GameObject suavemente hacia un punto.
    /// </summary>
    public virtual void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    public virtual void SlowlyRotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
    }

    /// <summary>
    /// Recibe daño y knockback.
    /// </summary>
    public virtual void OnHurt(float dmg, float knockback, Vector3 direction)
    {
        // Disparar animación "hurt" si se desea
        animator.SetTrigger("hurt");

        health -= dmg;

        // Aplica knockback
        if (rb != null)
        {
            //rb.AddForce(direction * knockback, ForceMode.Impulse);
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

        // Estado pasa a muerto
        currentState = EnemyState.Dead;

        StopMovement();

        // Llamar evento (si existe)
        if (dieEvent != null)
            dieEvent.Invoke();

        // Destruir el enemigo tras unos segundos
        Invoke(nameof(DestroyEnemy), 2f);
    }

    public virtual void DestroyEnemy()
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

    public void StopMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;
    }

    public void AllowMovement(){
        if( agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }

    public virtual void FootstepEvent()
    {
        // Convertir la lista de sonidos de paso a un array
        AudioClip[] footstepSoundsArray;
        if (speed < 3f)
            footstepSoundsArray = footstepWalkSounds.ToArray();
        else
            footstepSoundsArray = footstepRunSounds.ToArray();

        // Obtener un índice aleatorio dentro del rango del array
        int randomIndex = Random.Range(0, footstepSoundsArray.Length);

        // Coger un sonido de paso aleatorio y reproducirlo
        AudioClip footstepSound = footstepSoundsArray[randomIndex];
        audioSource.PlayOneShot(footstepSound);
    }

    /// <summary>
    /// Predice la posición futura del jugador según su velocidad (para disparos a distancia).
    /// </summary>
    public virtual Vector3 PredictFuturePosition(float projectileSpeed)
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
