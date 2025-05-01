using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum powerUpType
{
    None,
    MaxHealth,
    Heal,
    Damage,
    AttackSpeed,
    Movement,
    Push,
    Mana,
    ManaRegen

}

public class PowerUpStatsController : MonoBehaviour
{

    #region Variables
    private PowerUpSave stats;

    [SerializeField] private string filePath;

    [SerializeField] private int maxLevel;

    [Header("Movement Spped")]

    [SerializeField] private  float movementSpeedAdd;
    [SerializeField] private float movementSpeedBase;

    [Header("Heal")]
    [SerializeField] private  float healAdd;
    [SerializeField] private float healBase;

    [Header("Max Health")]
    [SerializeField] private  float maxLifeAdd;
    [SerializeField] private float maxLifeBase;

    [Header("Damage")]
    [SerializeField] private  float damageAdd;
    [SerializeField] private float damageBase;

    [Header("AttackSpeed")]
    [SerializeField] private  float atackSpeedAdd;
    [SerializeField] private float atackSpeedBase;

    [Header("Puah Force")]
    [SerializeField] private  float pushForceAdd;
    [SerializeField] private float pushForceBase;

    [Header("Max Mana")]
    [SerializeField] private  float maxManaAdd;
    [SerializeField] private float maxManaBase;

    [Header("Mana Regeneration")]
    [SerializeField] private float  manaRegenAdd;
    [SerializeField] private float manaRegenBase;


    #endregion

    #region MonoBehaviour
    public static PowerUpStatsController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
    #endregion

    #region Persistence
    public void loadGame()
    {
        StreamReader reader = new StreamReader(filePath);
        string json = reader.ReadToEnd();
        reader.Close();
        stats = PowerUpSave.FromJSON(json);
    }

    public void newGame()
    {
        stats = new PowerUpSave();
        saveStats();
    }
    public bool canLoadGame()
    {
        try
        {
            StreamReader reader = new StreamReader(filePath);
            string json = reader.ReadToEnd();
            reader.Close();
            if (json=="" || PowerUpSave.FromJSON(json) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }

    }
    public void saveStats()
    {
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine(stats.ToJSON());
        writer.Close();

    }
    public void Reset()
    {
        stats.reset();
        saveStats();
    }
    #endregion

    #region PowerUps
    public void LevelUpPowerUp(powerUpType type)
    {

        switch (type)
        {
            case powerUpType.None:
                break;
            case powerUpType.MaxHealth:
                if (stats.getMaxLifeLevel() < maxLevel)
                {
                    stats.levelUpMaxLifeLevel();
                }

                break;
            case powerUpType.Heal:
                if (stats.getHealLevel() < maxLevel)
                {
                    stats.levelUpHealLevel();
                }
                break;
            case powerUpType.Damage:
                if (stats.getDamageLevel() < maxLevel)
                {
                    stats.levelUpDamageLevel();
                }
                break;
            case powerUpType.AttackSpeed:
                if (stats.getAttackSpeedLevel() < maxLevel)
                {
                    stats.levelUpAttackSpeedLevel();
                }
                break;
            case powerUpType.Movement:
                if (stats.getMovementSpeedLevel() < maxLevel)
                {
                    stats.levelUpMovementSpeed();
                }
                break;
            case powerUpType.Push:
                if (stats.getPushForceLevel() < maxLevel)
                {
                    stats.levelUpPushForceLevel();
                }
                break;
            case powerUpType.Mana:
                if (stats.getMaxManaLevel() < maxLevel)
                {
                    stats.levelUpMaxManaLevel();
                }
                break;
            case powerUpType.ManaRegen:
                if (stats.getManaRegenLevel() < maxLevel)
                {
                    stats.levelUpManaRegenLevel();
                }
                break;
            default:
                break;

        }
        saveStats();
    }



    public void PowerUp(powerUpType powerUpType)
    {
        PlayerStatsController controller = PlayerStatsController.Instance;
        switch (powerUpType)
        {
            case powerUpType.None:
                break;
            case powerUpType.MaxHealth:
                controller.augmentMaxHealht(Mathf.RoundToInt(stats.getMaxLifeLevel() * maxLifeAdd+maxLifeBase));

                break;
            case powerUpType.Heal:
                controller.augmentHeal(stats.getHealLevel() * healAdd+healBase);
                break;
            case powerUpType.Damage:
                controller.augmentMaxDamage(stats.getDamageLevel() * damageAdd+damageBase);
                break;
            case powerUpType.AttackSpeed:
                controller.augmentAttackSpeed(stats.getAttackSpeedLevel() * atackSpeedAdd + atackSpeedBase);
                break;
            case powerUpType.Movement:
                controller.augmentMaxmoveSpeed(stats.getMovementSpeedLevel() * movementSpeedAdd+movementSpeedBase);
                break;
            case powerUpType.Push:
                controller.augmentPushForce(stats.getPushForceLevel() * pushForceAdd+pushForceBase);
                break;
            case powerUpType.Mana:
                controller.augmentMaxMana(Mathf.RoundToInt(stats.getMaxManaLevel() * maxManaAdd+maxManaBase));
                break;
            case powerUpType.ManaRegen:
                controller.augmentManaRegen(stats.getManaRegenLevel() * manaRegenAdd+manaRegenBase);
                break;
            default:
                break;
        }
    }

    public List<int> getLevels()
    {
        List<int> levels = new List<int>();
        for(int i=0;i<Enum.GetNames(typeof(powerUpType)).Length;i++)
        {
            powerUpType type = (powerUpType) i;
            switch (type)
            {
                case powerUpType.None:
                    levels.Add(0);
                    break;
                case powerUpType.MaxHealth:

                    levels.Add(stats.getMaxLifeLevel());
                    break;
                case powerUpType.Heal:
                    levels.Add(stats.getHealLevel());
                    break;
                case powerUpType.Damage:
                    levels.Add(stats.getDamageLevel());
                    break;
                case powerUpType.AttackSpeed:
                    levels.Add(stats.getAttackSpeedLevel());
                    break;
                case powerUpType.Movement:
                    levels.Add(stats.getMovementSpeedLevel());
                    break;
                case powerUpType.Push:
                    levels.Add(stats.getPushForceLevel()); 
                    break;
                case powerUpType.Mana:
                    levels.Add(stats.getMaxManaLevel());
                    break;
                case powerUpType.ManaRegen:
                    levels.Add(stats.getManaRegenLevel());
                    break;
                default:
                    break;
            }

        }
        return levels;
    }


    #endregion

}
