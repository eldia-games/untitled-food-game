using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PosibleLoot", menuName = "PosibleLoot")]
public class DropsList : ScriptableObject
{

    [Header("LootTable")]
    public List<GameObject> posibleLootTablePrefabs;
}
