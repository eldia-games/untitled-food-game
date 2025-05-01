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
    public bool debug = false;
    // Velocidad a la que la barra roja (daño) se drena hacia la salud actual
    [SerializeField]
    private float healthBarDrainSpeed = 10f;
    // Porcentaje actual de la "barra de daño" que se anima
    private float redHealthPercentage;
    // Declara esta variable para guardar la salud máxima
    private float maxHealth;
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

    private void UpdateHealthBar()
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

    protected override void Update()
    {
        UpdateHealthBar();
        base.Update();
    }

    protected override void Start()
    {
        base.Start();
        maxHealth = health;
        // Inicializamos la barra roja al 100%
        redHealthPercentage = 1f;
    }

    public virtual void OnGUI()
    {
        if (Camera.main == null) return;

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
        GUI.color = Color.green;
        GUI.DrawTexture(greenRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        // Restaurar
        GUI.color = Color.white;
    }


}
