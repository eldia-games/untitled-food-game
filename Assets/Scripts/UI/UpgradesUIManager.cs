using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System.Diagnostics;

public class UpgradesUIManager : MonoBehaviour
{
    [SerializeField] private Button[] buttonsUpgrade;
    [SerializeField] private TMP_Text[] currentLevel;
    [SerializeField] private TMP_Text[] upgradePrice;
    [SerializeField] private TMP_Text[] upgradeText;

    private PowerUpStatsController powerUpStatsController;
    private InventorySafeController inventory;
    private List<int> powerups;
    private int basePrice = 25;
    void Start()
    {
        inventory = InventorySafeController.Instance;
    }
    public void RefreshUpgrades()
    {
        powerups = PowerUpStatsController.Instance.getLevels();
        currentLevel[0].text = powerups[(int)powerUpType.MaxHealth].ToString();
        int i = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.MaxHealth]) / 1.0f);
        upgradePrice[1].text = i.ToString();
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
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Heal]) / 1.0f);
        if (inventory.hasMoney(price) )
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Heal);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }
    public void LevelUpGeneral(powerUpType type)
    {

        PowerUpStatsController.Instance.LevelUpPowerUp(type);
        RefreshUpgrades();
    }

    public void LevelUpMaxHealth()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.MaxHealth]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.MaxHealth);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }

    public void LevelUpMana()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Mana]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Mana);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }

    public void LevelUpManaRegen()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.ManaRegen]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.ManaRegen);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }

    public void LevelUpDamage()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Damage]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Damage);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }

    public void LevelUpAttackSpeed()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.AttackSpeed]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.AttackSpeed);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }
    public void LevelUpPush()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Push]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Push);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }

    public void LevelUpMovement()
    {
        int price = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Movement]) / 1.0f);
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Movement);
            inventory.substractMoney(price);
            RefreshUpgrades();
        }
    }


}
