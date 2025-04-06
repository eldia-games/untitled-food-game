using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mission 
{
    [SerializeField] private Items item;
    [SerializeField] private int quantity;
    [SerializeField] private int money;

    public Mission(Items item, int quantity, int money)
    {
        this.item = item;
        this.quantity = quantity;
        this.money = money;
    }

    public Items getItem()
    {
         return item;
        //return null;
    }
    public int getQuantity()
    {

        return quantity;
    }

    public int getPrice()
    {
        return money;
    }
    public void setValues(Items item,int quantity,int money)
    {
        this.item = item;
        this.quantity = quantity;
        this.money = money;
    }

   
}
