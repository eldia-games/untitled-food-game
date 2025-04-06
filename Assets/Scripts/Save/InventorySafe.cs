using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InventorySafe",menuName = "Save/InventorySafe", order =1)]
public class InventorySafe : ScriptableObject
{
    [SerializeField] private List<Mission> missions;
    [SerializeField] private int money=0;
    [SerializeField] private List<Items> loot;
    [SerializeField] private List<int> lootquantity;

    public void setMissions(List<Mission> missions)
    {
        this.missions = missions;
    }
    public void changeMission(Mission mission, int index)
    {
        missions[index]=mission;
    }
    public void addMoney(int money)
    {
        this.money += money;
    }
    public void resetMoney()
    {
        this.money = 0;
    }
    public void addItem(Items lootItem,int quantity)
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
    public List<Mission> getMissions()
    {
        return missions;
    }
    public void removeItem(Items lootItem, int quantity)
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
    public bool hasEnough(Items lootItem, int quantity)
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
    public int getQuantity(Items lootItem)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) == -1)
        {
            return 0;
        }
        return lootquantity[index];
    }
}
