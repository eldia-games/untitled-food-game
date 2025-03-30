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
        public int weaponID; // Tipo integer como solicitaste
        public string weaponName;
        public Sprite weaponImage;
    }
    private int currentWeaponIndex = 0;

    [SerializeField] private WeaponInfo[] weapons;
    [SerializeField] private Image weaponDisplayImage;
    [SerializeField] private TMP_Text weaponNameText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        weaponDisplayImage.sprite = weapons[currentWeaponIndex].weaponImage;
        weaponNameText.text = weapons[currentWeaponIndex].weaponName;
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
