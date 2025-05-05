using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class Items : ScriptableObject
{
    [Header("ItemStats")]
    public string itemName;
    public int id;
    public string description;
    public Texture2D icon;
    public GameObject mesh;
    public abstract Items GetItem();
    public int quantity;

    public bool stackeable;

    public int gold;
}
