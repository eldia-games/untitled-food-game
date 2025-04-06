using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UpgradesUIManager : MonoBehaviour
{
    [SerializeField] private Button[] buttonsUpgrade;
    [SerializeField] private TMP_Text[] currentLevel;
    [SerializeField] private TMP_Text[] upgradePrice;
    [SerializeField] private TMP_Text[] upgradeText;

    private PowerUpStatsController powerUpStatsController;
    private InventorySafeController inventory;
    private List<int> powerups;
    private int basePrice;
    private void Start()
    {
        powerUpStatsController = PowerUpStatsController.Instance;
        inventory = InventorySafeController.Instance;
    }
    public void RefreshUpgrades()
    {
        powerups = powerUpStatsController.getLevels();
        currentLevel[0].text = powerups[(int)powerUpType.MaxHealth].ToString();
        currentLevel[1].text = powerups[(int)powerUpType.Heal].ToString();
        currentLevel[2].text = powerups[(int)powerUpType.Mana].ToString();
        currentLevel[3].text = powerups[(int)powerUpType.ManaRegen].ToString();
        currentLevel[4].text = powerups[(int)powerUpType.Damage].ToString();
        currentLevel[5].text = powerups[(int)powerUpType.AttackSpeed].ToString();
        currentLevel[6].text = powerups[(int)powerUpType.Movement].ToString();
        currentLevel[7].text = powerups[(int)powerUpType.Push].ToString();
        //inventory.hasMoney();

    }

    public void LevelUpHeal()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.MaxHealth]) / 1.0f);
        if (inventory.hasMoney(price) )
        {
            powerUpStatsController.LevelUpPowerUp(powerUpType.Heal);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }
    public void LevelUpGeneral(powerUpType type)
    {
        powerUpStatsController.LevelUpPowerUp(type);
        RefreshUpgrades();
    }

    public void LevelUpMaxHealth()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.MaxHealth);
        RefreshUpgrades();
    }

    public void LevelUpMana()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.Mana);
        RefreshUpgrades();
    }

    public void LevelUpManaRegen()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.ManaRegen);
        RefreshUpgrades();
    }

    public void LevelUpDamage()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.Damage);
        RefreshUpgrades();
    }

    public void LevelUpAttackSpeed()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.AttackSpeed);
        RefreshUpgrades();
    }
    public void LevelUpPush()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.Push);
        RefreshUpgrades();
    }

    public void LevelUpMovement()
    {
        powerUpStatsController.LevelUpPowerUp(powerUpType.Movement);
        RefreshUpgrades();
    }


}
