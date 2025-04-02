using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpUIManager : MonoBehaviour
{
    [System.Serializable]
    private class PanelInfo
    {
        public int panelID;
        public string panelText;
        public Sprite panelImage;
    }

    private Dictionary<string, int> panelIndex = new Dictionary<string, int>()
    {
        {"Lever", 0},
        {"Door", 1},
        {"Chest", 2}
    };

    [SerializeField] private PanelInfo[] panels;
    [SerializeField] private Image spriteDisplayImage;
    [SerializeField] private TMP_Text textPopUp;

    public void displayUI(string action)
    {
        spriteDisplayImage.sprite = panels[0].panelImage;
        textPopUp.text = panels[0].panelText + " " + action;
        print("panel index: " + 0);
    }

   //public void useLeverPanel()
   //{
   //    displayUI(panelIndex["Lever"]);
   //}
   //
   //public void useDoorPanel()
   //{
   //    displayUI(panelIndex["Door"]);
   //}
   //
   //public void useChestPanel()
   //{
   //    displayUI(panelIndex["Chest"]);
   //}


}
