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

    private void displayUI(int index)
    {
        spriteDisplayImage.sprite = panels[index].panelImage;
        textPopUp.text = panels[index].panelText;
        print("panel index: " + index);
    }

    public void useLeverPanel()
    {
        displayUI(panelIndex["Lever"]);
    }

    public void useDoorPanel()
    {
        displayUI(panelIndex["Door"]);
    }

    public void useChestPanel()
    {
        displayUI(panelIndex["Chest"]);
    }


}
