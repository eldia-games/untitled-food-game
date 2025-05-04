using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class Boss : MonoBehaviour
{
    public bool debug = false;
    [Header("Stats generales")]
    public float speed = 4f;
    public float sprintSpeed = 8f;
    public float health = 100f;
    public float damage = 10f;

    [Header("Rango y visión")]
    public float attackRange;
    public float viewRadius = 20f;
    public float viewAngle = 90f;
    protected bool isSeen = false;
    

    [Header("Otros parámetros")]
    public float timeToReset = 5f;
    public float attackCooldown = 1f;

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
    
    private bool canUseAbility = true;

    [Header("Habilidades")]
    public GameObject mouth;
    public Collider mouthAttackCollider;
    // Mesh of collider 
    public MeshRenderer mouthColliderMeshRenderer;
    private Quaternion lastRotation;

    [Header("Rigging")]
    public Rig rig;
    public RigBuilder rigBuilder;
    public MultiAimConstraint aimConstraint;

    [Header("Effects Config")]
    [Tooltip("Configures flash and shrink parameters via ScriptableObject")]
    public EnemyEffectsConfig effectsConfig;
    public float healthBarDrainSpeed = 10f;

    [Header("SFX")]
    public AudioSource attackAudioSource;
    public AudioClip attackSFX;
    public AudioClip hurtSFX;
    public AudioClip deathSFX;

        
    protected float redHealthPercentage;
    // Declara esta variable para guardar la salud máxima
    protected float maxHealth;
    public bool drawGUI = false;
    

    // Estado interno
    protected bool isDead = false;
    protected bool onGround = true;
    
    protected EnemyState currentState = EnemyState.Spawn;
    private List<Material> flashMats = new List<Material>();
    private Vector3 originalScale;
    private AudioSource _audioSource;

    private Coroutine scaleRoutine;
    private float chaseStartTime;


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

        if (player != null)
        {
            Debug.Log("Player reference set in Boss script.");
            aimConstraint.data.sourceObjects.Clear();
            aimConstraint.data.sourceObjects.Add(new WeightedTransform(player.transform, 1f));
            rigBuilder.Build();
        }

        // Guardamos escala inicial
        originalScale = transform.localScale;

        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            var mats = r.materials;  // esto instancia cada material en el array
            r.materials = mats;      // re-asignamos para que use esas instancias

            flashMats.AddRange(mats);

            foreach (var m in mats)
                m.EnableKeyword("_EMISSION");
        }

    }

    public void Awake()
    {
        UIManager.Instance.ShowBossHealth();
        UIManager.Instance.SetBossHealth(health);
        UIManager.Instance.SetMaxBossHealth(health);
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return health;
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
                for (int i = 0; i < skills.Length; i++)
                {
                    skills[i].Initialize();
                }
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

                if (!canUseAbility) break;

                if (!currentSkill)
                {
                    List<BossSkillScriptableObject> availableSkills = new List<BossSkillScriptableObject>();
                    for (int i = 0; i < skills.Length; i++)
                    {
                        if (skills[i].CanUse(this, player))
                        {
                            availableSkills.Add(skills[i]);
                        }
                    }
                    if (availableSkills.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availableSkills.Count);
                        currentSkill = availableSkills[randomIndex];
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
                        chaseStartTime   = Time.time;  // arrancamos temporizador
                    }
                break;

            case EnemyState.Chase:
                // Si excede el tiempo máximo sin llegar al rango, reiniciamos
                if (currentSkill != null && Time.time - chaseStartTime > timeToReset)
                {
                    currentSkill = null;
                    currentState = EnemyState.Idle;
                    agent.speed  = speed;
                    StopMovement();
                    break;
                }

                // Persecución normal
                AllowMovement();
                agent.SetDestination(player.transform.position);

                // Al entrar en rango, pasamos a usar la habilidad
                if (currentSkill != null && currentSkill.InMinRange(this, player))
                {
                    agent.speed  = speed;
                    currentState = EnemyState.UsingAbility;
                }
                break;

            case EnemyState.UsingAbility:
                currentSkill.HandleMovement(this, player);

                // Si la habilidad ya se ha lanzado, se vuelve a Idle
                if (!currentSkill.isCasting)
                {
                    // Inicia el cooldown y vuelve a Idle
                    StartCoroutine(AbilityCooldown());
                    agent.speed = speed;
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
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);    
    }

    private IEnumerator AbilityCooldown()
    {
        canUseAbility = false;
        yield return new WaitForSeconds(attackCooldown);
        canUseAbility = true;
    }

    public virtual void ActivateMouthCollider()
    {
        if (mouthAttackCollider != null)
        {
            mouthAttackCollider.enabled = true;
            if(debug)
                mouthColliderMeshRenderer.enabled = true;
        }
    }

    public virtual void DeactivateMouthCollider()
    {
        if (mouthAttackCollider != null)
        {
            mouthAttackCollider.enabled = false;
            if(debug)
                mouthColliderMeshRenderer.enabled = false;
        }
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
                 || (distance < (attackRange + 1f));
    }

    /// <summary>
    /// Evento que llama la animación de ataque cuando impacta
    /// </summary>
    public virtual void OnAnimationEvent()
    {
        if(currentSkill != null)
        {
            currentSkill.OnAnimationEvent(this, player);
        }
    }

    public virtual void OnAttackStartAnimationEvent() {
        OnAnimationEvent();
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

        health -= dmg;

        UIManager.Instance.SetBossHealth(health);

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

    /// <summary>
    /// Maneja la muerte del enemigo (animación, collider, rigidbody, etc.)
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("die");
        animator.SetBool("isDead", true);

        StartCoroutine(DeathDissolve());

        // Deshabilitar collider y rigidbody
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        if (rb) rb.isKinematic = true;

        // Estado pasa a muerto
        currentState = EnemyState.Dead;

        StopMovement();
        UIManager.Instance.HideBossHealth();

        // Llamar evento (si existe)
        if (dieEvent != null)
            dieEvent.Invoke();

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
    
    public void OnGUI()
    {
        Vector3 worldPos = transform.position + Vector3.up * 2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (debug)
        {
            // Ajustar el eje Y para que el origen sea la parte superior de la pantalla
            screenPos.y = Screen.height - screenPos.y;
            
            // Definir offsets para mostrar la información al lado del enemigo
            float offsetX = 0f;  // Desplazamiento horizontal (ajusta según necesites)
            float offsetY = 0f; // Desplazamiento vertical (ajusta según necesites)

            // Crear un rectángulo para la etiqueta del estado
            Rect stateRect = new Rect(screenPos.x + offsetX, screenPos.y + offsetY, 300, 20);
            GUI.Label(stateRect, "Estado del enemigo: " + currentState.ToString(),
                new GUIStyle() { normal = new GUIStyleState() { textColor = Color.blue } });
            
            // Crear un rectángulo para la etiqueta de la habilidad actual, justo debajo
            Rect skillRect = new Rect(screenPos.x + offsetX, screenPos.y + offsetY + 10, 300, 20);
            if (currentSkill)
            {
                GUI.Label(skillRect, "Habilidad actual: " + currentSkill.name,
                    new GUIStyle() { normal = new GUIStyleState() { textColor = Color.green } });
            }
            else
            {
                GUI.Label(skillRect, "Habilidad actual: Ninguna",
                    new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } });
            }
            // Mostrar nombre de animacion actual en layer 0
            Rect animRect = new Rect(screenPos.x + offsetX, screenPos.y + offsetY + 20, 300, 20);
            if (animator != null)
            {
                string currentAnim = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") 
                ? "Idle" : animator.GetCurrentAnimatorStateInfo(0).ToString();
                GUI.Label(animRect, "Animación actual: " + currentAnim,
                    new GUIStyle() { normal = new GUIStyleState() { textColor = Color.yellow } });
            }

            if (Camera.main == null) return;
            if (player && player.GetComponent<PlayerCombat>().isDead) return;
            if (!drawGUI) return;

                
            GUI.depth = 0;  // Valor bajo para la barra de vida

            // Posición 2 unidades sobre el enemigo
            screenPos.y = Screen.height - screenPos.y;
            
            float barWidth = Screen.width * 0.1f;  // 10% del ancho de la pantalla
            float barHeight = Screen.height * 0.01f;  // 1% de la altura de la pantalla

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
    }
}
