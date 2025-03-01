using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Canvas CreditCanvas;

    private void Start()
    {
        ShowMainMenuCanvas();
    }

    public void ShowMainMenuCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(true);
        CreditCanvas.gameObject.SetActive(false);
    }

    public void ShowCreditCanvas()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        CreditCanvas.gameObject.SetActive(true);
    }

    public void EnterLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEngine.Debug.Log("Salgo del juego");
#else
        // Si estamos en una build, cerramos la aplicación
        OnExitGame?.Invoke();
        Application.Quit();
#endif
    }
}
