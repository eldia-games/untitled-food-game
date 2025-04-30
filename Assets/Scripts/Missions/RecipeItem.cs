using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipeItem 
{
    #region Variable

    [SerializeField] private Items item;
    [SerializeField] private int quantity;

    #endregion

    #region Item
    public RecipeItem(Items item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    public Items GetItem()
    {
        return item;
    }
    public int GetQuantity()
    {
        return quantity;
    }
    #endregion
}
