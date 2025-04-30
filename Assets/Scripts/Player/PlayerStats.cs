using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{

    #region InputVariables

    [Header("Movement")]
    //movement
    public float MovementSpeed = 9f;
    
    [Header("Stamina")]
    //Stamina
    [Range(0,10)]
    public float StaminaSlide = 10f;

    public float StaminaRegen = 5f;

    //Stamina/Slide
    public float velSlide = 1.0f;

    
    [Header("HP")]
    //HP
    public int maxLife = 100;

    public float heal = 10f;

    public float HP;

    [Header("Damage")]
    //Damage
    public float damage = 10f;

    //VelAttack
    public float velAttack = 1.0f;

    //PushForce
    public float pushForce = 10f;

    public float damageModifier = 1f;

    public float pushModifier = 1f;

    [Header("MP")]
    //MP
    public int maxMana = 100;

    public float MP;

    public float manaCost = 10f;

    public float manaRegen = 0.1f;

    [Header("Weapons: 0 = 1handAxe, 1 = 2handAxe, 2 = Bow, 3 = Mug to heal, 4 = Staff, 5 = None , more?")]
    //Weapons
    [Range(0,5)]
    public int weaponIndex = 0;

    [Header("Bullets: 0 = Arrow, 1 = Fireball")]
    //Supose to be a prefab of the bullet, changed before the run starts
    public List<GameObject> weaponType;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void Reset()
    {
        //Default values

        //Movement
        MovementSpeed = 9f;

        //Stamina
        StaminaSlide = 10;
        StaminaRegen = 5;
        velSlide = 1.0f;

        //HP
        maxLife = 100;
        heal = 10;
        HP = maxLife;

        //Damage
        damage = 10;
        velAttack = 1.0f;
        pushForce = 10;
        damageModifier = 1;
        pushModifier = 1;

        //MP
        maxMana = 100;
        MP = maxMana;
        manaCost = 10;
        manaRegen = 0.1f;

        //Weapons
        weaponIndex = 5;

        //Bullets
        //weaponType = new List<GameObject>
        //{
        //    //"D:\github_unibotics\EldiaGames\untitled-food-game\Assets\Prefabs\Player\Bullets\arrow.prefab"
        //    // "D:\github_unibotics\EldiaGames\untitled-food-game\Assets\Prefabs\Player\Bullets\fireball.prefab"
        //
        //    //(GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player/Bullets/arrow.prefab", typeof(GameObject)),
        //    //(GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player/Bullets/fireball.prefab", typeof(GameObject))
        //};   
    }

    internal void SetWeaponType(int weaponType)
    {
        switch (weaponType)
      {
          case 0:
              Debug.Log("Weapon type: " + weaponType + " - Sword");
              damage = 10f;
              velAttack = 1f;
              weaponIndex = 0;
              break;
          case 1:
              Debug.Log("Weapon type: " + weaponType + " - Axe");
              damage = 10f;
              velAttack = 0.7f;
              weaponIndex = 1;
              break;
          case 2:
              Debug.Log("Weapon type: " + weaponType + " - Bow");
              damage = 5f;
              velAttack = 1f;
              weaponIndex = 2;
              break;
          case 4:
              Debug.Log("Weapon type: " + weaponType + " - Staff");
              damage = 20f;
              velAttack = 0.5f;
              weaponIndex = 4;
              break;
          default:
              Debug.Log("Invalid weapon type: " + weaponType);
            damage = 0f;
            velAttack = 0f;
            weaponIndex = 5;
              break;
      }
    }
}
