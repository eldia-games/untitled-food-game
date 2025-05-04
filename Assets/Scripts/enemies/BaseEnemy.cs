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
    public float hearingRange = 10f;

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
    public UnityEvent dieEvent;

    [Header("Configuración de pasos")]
    [Tooltip("Referencia al ScriptableObject con la lista de clips de audio.")]
    public FootstepAudioList footstepAudioList;
    public ParticleSystem footstepDustPrefab;

    [Tooltip("Cuánto volumen tendrá el sonido de paso.")]
    [Range(0f, 1f)]
    public float stepVolume = 0.5f;

    [Header("Drops")]
    public List<EnemyDrop> drop;
    [Range(0,1)] public float chanceDrop;
    
    public bool canDrop=true;

    [Header("Effects Config")]
    [Tooltip("Configures flash and shrink parameters via ScriptableObject")]
    public EnemyEffectsConfig effectsConfig;

    // Velocidad a la que la barra roja (daño) se drena hacia la salud actual
    [SerializeField]
    public float healthBarDrainSpeed = 10f;
    // Porcentaje actual de la "barra de daño" que se anima

    [Header("SFX")]
    public AudioSource attackAudioSource;
    public AudioClip attackSFX;
    public AudioClip hurtSFX;
    public AudioClip deathSFX;


    protected float redHealthPercentage;
    // Declara esta variable para guardar la salud máxima
    protected float maxHealth;

    // Estado interno
    protected bool isDead = false;


    
    protected bool isAttacking = false;
    protected bool inCombat = false;
    protected bool isSeen = false;
    protected bool isWaitAndMove = false;


    // Timers y posiciones
    protected float timer = 0f;         // Para volver a posición inicial
    protected float attackTimer = 0f;   // Para controlar cooldown de ataque
    protected Vector3 initialPosition;
    private Vector3 originalScale;
    private Coroutine scaleRoutine;
    private AudioSource _audioSource;
    public bool drawGUI = false;

    // Guardamos todos los materials instanciados
    private List<Material> flashMats = new List<Material>();

    protected virtual void Start()
    {
        // Configuración del NavMeshAgent
        agent.acceleration = 100f;
        agent.speed = speed;
        agent.angularSpeed = 1000f;

        // Guardamos la posición inicial para volver
        initialPosition = transform.position;

        // Guardamos escala inicial
        originalScale = transform.localScale;

        // 1. Recogemos todos los Renderers del prefab
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            // 2. Clonamos su array de materiales
            var mats = r.materials;  // esto instancia cada material en el array
            r.materials = mats;      // re-asignamos para que use esas instancias

            // 3. Añadimos todas esas instancias a nuestra lista
            flashMats.AddRange(mats);

            //  opcional: forzar emission keyword en cada material
            foreach (var m in mats)
                m.EnableKeyword("_EMISSION");
        }

        //drop = new List<EnemyDrop>(GetComponents<EnemyDrop>());

        maxHealth = health;
        // Inicializamos la barra roja al 100%
        redHealthPercentage = 1f;

    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    public virtual void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    protected virtual void Update()
    {
        UpdateHealthBar();

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
            drawGUI = true;
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

        isSeen = (distance < viewRadius && angle < viewAngle / 2 && isHit && hit.collider.gameObject == player)
                 || (distance < hearingRange);
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
    public virtual void StopAttack()
    {
        isAttacking = false;
    }

    public virtual void AttackEndEvent()
    {
        Debug.Log("AttackEndEvent");
        StopAttack();
    }

    public virtual void AttackStartAnimationEvent()
    {
        // Reproducir SFX de ataque
        if (attackSFX != null && attackAudioSource != null)
        {
            attackAudioSource.clip = attackSFX;
            attackAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No hay SFX de ataque asignado o AudioSource no encontrado.", this);
        }
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
    public virtual void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    public virtual void LookAt(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = lookRotation;
    }

    #region "On Hit Effects"

    private IEnumerator FlashCoroutine()
    {
        float timer   = 0f;
        float maxPow  = effectsConfig.flashMaxPower;
        float duration = effectsConfig.flashDuration;

        // Begin at full power
        foreach (var m in flashMats)
            m.SetFloat("_FlashPower", maxPow);

        // Fade out
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / duration);
            float pow = t * maxPow;

            foreach (var m in flashMats)
                m.SetFloat("_FlashPower", pow);

            yield return null;
        }

        // Ensure off
        foreach (var m in flashMats)
            m.SetFloat("_FlashPower", 0f);
    }

    
    private IEnumerator ShrinkCoroutine()
    {
        float halfDur = effectsConfig.shrinkDuration * 0.5f;
        float factor  = Mathf.Clamp(effectsConfig.shrinkFactor, 0.1f, 1f);
        Vector3 targetScale = originalScale * factor;

        float timer = 0f;

        // Fase de encoger
        while (timer < halfDur)
        {
            timer += Time.deltaTime;
            float t = timer / halfDur;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Fase de volver
        timer = 0f;
        while (timer < halfDur)
        {
            timer += Time.deltaTime;
            float t = timer / halfDur;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        scaleRoutine = null;
    }

    /// <summary>
    /// Recibe daño y knockback.
    /// </summary>
    public virtual void OnHurt(float dmg, float knockback, Vector3 direction)
    {
        if (isDead) return;

        if (flashMats.Count > 0)
            StartCoroutine(FlashCoroutine());
        else
            Debug.LogWarning("No hay materials para hacer flash. ¿Renderers encontrados?");

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ShrinkCoroutine());

        // Disparar animación "hurt" si se desea
        animator.SetTrigger("hurt");

        health -= dmg;

        // Aplica knockback
        if (rb != null)
        {
            // Calculamos la dirección del knockback
            Vector3 pla = (player.transform.position - transform.position).normalized;
            rb.AddForce(pla * knockback, ForceMode.Impulse);
        }

        // Reproducir SFX de daño
        if (hurtSFX != null && _audioSource != null)
        {
            _audioSource.clip = hurtSFX;
            _audioSource.Play();
        } else 
        {
            Debug.LogWarning("No hay SFX de daño asignado o AudioSource no encontrado.", this);
        }

        // Comprobamos muerte
        if (health <= 0f)
        {
            Die();
        }
    }

    #endregion

    #region "Death"

    /// <summary>
    /// Maneja la muerte del enemigo (animación, collider, rigidbody, etc.)
    /// </summary>
    public virtual void Die()
    {
        // Si ya está muerto, no hacemos nada
        if (isDead) return;

        isDead = true;

        animator.SetTrigger("die");
        animator.SetBool("isDead", true);

        StartCoroutine(DeathDissolve());

        // Deshabilitar collider y rigidbody
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        if (rb) rb.isKinematic = true;

        agent.isStopped = true;

        // Llamar evento (si existe)
        if (dieEvent != null)
            dieEvent.Invoke();
        if (drop !=null && canDrop)
        {
            for(int i = 0; i < drop.Count; i++)
            {
                EnemyDrop enemyDrop = drop[i];
                float rand= Random.value;
                if(rand < enemyDrop.chanceDrop)
                {

                    GameObject objectCreated = Instantiate(enemyDrop.drop, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                        
                    ObjectDrop objectdrop = objectCreated.GetComponent<ObjectDrop>();
                    objectdrop.quantity = Random.Range(enemyDrop.minDrop, enemyDrop.maxDrop + 1);
                }
            }
        }
        // Destruir el enemigo tras unos segundos
        Invoke(nameof(DestroyEnemy), 2f);
    }

    IEnumerator DeathDissolve() {
        float timer = 0f;
        float duration = 1f; // Duración del disolverse

        yield return new WaitForSeconds(1f); // Esperar un poco antes de empezar a disolverse

        
        while (timer < duration) {
            timer += Time.deltaTime;
            foreach (var m in flashMats)
                m.SetFloat("_DissolveThreshold", timer / duration);
            yield return null;
        }

        foreach (var m in flashMats)
            m.SetFloat("_DissolveThreshold", 1f);
    }

    public virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    #endregion

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

    #region "Health Bar"

    public void UpdateHealthBar()
    {
        float currentPct = Mathf.Clamp01(health / maxHealth);
        if (redHealthPercentage > currentPct)
        {
            redHealthPercentage = Mathf.MoveTowards(
                redHealthPercentage,
                currentPct,
                healthBarDrainSpeed * Time.deltaTime
            );
        }
        else
        {
            redHealthPercentage = currentPct;
        }
    }

    #endregion

    #region "Footsteps"
    public void PlayFootstep()
    {
        if (footstepAudioList == null || footstepAudioList.footstepClips.Count == 0)
        {
            Debug.LogWarning("No hay clips de paso asignados en FootstepAudioList.", this);
            return;
        }

        // Elegimos un clip aleatorio
        int index = Random.Range(0, footstepAudioList.footstepClips.Count);
        AudioClip clip = footstepAudioList.footstepClips[index];

        _audioSource.clip = clip;
        _audioSource.volume = stepVolume;
        _audioSource.Play();
    }

    public virtual void FootstepAnimationEvent()
    {
        PlayFootstep();
        footstepDustPrefab?.Play();
    }

    #endregion


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

    #region "Debug"

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

    public virtual void OnGUI()
    {
        if (Camera.main == null) return;
        if (player && player.GetComponent<PlayerCombat>().isDead) return;
        if (!drawGUI) return;

        // Posición 2 unidades sobre el enemigo
        Vector3 worldPos = transform.position + Vector3.up * 2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.y = Screen.height - screenPos.y;
        

        float barWidth = 50f;
        float barHeight = 5f;

        // Porcentajes
        float currentPct = Mathf.Clamp01(health / maxHealth);
        float redPct = redHealthPercentage;

        // Rectángulos
        Rect bgRect = new Rect(
            screenPos.x - barWidth / 2f,
            screenPos.y,
            barWidth,
            barHeight
        );
        Rect redRect = new Rect(
            bgRect.x,
            bgRect.y,
            barWidth * redPct,
            barHeight
        );
        Rect greenRect = new Rect(
            bgRect.x,
            bgRect.y,
            barWidth * currentPct,
            barHeight
        );

        // Fondo negro
        GUI.color = Color.black;
        GUI.DrawTexture(bgRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        // Barra roja
        GUI.color = Color.red;
        GUI.DrawTexture(redRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        // Barra verde (salud actual)
        GUI.color = new Color(0.2f, 0.75f, 0.2f, 1f); // Verde 
        GUI.DrawTexture(greenRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        // Restaurar
        GUI.color = Color.white;
    }
    
    #endregion
}
