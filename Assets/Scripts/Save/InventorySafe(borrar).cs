using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class InventorySafe
{
    [SerializeField] private List<Mission> missions;
    [SerializeField] private int money=0;
    [SerializeField] private List<int> loot;
    [SerializeField] private List<int> lootquantity;

    public static InventorySafe FromJSON(string json)
    {
        return JsonUtility.FromJson<InventorySafe>(json);
    }

    public string ToJSON()
    { 
        return JsonUtility.ToJson(this);
    }
    public void setMissions(List<Mission> missions)
    {
        this.missions = missions;
    }
    public void changeMission(Mission mission, int index)
    {
        missions[index]=mission;
    }
    public List<Mission> getMissions()
    {
        return missions;
    }
    public void addMoney(int money)
    {
        this.money += money;
    }
    public void resetMoney()
    {
        this.money = 0;
    }
    public void addItem(int lootItem,int quantity)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) == -1)
        {
            loot.Add(lootItem);
            lootquantity.Add(quantity);
        }
        else
        {
            lootquantity[index] += quantity;
        }

    }

    public void removeItem(int lootItem, int quantity)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) != -1)
        {
            if (lootquantity[index] > quantity)
            {
                lootquantity[index] -= quantity;
            }
            else
            {

                loot.RemoveAt(index);
                lootquantity.RemoveAt(index);
            }

        }
    }
    public bool hasEnough(int lootItem, int quantity)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) == -1)
        {
            return false;
        }
        if (lootquantity[index] < quantity)
        {
            return false;
        }
        return true;
    }
    public void clearLoot()
    {
        lootquantity.Clear();
        loot.Clear();
    }
    public void clearMissions()
    {
        missions.Clear();
    }
    public int getQuantity(int lootItem)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) == -1)
        {
            return 0;
        }
        return lootquantity[index];
    }
}
