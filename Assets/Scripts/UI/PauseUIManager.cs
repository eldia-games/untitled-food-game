using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text boxText;

    public void RefreshPauseUI()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (activeSceneIndex)
        {
            case 1:
                boxText.text = "Return to main menu";
                break;

            case 2:
                boxText.text = "Return to tavern without loot";
                break;

            default:
                boxText.text = "Return to tavern without loot";
                break;
        }
    }

}
