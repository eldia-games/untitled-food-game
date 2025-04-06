using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PowerUpSave
{

    #region Variables
    [SerializeField] private int MovementSpeedLevel;
    [SerializeField] private int HealLevel;
    [SerializeField] private int maxLifeLevel;
    [SerializeField] private int damageLevel;
    [SerializeField] private int attackSpeedLevel;
    [SerializeField] private int pushForceLevel;
    [SerializeField] private int maxManaLevel;
    [SerializeField] private int manaRegenLevel;

    #endregion

    #region Persistence
    public PowerUpSave()
    {
        MovementSpeedLevel = 1;
        HealLevel = 1;
        maxLifeLevel = 1;
        damageLevel = 1;
        attackSpeedLevel = 1;
        pushForceLevel = 1;
        maxManaLevel = 1;
        manaRegenLevel = 1;
    }
    public void reset()
    {
        MovementSpeedLevel = 1;
        HealLevel = 1;
        maxLifeLevel = 1;
        damageLevel = 1;
        attackSpeedLevel = 1;
        pushForceLevel = 1;
        maxManaLevel = 1;
        manaRegenLevel = 1;
    }
    public static PowerUpSave FromJSON(string inventoryjson)
    {
        return JsonUtility.FromJson<PowerUpSave>(inventoryjson);
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
    #endregion

    #region LevelUp
    public void levelUpMovementSpeed()
    {
        MovementSpeedLevel++;
    }
    public void levelUpHealLevel()
    {
        HealLevel++;
    }
    public void levelUpMaxLifeLevel()
    {
        maxLifeLevel++;
    }

    public void levelUpDamageLevel()
    {
        damageLevel++;
    }

    public void levelUpAttackSpeedLevel()
    {
        attackSpeedLevel++;
    }
    public void levelUpPushForceLevel() {
        pushForceLevel++;
    }
    public void levelUpMaxManaLevel()
    {
        maxManaLevel++;
    }
    public void levelUpManaRegenLevel()
    {
        manaRegenLevel++;
    }

    #endregion

    #region GetStats
    public int getMovementSpeedLevel()
    {
        return MovementSpeedLevel;
    }

    public int getHealLevel()
    {
        return HealLevel;
    }
    public int getMaxLifeLevel()
    {
        return maxLifeLevel;
    }

    public int getDamageLevel()
    {
        return damageLevel;
    }

    public int getAttackSpeedLevel()
    {
        return attackSpeedLevel;
    }
    public int getPushForceLevel()
    {
        return pushForceLevel;
    }
    public int getMaxManaLevel()
    {
        return maxManaLevel;
    }
    public int getManaRegenLevel()
    {
        return manaRegenLevel;
    }
    #endregion

}
