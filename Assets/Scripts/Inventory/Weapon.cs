using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class WeaponType : Items
{

    [Header("WeaponStats")]
    public int damage;

    [Header("Weapon Type: 0 = Melee 1 hand, 1 = Melee 2 hand, 2 = Ballista, 4 = Staff, 5 = None(Gloves/Boots)")]
    public int type;
    public int attackSpeed;
    public float MovementSpeed;
    public override Items GetItem()
    {
        return this;
    }

    public WeaponType(int damage, int type, int attackSpeed, float MovementSpeed)
    {
        this.damage = damage;
        this.type = type;
        this.attackSpeed = attackSpeed;
        this.MovementSpeed = MovementSpeed;
    }

}