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

    [SerializeField] private PanelInfo[] panels;
    [SerializeField] private Image spriteDisplayImage;
    [SerializeField] private TMP_Text textPopUp;

    public void displayUI(string action,bool active)
    {
        spriteDisplayImage.sprite = panels[0].panelImage;
        if (active)
        {
            textPopUp.text = panels[0].panelText + " " + action;
        }
        else
        {
            textPopUp.text = panels[1].panelText + " " + action;
        }
        print("panel index: " + 0);
    }

}
