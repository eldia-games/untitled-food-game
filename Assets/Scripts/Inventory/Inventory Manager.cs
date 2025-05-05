using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IPointerClickHandler
{
    //public DropsList dropsList;


    //public List<GameObject> lootTable => dropsList.posibleLootTablePrefabs;

    [SerializeField] private GameObject slotsHolder;
    //[SerializeField] public Items itemToAdd;
    //[SerializeField] public Items itemToRemove;
    // [SerializeField] public int GameObjectToRemove;

    //[SerializeField] public float money;

    //[SerializeField] public GameObject moneyHolder;


    private GameObject[] slots;

   // public GameObject player;

    private InputHandler _handler;
    private bool inventoryOpened = true;




   


    void Update()
    {
        if(_handler.inventory )
        {

            RefreshUI();
            inventoryOpened = true;
            UIManager.Instance.pauseLocked = true;
            Time.timeScale = 0.0f;
            //enable the inventory 
            slotsHolder.GetComponentInParent<Canvas>().enabled = true;
            //disable PlayerCombat
            //this.GameObject().GetComponent<PlayerCombat>().enabled = false;
            
        }
        else if(inventoryOpened)
        {
            inventoryOpened = false;
            UIManager.Instance.pauseLocked = false;
            Time.timeScale = 1.0f;
            //disable
            slotsHolder.GetComponentInParent<Canvas>().enabled = false;
            //this.GameObject().GetComponent<PlayerCombat>().enabled = true;
        }
    }

    private void Start()
    {
        slots = new GameObject[slotsHolder.transform.childCount];
        _handler = GetComponent<InputHandler>();
        for (int i = 0; i < slotsHolder.transform.childCount; i++)
        {
            slots[i] = slotsHolder.transform.GetChild(i).gameObject;
        }
    }

    public void RefreshUI()
    {
        List<ItemInInventory> items = InventoryList.Instance.getItems();
        for (int i = 0; i < slots.Length; i++)
        {

            try
            {
                if (i < items.Count)
                {
                    slots[i].GetComponent<RawImage>().enabled = true;
                    slots[i].GetComponent<RawImage>().texture = items[i].item.icon;
                    slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    slots[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].quantity.ToString();
                }
                else
                {
                    slots[i].GetComponent<RawImage>().enabled = false;
                    slots[i].GetComponent<RawImage>().texture = null;
                    slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                }
            }
            catch
            {
                print("error "+i);
                slots[i].GetComponent<RawImage>().enabled = false;
                slots[i].GetComponent<RawImage>().texture = null;
                slots[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
        }

        //moneyHolder.GetComponent<TextMeshProUGUI>().text = money.ToString();
    }
    

  
    public void AddItem(Items item, int prefab, int quantity, bool stackeable)
    {
       InventoryList.Instance.AddItem(item, prefab, quantity, stackeable);
        RefreshUI();
    }

    //public void RemoveItem(Items item, int quantity)
    //{
    //    if (player != null) {
    //        itemToRemove = item;
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].item == item)
    //            {
    //                Instantiate(
    //                    lootTable[items[i].prefab],
    //                    player.transform.position + player.transform.forward * 2 + transform.up,
    //                    Quaternion.Euler(-90, player.transform.eulerAngles.y, 0));

    //                var tempItem = items[i];
    //                tempItem.quantity -= quantity;
    //                if (tempItem.quantity <= 0)
    //                {
    //                    items.RemoveAt(i);
    //                }
    //                else
    //                {
    //                    items[i] = tempItem;
    //                }

    //                break;
    //            }
    //        }
    //        RefreshUI();
    //    }
    //}

    public void RemoveItemNoDrop(Items item, int quantity)
    {
        InventoryList.Instance.RemoveItemNoDrop(item, quantity);
        RefreshUI();
        //}
    }

    public bool HasItems(Items item, int quantity)
    {

        return InventoryList.Instance.HasItems(item, quantity);
    }
    //public void UseItem(Items item, int quantity)
    //{
    //   // if (player != null) {
    //        itemToRemove = item;
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].item == item)
    //            {
    //                var tempItem = items[i];
    //                tempItem.quantity -= quantity;
    //                if (tempItem.quantity <= 0)
    //                {
    //                    items.RemoveAt(i);
    //                }
    //                else
    //                {
    //                    items[i] = tempItem;
    //                }

    //                break;
    //            }
    //        }
    //        RefreshUI();
    //    //}
    //}

    //public void ButtonSellItem()
    //{

    //    // Assuming you have a reference to the selected item and its quantity
    //    Items selectedItem = itemToRemove; // This should be set to the item you want to sell
    //    int selectedQuantity = 1; // This should be set to the quantity you want to sell

    //    // Show a confirmation popup
    //    bool confirmSell = ShowConfirmationPopup("Do you want to sell this item?");
    //    if (confirmSell)
    //    {
    //        // Remove the item from the inventory
    //        RemoveItem(selectedItem, selectedQuantity);

    //        // Add money to the player's total
    //        money += selectedItem.gold * selectedQuantity;

    //        // Refresh the UI to reflect changes
    //        RefreshUI();
    //    }
    //}

    private bool ShowConfirmationPopup(string message)
    {
        // Implement your popup logic here
        // For now, we'll just return true to simulate a confirmation
        print(message);
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        //GameObject clickedSlot = eventData.pointerEnter;
        //int i = System.Array.IndexOf(slots, clickedSlot);

        //Debug.Log("Click on slot: "+i);

        //itemToRemove = items[i].item;

        //if (items[i].item.itemName == "Beer")
        //{
        //    player.GetComponentInParent<PlayerCombat>().OnHeal();
        //}
        //RefreshUI();
        //RemoveItem(itemToRemove, 1);
        
       
    }
}