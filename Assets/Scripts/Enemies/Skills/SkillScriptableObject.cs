using UnityEngine;

public class SkillScriptableObject : ScriptableObject
{
    public float cooldown = 10f;
    public int damage = 5;

    public bool isCasting = false;

    protected float castTime = 0.0f;

    public virtual void Initialize()
    {
        castTime = Time.time;
        isCasting = false;
    } 

    public virtual bool InRange(BaseEnemyV2 enemy, GameObject player)
    {
        return true;
    }

    public virtual void UseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        isCasting = true;
    }

    public virtual void Use(BaseEnemyV2 enemy, GameObject player)
    {
        isCasting = true;
    }

    public virtual bool CanUse(BaseEnemyV2 enemy, GameObject player)
    {
        return true;
    }

    public virtual bool CanUseSkill(BaseEnemyV2 enemy, GameObject player)
    {
        return true;
    }

    public virtual void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        return;
    }
}