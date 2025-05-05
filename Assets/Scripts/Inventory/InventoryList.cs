using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryList : MonoBehaviour
{
    public static InventoryList Instance { get; private set; }

    private List<ItemInInventory> items;
    [SerializeField] private int MaxBeers;
    public int Beers;
 

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

    private void Start()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        resetItems();
        resetBeers();
    }



    #region Beers
    public void useBeer()
    {
        if (Beers > 0)
        {
            Beers--;
            PlayerStatsController.Instance.heal(PlayerStatsController.Instance.getHeal());
            UIManager.Instance.ResetPlayerUiStats();
        }
    }
    public void refillBeers()
    {
        Beers = Mathf.Max(MaxBeers, Beers);
    }

    public int getBeers()
    {
        return Beers;
    }
    public int getMaxBeers()
    {
        return MaxBeers;
    }
    public void addBeers(int beers)
    {
        Beers += beers;
    }
    public void resetBeers()
    {
        Beers=MaxBeers;
    }
    #endregion

    #region Items
    public List<ItemInInventory> getItems()
    {
        return items;
    }

    public void resetItems()
    {
        items = new List<ItemInInventory>();
    }

    public void AddItem(Items item, int prefab, int quantity, bool stackeable)
    {

        bool newItem = true;
        if (stackeable)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                if (items[i].item == item)
                {
                    newItem = false;
                    var tempItem = items[i];
                    tempItem.quantity += quantity;
                    items[i] = tempItem;
                    break;
                }
            }
        }

        if (newItem == true)
            items.Add(new ItemInInventory(item, prefab, quantity));
    }

    public void RemoveItemNoDrop(Items item, int quantity)
    {

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                var tempItem = items[i];
                tempItem.quantity -= quantity;
                if (tempItem.quantity <= 0)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    items[i] = tempItem;
                }

                break;
            }
        }
    }
    public bool HasItems(Items item, int quantity)
    {

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                var tempItem = items[i];
                if (tempItem.quantity >= quantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        return false;
    }
    public int getQuantity(Items item)
    {

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                var tempItem = items[i];
                return items[i].quantity;


            }
        }
        return 0;


    }



    #endregion
}
