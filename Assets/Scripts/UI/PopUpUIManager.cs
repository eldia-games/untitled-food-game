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
    private class ChamberInfo
    {
        public int panelID;
        public string panelText;
        public string descriptionText;
    }

    [SerializeField] private PanelInfo[] panels;
    [SerializeField] private ChamberInfo[] chambers;
    [SerializeField] private Image spriteDisplayImage;
    [SerializeField] private TMP_Text helpPopUp;
    [SerializeField] private TMP_Text chamberPopUp;
    [SerializeField] private TMP_Text descriptionPopUp;

    public void displaypopUpHelp(string action,bool affirmative)
    {
        if (affirmative)
        {
            spriteDisplayImage.sprite = panels[0].panelImage;
            helpPopUp.text = panels[0].panelText + action.ToLower() + ".";
        }
        else
        {
            spriteDisplayImage.sprite = panels[1].panelImage;
            helpPopUp.text = panels[1].panelText + action.ToLower() + ".";
        }
    }

    public void displayPopUpChamber(RoomType room)
    {
        chamberPopUp.text = chambers[(int)room].panelText;
        descriptionPopUp.text = chambers[(int)room].panelText;
    }

}
