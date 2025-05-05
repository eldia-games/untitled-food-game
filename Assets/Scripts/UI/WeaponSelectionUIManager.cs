using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelectionUIManager : MonoBehaviour
{
    [System.Serializable]
    private class WeaponInfo
    {
        public int weaponID;
        public string weaponName;
        public Sprite weaponImage;
        public float damage;
        public float fireRate;
        public string rangedType;
    }
    private int currentWeaponIndex = 0;

    [SerializeField] private WeaponInfo[] weapons;
    [SerializeField] private Image weaponDisplayImage;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private TMP_Text weaponFireRateText;
    [SerializeField] private TMP_Text weaponRangedTypeText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        weaponDisplayImage.sprite = weapons[currentWeaponIndex].weaponImage;
        weaponNameText.text = weapons[currentWeaponIndex].weaponName;
        weaponDamageText.text = weapons[currentWeaponIndex].damage.ToString();
        weaponFireRateText.text = weapons[currentWeaponIndex].fireRate.ToString();
        weaponRangedTypeText.text = weapons[currentWeaponIndex].rangedType.ToString();
        print("weapon index: " + currentWeaponIndex);
    }

    public void ShowNextWeapon()
    {
        if (currentWeaponIndex < 3)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            UpdateUI();
            
        }
        else
        {
            Debug.Log("Fin de armas");
        }
    }

    public void ShowPreviousWeapon()
    {
        if (currentWeaponIndex > 0)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            UpdateUI();
        }
        else {
            Debug.Log("Inicio de armas");
        }
    }

    public void PlayerSelectedWeapon(){
        int type = weapons[currentWeaponIndex].weaponID;
        GameManager.Instance.setCurrentWeaponType(type);
        print("Arma seleccionada: " + type);
    }

}
