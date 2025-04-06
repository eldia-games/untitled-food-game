using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class InventorySave
{
    //[SerializeField] private List<Mission> missions;
    [SerializeField] public int money = 0;
    [SerializeField] public List<int> loot;
    [SerializeField] public List<int> lootquantity;

    public InventorySave()
    {
        money = 0;
        loot = new List<int>();
        lootquantity= new List<int>();
    }
    public static InventorySave FromJSON(string json)
    {
        return JsonUtility.FromJson<InventorySave>(json);
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
    public void setMissions(List<Mission> missions)
    {
       // this.missions = missions;
    }
    public void changeMission(Mission mission, int index)
    {
        //missions[index] = mission;
    }
    public List<Mission> getMissions()
    {
        //return missions;
        return null;
    }
    public void addMoney(int money)
    {
        this.money += money;
    }
    public void resetMoney()
    {
        this.money = 0;
    }
    public void addItem(int lootItem, int quantity)
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
       // missions.Clear();
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

