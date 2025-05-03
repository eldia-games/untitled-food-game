using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class MageV2 : BaseEnemyV2
{
    [Header("Sonidos")]
    public List<AudioClip> spellSounds;

    [Header("Flee Params")]
    public float fleeDistance = 7f;
    public float approachDistance = 10f;
    public float fleeRadius = 1f;
    public int rayCount = 12;
    public float checkRadius = 1f;
    private float orbitalTimer = 0f;
    public float orbitalChangeInterval = 3f; // Cambia la dirección cada 3 segundos (ajustable)
    private float orbitalSign = 1f; // 1 para girar a la derecha, -1 para girar a la izquierda

    public Transform staffTip;

    public SkillScriptableObject[] skills;
    private SkillScriptableObject currentSkill;
    protected EnemyState currentState = EnemyState.Spawn;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Initialize();
        }

        // Si se ha introducido un objeto de destino, lo asignamos al rig
        if (player != null)
        {
            MultiAimConstraint mac = GetComponent<MultiAimConstraint>();
            mac.data.sourceObjects.Clear();
            mac.data.sourceObjects.Add(new WeightedTransform(player.transform, 1f));
            RigBuilder rigs = GetComponent<RigBuilder>();
            rigs.Build();
        }
    }

    protected override void Update()
    {
        
        if (isDead) return;
        if (player == null) return; // Por seguridad

        CheckPlayerVisibility();

        switch(currentState)
        {
            case EnemyState.Spawn:
                // Lógica de spawn
                // Espera a que el jugador esté cerca, grita y se pone en combate
                StopMovement();
                currentState = EnemyState.Idle;
                break;
            
            case EnemyState.Taunt:
                // Lógica de taunt
                break;

            case EnemyState.Idle:
                AllowMovement();
                // Lógica de idle
                HandlePatrol();
                if(isSeen)
                {
                    currentState = EnemyState.Combat;
                    drawGUI = true;
                    inCombat = true;
                }
                break;

            case EnemyState.Combat:
                StopMovement();
                bool canUseButNotInRange = false;
                // Lógica de combate
                if (!currentSkill)
                {
                    for (int i = 0; i < skills.Length; i++)
                    {
                        if (skills[i].CanUse(this, player))
                        {
                            if (!skills[i].InRange(this, player))
                            {
                                canUseButNotInRange = true;
                            }
                            else
                            {
                                currentSkill = skills[i];
                                skills[i].Use(this, player);
                                currentState = EnemyState.UsingAbility;
                                break;
                            }
                        }
                    }
                }
                
                if(canUseButNotInRange && !currentSkill)
                {
                    currentState = EnemyState.Chase;
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    // Si no hay habilidades se huye si el jugador está cerca
                    AllowMovement();
                    MantainDistance();
                }
                break;

            case EnemyState.Chase:
                // Comportamiento de persecución
                AllowMovement();
                agent.SetDestination(player.transform.position);

                if(!currentSkill)
                    for (int i = 0; i < skills.Length; i++)
                    {
                        if (skills[i].CanUse(this, player) && skills[i].InRange(this, player))
                        {
                            skills[i].Use(this, player);
                            currentSkill = skills[i];
                            currentState = EnemyState.UsingAbility;
                            break;
                        }
                    }
                break;

            case EnemyState.UsingAbility:
                StopMovement();
                if(!currentSkill || !currentSkill.isCasting)
                {
                    currentSkill = null;
                    currentState = EnemyState.Combat;
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

    public virtual void OnAnimationEvent()
    {
        if(currentSkill != null)
        {
            currentSkill.OnAnimationEvent(this, player);
        }
    }

    protected void MantainDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Si el jugador está demasiado cerca, huir.
        if (distanceToPlayer <= fleeRadius)
        {
            Vector3 bestFlee = GetBestFleeDirection();
            if (bestFlee != Vector3.zero)
                agent.SetDestination(bestFlee);
        }
        // Si el jugador está muy lejos, acercarse.
        else if (distanceToPlayer > approachDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        // Si está en rango adecuado, moverse en órbita.
        else
        {
            // Calcula la dirección hacia el jugador.
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            
            // Actualiza el temporizador para cambiar la dirección orbital.
            orbitalTimer += Time.deltaTime;
            if (orbitalTimer >= orbitalChangeInterval)
            {
                orbitalTimer = 0f;
                orbitalSign = (Random.value > 0.5f) ? 1f : -1f;
            }
            
            // Calcula la dirección orbital multiplicando la rotación de 90° por el signo.
            Vector3 orbitalDirection = Quaternion.Euler(0, 90 * orbitalSign, 0) * directionToPlayer;
            
            // Lanza un raycast en la dirección orbital para evitar obstáculos.
            float rayDistance = 2f; // Ajusta este valor según tus necesidades.
            RaycastHit hit;
            if (Physics.Raycast(transform.position, orbitalDirection, out hit, rayDistance))
            {
                // Si se detecta obstáculo, prueba con la dirección opuesta.
                orbitalDirection = -orbitalDirection;
                if (Physics.Raycast(transform.position, orbitalDirection, out hit, rayDistance))
                {
                    // Si ambos lados están bloqueados, acércate directamente al jugador.
                    agent.SetDestination(player.transform.position);
                    return;
                }
            }
            
            // Calcula la posición destino para el movimiento orbital.
            Vector3 targetPosition = transform.position + orbitalDirection * rayDistance;
            agent.SetDestination(targetPosition);
        }
    }

    public void PlayRandomSpellSound()
    {
        if (spellSounds != null)
        {
            // Convertir la lista de sonidos de paso a un array
            AudioClip[] spellSoundsArray = spellSounds.ToArray();

            // Obtener un índice aleatorio dentro del rango del array
            int randomIndex = Random.Range(0, spellSoundsArray.Length);

            // Coger un sonido de paso aleatorio y reproducirlo
            AudioClip footstepSound = spellSoundsArray[randomIndex];
            // Pitch aleatorio para mayor variedad
        }
    }

    public void OnSpellAnimationEvent()
    {
        if (currentSkill != null)
        {
            currentSkill.OnAnimationEvent(this, player);
            // currentSkill = null;
        }
    }

    private Vector3 GetBestFleeDirection()
    {
        float maxDistance = 0f;
        Vector3 bestDirection = Vector3.zero;

        // Revisamos varios rayos en círculo
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 potentialPos = transform.position + dir * fleeRadius;

            if (NavMesh.SamplePosition(potentialPos, out NavMeshHit hit, checkRadius, NavMesh.AllAreas))
            {
                float distFromPlayer = Vector3.Distance(hit.position, player.transform.position);
                if (distFromPlayer > maxDistance)
                {
                    maxDistance = distFromPlayer;
                    bestDirection = hit.position;
                }
            }
        }

        return bestDirection;
    }

    public override void OnHurt(float dmg, float knockback, Vector3 direction)
    {
        // Lógica de daño
        if (currentState != EnemyState.Dead)
        {
            // Si el enemigo está en combate se pone en combate.
            if(currentState == EnemyState.Idle)
                currentState = EnemyState.Combat;
            if(isBlocking)
            {
                // Si está bloqueando, reduce el daño.
                dmg *= 0.5f;
                knockback = 0f; // No hay retroceso al bloquear
            }
            // Aplicar daño al enemigo
            base.OnHurt(dmg, knockback, direction);
            // TODO Reproducir sonido de daño
        }
    }

    public override void SetPlayer(GameObject player)
    {
        base.SetPlayer(player);
        if (player != null)
        {
            MultiAimConstraint mac = GetComponent<MultiAimConstraint>();
            mac?.data.sourceObjects.Clear();
            mac?.data.sourceObjects.Add(new WeightedTransform(player.transform, 1f));

            RigBuilder rigs = GetComponent<RigBuilder>();
            rigs?.Build();
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();
        if (debug)
        {
            // Obtener la posición en pantalla del enemigo
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            // Ajustar el eje Y para que el origen sea la parte superior de la pantalla
            screenPos.y = Screen.height - screenPos.y;
            
            // Definir offsets para mostrar la información al lado del enemigo
            float offsetX = 20f;  // Desplazamiento horizontal (ajusta según necesites)
            float offsetY = 00f; // Desplazamiento vertical (ajusta según necesites)

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
        }
    }

    public override void Die()
    {
        base.Die();
        if(meleeAttackCollider != null)
            meleeAttackCollider.enabled = false;
        if(currentSkill != null)
            currentSkill.Stop(this, player);
    }

}
