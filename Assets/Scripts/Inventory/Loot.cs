using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot", menuName = "Items/Loot")]
public class Loot : Items
{
    //[Header("LootStats")]

    public override Items GetItem()
    {
        return this;
    }
}
