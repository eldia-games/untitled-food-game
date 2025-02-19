using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Canvas LobbyCanvas;
    [SerializeField] private Canvas MapCanvas;
    [SerializeField] private Canvas CombatCanvas;
    [SerializeField] private Canvas MisionCanvas;
    [SerializeField] private Canvas UpgradesCanvas;
    [SerializeField] private Canvas BackgroundCanvas;


    private void Start()
    {
        ShowMainMenuCanvas();
    }

    void ShowMainMenuCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(true);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        BackgroundCanvas.gameObject.SetActive(true);
    }

    void ShowLobbyCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(true);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        BackgroundCanvas.gameObject.SetActive(false);
    }

    void ShowMapCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(true);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(true);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        BackgroundCanvas.gameObject.SetActive(false);
    }

    void ShowCombatCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(true);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        BackgroundCanvas.gameObject.SetActive(false);
    }
    void ShowMisionCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(true);
        UpgradesCanvas.gameObject.SetActive(false);
        BackgroundCanvas.gameObject.SetActive(true);
    }
    void ShowUpgradeCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(true);
        BackgroundCanvas.gameObject.SetActive(true);
    }
}
