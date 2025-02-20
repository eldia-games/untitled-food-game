using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Canvas LobbyCanvas;
    [SerializeField] private Canvas MapCanvas;
    [SerializeField] private Canvas CombatCanvas;
    [SerializeField] private Canvas MisionCanvas;
    [SerializeField] private Canvas UpgradesCanvas;
    [SerializeField] private Canvas CreditCanvas;


    private void Start()
    {
        ShowMainMenuCanvas();
    }

    public void ShowMainMenuCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(true);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(false);
    }
    public void ShowCreditsUI()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(true);
    }
    public void ShowLobbyCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(true);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(false);
    }

    public void ShowMapCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(true);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(false);
    }

    public void ShowCombatCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(true);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(false);
    }
    public void ShowMisionCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(true);
        UpgradesCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(false);
    }
    public void ShowUpgradeCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        LobbyCanvas.gameObject.SetActive(false);
        MapCanvas.gameObject.SetActive(false);
        CombatCanvas.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(true);
        CreditCanvas.gameObject.SetActive(false);
    }
}
