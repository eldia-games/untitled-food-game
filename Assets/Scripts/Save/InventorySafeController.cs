using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventorySafeController : MonoBehaviour
{
    
    [SerializeField] private List<Items> lootList;
    [SerializeField] private TextAsset file ;
    [SerializeField] private string filePath;

    private InventorySave inventory;
    public static InventorySafeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //inventory= new InventorySave();
            //saveInventory();
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inventory= InventorySave.FromJSON(file.text);
        }

    }
    public void saveInventory()
    {
        StreamWriter writer=new StreamWriter(filePath);
        writer.WriteLine(inventory.ToJSON());
        writer.Close();

    }
    public void setsMissions(List<Mission> misions)
    {
        inventory.setMissions(misions);
        saveInventory();

    }
    public List<Mission> getMissions()
    {
        return inventory.getMissions();
    }
    public void addMoney(int money)
    {
        inventory.addMoney(money);
        saveInventory();
    }
    public void setMission(Mission mission,int index)
    {
        inventory.changeMission(mission,index);
        saveInventory();
    }

    public void addInventory(List<ItemInInventory> items)
    {
        foreach (ItemInInventory item in items)
        {
            int index=lootList.IndexOf(item.item);
            if (index != -1)
            {
                inventory.addItem(index, item.quantity);

            }

            
        }
        saveInventory();
    }

    public bool hasItem(Items item,int quantity)
    {
        int index = lootList.IndexOf(item);
        if (index != -1)
        {
            return inventory.hasEnough(index, quantity);
        }
        return false;
    }

    public void removeItem(Items item, int quantity)
    {
        int index = lootList.IndexOf(item);
        if (index != -1)
        {
            inventory.removeItem(index, quantity);
        }
        saveInventory();
    }
    public void Reset()
    {
        inventory.resetMoney();
        inventory.clearLoot();
        inventory.clearMissions();
        saveInventory();
    }
    public int getQuantity(Items item)
    {
        int index = lootList.IndexOf(item);
        if (index != -1)
        {
            return inventory.getQuantity(index);
        }
        return 0;


    }

}
