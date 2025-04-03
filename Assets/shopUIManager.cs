using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class shopUIManager : MonoBehaviour
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
}
