using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.UI;
using System.Diagnostics;

public class Game : MonoBehaviour
{
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreditsButton;
    [SerializeField] private Button ExitButton;

    public static event Action OnStartGame;
    public static event Action OnReturnMenu;
    public static event Action OnReturnTavern;
    public static event Action OnShowCredits;
    public static event Action OnStartMap;
    public static event Action OnReturnMap;
    public static event Action OnStartRoom;
    public static event Action OnOpenMisions;
    public static event Action OnOpenUpgrades;
    public static event Action OnExitGame;

    public UIManager uiManager;

    // Start is called before the first frame update
    private void Awake()
    {
        UnityEngine.Debug.Log("Se ejecuta el juego en unity");
        StartGameButton.onClick.AddListener(StartGame);
        CreditsButton.onClick.AddListener(ShowCredits);
        ExitButton.onClick.AddListener(ExitGame);
    }

    // Update is called once per frame
    private void StartGame()
    {
        OnStartGame?.Invoke();
        UnityEngine.Debug.Log("Comenzar juego en el lobby");
        uiManager.ShowLobbyCanvas();
    }

    public void ReturnMenu()
    {
        OnReturnMenu?.Invoke();
        UnityEngine.Debug.Log("Volver al menu");
        uiManager.ShowMainMenuCanvas();
    }

    public void ReturnTavern()
    {
        OnReturnTavern?.Invoke();
        UnityEngine.Debug.Log("Volver a taberna");
        uiManager.ShowLobbyCanvas();
    }
    public void ShowCredits()
    {
        OnShowCredits?.Invoke();
        UnityEngine.Debug.Log("Enseño creditos");
        uiManager.ShowCreditsUI();
        
    }
    public void StartMap()
    {
        OnStartMap?.Invoke();
        UnityEngine.Debug.Log("Salgo y genero un mapa");
        uiManager.ShowMapCanvas();
    }

    public void ReturnMap()
    {
        OnStartMap?.Invoke();
        UnityEngine.Debug.Log("Vuelvo al mapa");
        uiManager.ShowMapCanvas();
    }
    public void StarRoom()
    {
        OnStartRoom?.Invoke();
        UnityEngine.Debug.Log("Entro en una sala generada");
        uiManager.ShowCombatCanvas();

    }
    public void OpenUpgrades()
    {
        OnOpenUpgrades?.Invoke();
        UnityEngine.Debug.Log("Abro las mejoras");
        uiManager.ShowUpgradeCanvas();  

    }
    public void OpenMisions()
    {
        OnOpenMisions?.Invoke();
        UnityEngine.Debug.Log("Abro las misiones");
        uiManager.ShowMisionCanvas();   

    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        OnExitGame?.Invoke();
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEngine.Debug.Log("Salgo del juego");
#else
        // Si estamos en una build, cerramos la aplicación
        OnExitGame?.Invoke();
        Application.Quit();
#endif
    }
}
