using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class InventorySave
{

    #region Variables
    [SerializeField] private List<Mission> missions;
    [SerializeField] private int money = 0;
    [SerializeField] private List<Items> loot;
    [SerializeField] private List<int> lootquantity;

    [SerializeField] private List<Achievement> achievements;

    #endregion

    #region Persistence
    public InventorySave()
    {
        money = 0;
        loot = new List<Items>();
        lootquantity= new List<int>();
        missions = new List<Mission>();
        achievements= new List<Achievement>();
    }
    public static InventorySave FromJSON(string inventoryjson)
    {
        return JsonUtility.FromJson<InventorySave>(inventoryjson);
    }

    public string ToJSON()
    {
        String json= JsonUtility.ToJson(this);
        return JsonUtility.ToJson(this);
    }
    //public void setAchievementsNumber(int number)
    //{
    //    if (achievements.Count < number)
    //    {
    //        for(int i= achievements.Count; i < number; i++)
    //        {
    //            achievements.Add(false);
    //        }
    //    }
    //}

    public void clearLoot()
    {
        lootquantity.Clear();
        loot.Clear();
    }
    public void clearMissions()
    {
        missions.Clear();
    }
    #endregion

    #region Missions
    public void setMissions(List<Mission> missions)
    {
       this.missions = missions;
    }
    public void changeMission(Mission mission, int index)
    {
        missions[index] = mission;
    }
    public List<Mission> getMissions()
    {
        return missions;
    }
    #endregion

    #region Money
    public void addMoney(int money)
    {
        this.money += money;
    }
    public void resetMoney()
    {
        this.money = 0;
    }
    public void substractMoney(int money)
    {
        this.money -= money;
    }

    public int getMoney()
    {
        return this.money;
    }
    #endregion

    #region Items
    public void addItem(Items lootItem, int quantity)
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
    public int getQuantity(Items lootItem)
    {
        int index;
        if ((index = loot.IndexOf(lootItem)) == -1)
        {
            return 0;
        }
        return lootquantity[index];
    }


    #endregion

    #region Achievements 

    public void SetAchievements(List<Achievement> achs)
    {
        this.achievements = achs;
    }
    public void SetAchievement(Achievement achievement, int index)
    {
        achievements[index] = achievement;
    }
    public List<Achievement> getAchievements()
    {
        return achievements;
    }

    #endregion
}

