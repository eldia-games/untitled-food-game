using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpSelectionUIManager : MonoBehaviour
{
    [System.Serializable]
    private class HelpInfo
    {
        public string helpName;
        public Sprite helpImage;
        public string helpDescription;

    }
    private int currentIndex = 0;

    private void Start()
    {
        UpdateUI();
    }
    [SerializeField] private HelpInfo[] helps;
    [SerializeField] private Image helpImage;
    [SerializeField] private TMP_Text helpName;
    [SerializeField] private TMP_Text helpDescription;
    private void UpdateUI()
    {
        helpImage.sprite = helps[currentIndex].helpImage;
        helpName.text = helps[currentIndex].helpName;
        helpDescription.text = helps[currentIndex].helpDescription;
    }

    public void ShowNextWeapon()
    {
        if (currentIndex < 3)
        {
            currentIndex = (currentIndex + 1) % helps.Length;
            UpdateUI();

        }
        else
        {
            Debug.Log("Fin de armas");
        }
    }

    public void ShowPreviousWeapon()
    {
        if (currentIndex > 0)
        {
            currentIndex = (currentIndex - 1 + helps.Length) % helps.Length;
            UpdateUI();
        }
        else
        {
            Debug.Log("Inicio de armas");
        }
    }

}
