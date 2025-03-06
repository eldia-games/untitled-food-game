using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IPointerClickHandler
{
    public DropsList dropsList;

    public List<GameObject> lootTable => dropsList.posibleLootTablePrefabs;

    public static InventoryManager Instance { get; private set; }

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

        slots = new GameObject[slotsHolder.transform.childCount];
    }
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] public Items itemToAdd;
    [SerializeField] public Items itemToRemove;
    [SerializeField] public int GameObjectToRemove;

    [SerializeField] public float money;

    [SerializeField] public GameObject moneyHolder;

    private InputHandler _handler;
    
    public List<ItemInInventory> items;
    public GameObject[] slots;

    public GameObject player;
    void Update()
    {
        if(_handler.inventory)
        {
            Time.timeScale = 0.0f;
            //enable the inventory 
            slotsHolder.GetComponentInParent<Canvas>().enabled = true;
            //disable PlayerCombat
            //this.GameObject().GetComponent<PlayerCombat>().enabled = false;
        }
        else
        {
            Time.timeScale = 1.0f;
            //disable
            slotsHolder.GetComponentInParent<Canvas>().enabled = false;
            //this.GameObject().GetComponent<PlayerCombat>().enabled = true;
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
    public void setPlayer(GameObject player)
    {
        this.player = player;
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

        moneyHolder.GetComponent<TextMeshProUGUI>().text = money.ToString();
    }
    
    public void AddItem(Items item, int prefab, int quantity, bool stackeable)
    {
        itemToAdd = item;
        GameObjectToRemove = prefab;
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
            items.Add(new ItemInInventory(itemToAdd,prefab,quantity));
        RefreshUI();
    }

    public void RemoveItem(Items item, int quantity)
    {
        if (player != null) {
            itemToRemove = item;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == item)
                {
                    Instantiate(
                        lootTable[items[i].prefab],
                        transform.position + player.transform.forward * 2 + transform.up,
                        Quaternion.Euler(-90, player.transform.eulerAngles.y, 0));

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

    public void ButtonSellItem()
    {

        // Assuming you have a reference to the selected item and its quantity
        Items selectedItem = itemToRemove; // This should be set to the item you want to sell
        int selectedQuantity = 1; // This should be set to the quantity you want to sell

        // Show a confirmation popup
        bool confirmSell = ShowConfirmationPopup("Do you want to sell this item?");
        if (confirmSell)
        {
            // Remove the item from the inventory
            RemoveItem(selectedItem, selectedQuantity);

            // Add money to the player's total
            money += selectedItem.gold * selectedQuantity;

            // Refresh the UI to reflect changes
            RefreshUI();
        }
    }

    private bool ShowConfirmationPopup(string message)
    {
        // Implement your popup logic here
        // For now, we'll just return true to simulate a confirmation
        print(message);
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        GameObject clickedSlot = eventData.pointerEnter;
        int i = System.Array.IndexOf(slots, clickedSlot);

        Debug.Log("Click on slot: "+i);

        if(i!=0)
        {
            itemToRemove = items[i].item;

            RemoveItem(itemToRemove, 1);
        }
       
    }
}