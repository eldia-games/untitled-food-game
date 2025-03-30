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

    public virtual void UseSkill(BaseEnemyV2 Enemy, GameObject Player)
    {
        isCasting = true;
    }

    public virtual bool CanUseSkill(BaseEnemyV2 Enemy, GameObject Player)
    {
        return true;
    }

    public virtual void OnAnimationEvent(BaseEnemyV2 enemy, GameObject player)
    {
        return;
    }
}