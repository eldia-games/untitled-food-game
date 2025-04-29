
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventorySafeController : MonoBehaviour
{

    #region Variables
    [SerializeField] private string filePath;

    private InventorySave inventory;
    #endregion

    #region MonoBehaviour
    public static InventorySafeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);

            inventory = new InventorySave(); 
        }

    }
    #endregion

    #region Persistence
    public void loadGame()
    {
        StreamReader reader = new StreamReader(filePath);
        string json=reader.ReadToEnd();
        reader.Close();
        inventory = InventorySave.FromJSON(json);
    }

    public void newGame()
    {
        inventory = new InventorySave();
        saveInventory();
    }
    public void saveInventory()
    {
        StreamWriter writer=new StreamWriter(filePath);
        writer.WriteLine(inventory.ToJSON());
        writer.Close();

    }

    public void Reset()
    {
        inventory.resetMoney();
        inventory.clearLoot();
        inventory.clearMissions();
        saveInventory();
    }

    public bool canLoadGame()
    {
        try
        {
            StreamReader reader = new StreamReader(filePath);
            string json = reader.ReadToEnd();
            reader.Close();
            if (json == "" || InventorySave.FromJSON(json) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }

    }


    #endregion

    #region Missions
    public void setsMissions(List<Mission> misions)
    {
        inventory.setMissions(misions);
        saveInventory();

    }

    public List<Mission> getMissions()
    {
        return inventory.getMissions();
    }

    public void setMission(Mission mission, int index)
    {
        inventory.changeMission(mission, index);
        saveInventory();
    }
    #endregion

    #region Money
    public void addMoney(int money)
    {
        inventory.addMoney(money);
        saveInventory();
    }

    public void substractMoney(int money)
    {
        if(hasMoney(money))
        {
            inventory.substractMoney(money);
        }
        else
        {
            inventory.resetMoney();

        }
        saveInventory();
    }
    public bool hasMoney(int money)
    {
        if (inventory.getMoney() >= money)
        {
            return true;
        }
        return false;
    }

    public int getMoney()
    {
        return inventory.getMoney();
    }

    #endregion

    #region Items
    public void addInventory(List<ItemInInventory> items)
    {
        foreach (ItemInInventory item in items)
        {

         inventory.addItem(item.item, item.quantity);
            
        }
        saveInventory();
    }

    public bool hasItem(Items item,int quantity)
    {

            return inventory.hasEnough(item, quantity);

    }

    public void removeItem(Items item, int quantity)
    {

        inventory.removeItem(item, quantity);
        saveInventory();

    }
    public int getQuantity(Items item)
    {

        return inventory.getQuantity(item);


    }
    #endregion

}
