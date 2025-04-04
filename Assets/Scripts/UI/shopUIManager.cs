using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    [System.Serializable]
    private class ItemlInfo
    {
        public int panelID;
        public string itemText;
        public Sprite itemImage;
    }

    [SerializeField] private ItemlInfo[] items;
    [SerializeField] private Image spriteDisplayImage;
    [SerializeField] private TMP_Text textItemName;
    [SerializeField] private TMP_Text textItemQuantity;

    public void displayItems(string action)
    {
        // for recorriendo todas las casillas tienda
        spriteDisplayImage.sprite = items[0].itemImage;
        textItemName.text = items[0].itemText;
        textItemQuantity.text = items[0].itemText; // preguntar a inventario cnatidad
        print("panel index: " + 0);
    }

    private Sprite findItemSprite(string item)
    {
        return spriteDisplayImage.sprite;
    }
    private string findeItemName()
    {
        return "Ejemplo"; //alamcenar nombre de items con sus sprites correspondientes

    }

    private int findItemQuantity(string itemName)
    {
        return 1; //llamar inventario
    }
    [SerializeField]
    [SerializeField] private GameObject tradesHolder;
    public GameObject[] trades;
    public Dictionary<ItemInInventory> itemDictionary;
    [SerializeField] public Items itemToAdd;
    [SerializeField] public Items itemToRemove;

    public void RefreshShopUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            itemTrades[i].GetComponent<Image>().sprite = items[i].item.icon;
            itemTrades[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].quantity.ToString();
            itemTrades[i].GetComponent<Image>().sprite = items[i].item.icon;
            itemTrades[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].quantity.ToString();
        }
    }
    //public void confirmTrade(int tradeNumber)
    //{
    //    switch{
    //        case 1:
    //            InventoryManager.Instance.AddItem(itemTrades[1], 1, itemTrades[], true);
    //            InventoryManager.Instance.RemoveItem(itemTrades[2], 1, itemTrades[], true);
    //    }
    //}
}
