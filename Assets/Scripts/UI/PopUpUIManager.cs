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

    [System.Serializable]
    private class PowerupInfo
    {
        public int ID;
        public string powerupText;
        public Sprite powerupImage;
    }

    [SerializeField] private PanelInfo[] panels;
    [SerializeField] private ChamberInfo[] chambers;
    [SerializeField] private PowerupInfo[] powerups;

    [SerializeField] private Image spriteDisplayImage;
    [SerializeField] private TMP_Text helpPopUp;
    [SerializeField] private TMP_Text chamberPopUp;
    [SerializeField] private TMP_Text descriptionPopUp;
    [SerializeField] private GameObject chamberPopUpGameObject;
    [SerializeField] private GameObject helpPopUpGameObject;
    
    [SerializeField] private GameObject powerupGameObject;
    [SerializeField] private TMP_Text powerupTMP;
    [SerializeField] private Image powerupSprite;

    private void Awake()
    {
        hideHelpPopUp();
        hidePowerUpPopUp();

    }
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

    public void displayPowerUp(powerUpType powerUp)
    {
        powerupGameObject.SetActive(true);
        powerupTMP.text = powerups[(int)powerUp].powerupText;
        powerupSprite.sprite = powerups[(int)powerUp].powerupImage;
    }

    public void hidePopUps()
    {
        chamberPopUpGameObject.SetActive(false);
        helpPopUpGameObject.SetActive(false);
        powerupGameObject.SetActive(false);
    }
    public void hideHelpPopUp()
    {
        helpPopUpGameObject.SetActive(false);
    }
    public void hideChamberPopUp()
    {
        chamberPopUpGameObject.SetActive(false);
    }
    public void hidePowerUpPopUp()
    {
        powerupGameObject.SetActive(false);
    }
}
