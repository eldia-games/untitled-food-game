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
    [System.Serializable]
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
    [SerializeField] private GameObject chamberPopUpGameObject;
    [SerializeField] private GameObject helpPopUpGameObject;

    public void displaypopUpHelp(string action,bool affirmative)
    {
        helpPopUpGameObject.SetActive(true);
        if (affirmative)
        {
            spriteDisplayImage.sprite = panels[0].panelImage;
            helpPopUp.text = panels[0].panelText + " " + action.ToLower() + ".";
        }
        else
        {
            spriteDisplayImage.sprite = panels[1].panelImage;
            helpPopUp.text = panels[1].panelText + " " + action.ToLower() + ".";
        }
    }

    public void displayPopUpChamber(RoomType room)
    {
        chamberPopUpGameObject.SetActive(true);
        chamberPopUp.text = chambers[(int)room].panelText;
        descriptionPopUp.text = chambers[(int)room].descriptionText;
    }

    public void hidePopUps()
    {
        chamberPopUpGameObject.SetActive(false);
        helpPopUpGameObject.SetActive(true);
    }

}
