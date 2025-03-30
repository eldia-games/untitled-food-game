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
    public float MovementSpeed = 5f;
    
    [Header("Stamina")]
    //Stamina
    [Range(0,10)]
    public float StaminaSlide = 10;

    //Stamina/Slide
    public float velSlide = 1.0f;

    
    [Header("HP")]
    //HP
    public int maxLife = 100;

    public float heal = 10;

    public float HP;

    [Header("Damage")]
    //Damage
    public float damage = 10;

    //VelAttack
    public float velAttack = 1.0f;

    //PushForce
    public float pushForce = 10;

    public float damageModifier = 1;

    public float pushModifier = 1;

    [Header("MP")]
    //MP
    public int maxMana = 100;

    public float MP;

    public float manaCost = 10;

    public float manaRegen = 0.1f;

    [Header("Weapons: 0 = 1handAxe, 1 = 2handAxe, 2 = Bow, 3 = Mug to heal, 4 = Staff, 5 = None , more?")]
    //Weapons
    [Range(0,5)]
    public int weaponIndex = 0;

    [Header("WeaponType (If to change the bullet type)")]
    //Supose to be a prefab of the bullet, changed before the run starts
    public GameObject weaponType;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }





}
