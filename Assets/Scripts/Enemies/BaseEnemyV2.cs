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

    // Declara esta variable para guardar la salud máxima
    private float maxHealth;

    protected override void Start()
    {
        base.Start();
        maxHealth = health; // Guarda la salud inicial como máxima
    }

    public virtual void OnGUI()
    {
        // Ajusta la posición para que la barra aparezca encima del enemigo (ej. 2 unidades arriba)
        Vector3 worldPosition = transform.position + Vector3.up * 2f;
        // Convierte la posición del mundo a coordenadas de pantalla
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        // Ajusta la coordenada Y (la posición de pantalla tiene Y invertido)
        screenPosition.y = Screen.height - screenPosition.y;
        
        // Dimensiones de la barra
        float barWidth = 50f;
        float barHeight = 5f;
        
        // Calcula el porcentaje de salud
        float healthPercentage = health / maxHealth;
        
        // Dibuja la barra de fondo (rojo)
        // GUI.color = Color.red;
        // GUI.Box(new Rect(screenPosition.x - barWidth / 2, screenPosition.y, barWidth, barHeight), GUIContent.none);
        
        // Dibuja la barra de salud actual (verde)
        GUI.color = Color.green;
        GUI.Box(new Rect(screenPosition.x - barWidth / 2, screenPosition.y, barWidth * healthPercentage, barHeight), GUIContent.none);
        
        // Restaura el color original
        GUI.color = Color.white;
    }

}
