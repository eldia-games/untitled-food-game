using UnityEngine;

public class BossSkillScriptableObject : ScriptableObject
{
    public float cooldown = 10f;
    public int damage = 5;
    public float minRange = 5.0f;
    public float maxRange = 10.0f;
    public bool isMelee = false;

    public bool isCasting = false;

    protected float castTime = 0.0f;

    public virtual void Initialize()
    {
        castTime = Time.time;
        isCasting = false;
    } 

    public virtual void Use(Boss boss, GameObject player)
    {
        isCasting = true;
    }

    public virtual void HandleMovement(Boss boss, GameObject player){
        boss.StopMovement();
    }

    public virtual bool CanUse(Boss boss, GameObject player)
    {
        return true;
    }

    public virtual bool InMinRange(Boss boss, GameObject player)
    {
        return true;
    }

    public virtual void OnAnimationEvent(Boss boss, GameObject player)
    {
        return;
    }
}