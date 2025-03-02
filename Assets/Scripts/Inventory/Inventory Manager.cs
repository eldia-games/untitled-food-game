using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private Items itemToAdd;
    [SerializeField] private Items itemToRemove;

    private InputHandler _handler;
    
    public List<ItemInInventory> items;
    public GameObject[] slots;

    private void Awake()
    {
        items = new List<ItemInInventory>();
        slots = new GameObject[slotsHolder.transform.childCount];
    }

    void Update()
    {
        if(_handler.inventory)
        {
            //enable the inventory 
            slotsHolder.GetComponentInParent<Canvas>().enabled = true;
        }
        else
        {
            //disable
            slotsHolder.GetComponentInParent<Canvas>().enabled = false;
        }
    }

    private void Start()
    {
        _handler = GetComponent<InputHandler>();
        for (int i = 0; i < slotsHolder.transform.childCount; i++)
        {
            slots[i] = slotsHolder.transform.GetChild(i).gameObject;
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                if (i < items.Count)
                {
                    slots[i].GetComponent<RawImage>().enabled = true;
                    slots[i].GetComponent<RawImage>().texture = items[i].item.icon;
                    slots[i].GetComponentInChildren<Text>().enabled = true;
                    slots[i].GetComponentInChildren<Text>().text = items[i].quantity.ToString();
                }
                else
                {
                    slots[i].GetComponent<RawImage>().enabled = false;
                    slots[i].GetComponent<RawImage>().texture = null;
                    slots[i].GetComponentInChildren<Text>().enabled = false;
                }
            }
            catch
            {
                print("error "+i);
                slots[i].GetComponent<RawImage>().enabled = false;
                slots[i].GetComponent<RawImage>().texture = null;
                slots[i].GetComponentInChildren<Text>().enabled = false;
            }
        }
    }
    
    public void AddItem(Items item, int quantity, bool stackeable)
    {
        itemToAdd = item;
        bool newItem = true;
        if(stackeable){
            for(int i = 0; i<items.Count(); i++)
            {
                if(items[i].item == item)
                {
                    newItem = false;
                    var tempItem = items[i];
                    tempItem.quantity += quantity;
                    items[i] = tempItem;
                    break;
                }
            }
        }

        if(newItem==true)
            items.Add(new ItemInInventory(itemToAdd,quantity));
        RefreshUI();
    }

    public void RemoveItem(Items item, int quantity)
    {
        itemToRemove = item;
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
        RefreshUI();
    }
}
