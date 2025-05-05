using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mission 
{
    #region Variables
    [SerializeField] private List<RecipeItem> item;

    [SerializeField] private int money;
    [SerializeField] private string title;
    #endregion

    #region Missions
    public Mission(List<RecipeItem> item, int money,string title)
    {
        this.item = item;
        this.money = money;
        this.title = title;
    }

    public List<RecipeItem> getItems()
    {
         return item;
        //return null;
    }


    public int getPrice()
    {
        return money;
    }
    public string getTitle()
    {
        return title;
    }
    public void setValues(List<RecipeItem> item,int quantity,int money)
    {
        this.item = item;

        this.money = money;
    }
    #endregion

}
