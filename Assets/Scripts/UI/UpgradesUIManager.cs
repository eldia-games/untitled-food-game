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
    [SerializeField] private TMP_Text playerMoney;

    private PowerUpStatsController powerUpStatsController;
    private InventorySafeController inventory;
    private List<int> powerups;
    private int basePrice = 25;
    private int[] priceList = new int[8];
    void Start()
    {
        inventory = InventorySafeController.Instance;
    }
    public void RefreshUpgrades()
    {
        powerups = PowerUpStatsController.Instance.getLevels();
        playerMoney.text = InventorySafeController.Instance.getMoney().ToString();

        priceList[0] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.MaxHealth]) / 1.0f);
        upgradePrice[0].text = priceList[0].ToString();
        currentLevel[0].text = powerups[(int)powerUpType.MaxHealth].ToString();

        priceList[1] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Heal]) / 1.0f);
        upgradePrice[1].text = priceList[0].ToString();
        currentLevel[1].text = powerups[(int)powerUpType.Heal].ToString();

        priceList[2] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Mana]) / 1.0f);
        upgradePrice[2].text = priceList[2].ToString();
        currentLevel[2].text = powerups[(int)powerUpType.Mana].ToString();

        priceList[3] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.ManaRegen]) / 1.0f);
        upgradePrice[3].text = priceList[3].ToString();
        currentLevel[3].text = powerups[(int)powerUpType.ManaRegen].ToString();

        priceList[4] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Damage]) / 1.0f);
        upgradePrice[4].text = priceList[4].ToString();
        currentLevel[4].text = powerups[(int)powerUpType.Damage].ToString();

        priceList[5] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.AttackSpeed]) / 1.0f);
        upgradePrice[5].text = priceList[5].ToString();
        currentLevel[5].text = powerups[(int)powerUpType.AttackSpeed].ToString();

        priceList[6] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Movement]) / 1.0f);
        upgradePrice[6].text = priceList[6].ToString();
        currentLevel[6].text = powerups[(int)powerUpType.Movement].ToString();

        priceList[7] = (int)(basePrice * Mathf.Pow(2.0f, powerups[(int)powerUpType.Push]) / 1.0f);
        upgradePrice[7].text = priceList[7].ToString();
        currentLevel[7].text = powerups[(int)powerUpType.Push].ToString();

    }


    public void LevelUpMaxHealth()
    {
        int price = priceList[0];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.MaxHealth);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpHeal()
    {
        int price = priceList[1];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Heal);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpMana()
    {
        int price = priceList[2];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Mana);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpManaRegen()
    {
        int price = priceList[3];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.ManaRegen);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpDamage()
    {
        int price = priceList[4];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Damage);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpAttackSpeed()
    {
        int price = priceList[5];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.AttackSpeed);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }
    public void LevelUpPush()
    {
        int price = priceList[6];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Push);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    public void LevelUpMovement()
    {
        int price = priceList[7];
        if (inventory.hasMoney(price))
        {
            PowerUpStatsController.Instance.LevelUpPowerUp(powerUpType.Movement);
            inventory.substractMoney(price);
            RefreshUpgrades();
            AudioManager.Instance.PlaySFXConfirmation();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }


}
