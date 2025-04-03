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
    [SerializeField] private TMP_Text textPopUp;

    public void displayItem(string action)
    {
        spriteDisplayImage.sprite = items[0].itemImage;
        textPopUp.text = items[0].itemImage + " " + action;
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
