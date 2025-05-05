using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct missionItem
{
    public Items item;
    public int quantity;
}
[CreateAssetMenu(fileName = "New Recipe", menuName = "Missions/Recipe")]
public class ScriptableRecipe : ScriptableObject
{
   public   List<missionItem> items;
   public   int level;
   public   string title;
}
