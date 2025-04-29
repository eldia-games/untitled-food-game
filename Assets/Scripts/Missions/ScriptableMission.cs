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
[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Mission")]
public class ScriptableMission : ScriptableObject
{
   public   List<missionItem> items;
   public   int level;
   public   string title;
}
