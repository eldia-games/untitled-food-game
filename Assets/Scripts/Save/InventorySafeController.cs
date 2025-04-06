using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySafeController : MonoBehaviour
{
    [SerializeField] private InventorySafe inventory;
    // Start is called before the first frame update
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
        }

    }
    public void setsMissions(List<Mission> misions)
    {
        inventory.setMissions(misions);
    }
    public List<Mission> getMissions()
    {
        return inventory.getMissions();
    }
    public void addMoney(int money)
    {
        inventory.addMoney(money);
    }
    public void setMission(Mission mission,int index)
    {
        inventory.changeMission(mission,index);
    }
    public void addInventory(List<ItemInInventory> items)
    {
        foreach (ItemInInventory item in items)
        {
            inventory.addItem(item.item, item.quantity);
        }
    }

    public bool hasItem(Items item,int quantity)
    {
        return inventory.hasEnough(item,quantity);
    }

    public void removeItem(Items item, int quantity)
    {
        inventory.removeItem(item,quantity);
    }
    public void Reset()
    {
        inventory.resetMoney();
        inventory.clearLoot();
        inventory.clearMissions();
    }




}
