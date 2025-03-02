using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas[] ArrayCanvas;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& SceneManager.GetActiveScene().buildIndex == 3)
        {
            ShowPauseCanvas();
        }
    }

    private void Start()
    {
        HideAllCanvas();
        ShowMainMenuCanvas();
    }

    public void ShowMainMenuCanvas()
    {
        ShowMainMenu();
        AudioManager.Instance.PlayMenuMusic();
    }

    public void BackToMainMenuCanvas()
    {
        ShowMainMenu();
        HideCredits();
        HideSettings();
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowCreditCanvas()
    {
        HideMainMenu();
        ShowCredits();
        AudioManager.Instance.PlaySFXClick();

    }

    public void ShowSettingsCanvas()
    {
        HideMainMenu();
        ShowSettings();
        AudioManager.Instance.PlaySFXClick();

    }

    public void ShowMapCanvas()
    {;
        ShowMap();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void ShowPauseCanvas()
    {
        HideMap();
        HideChamber();
        ShowPause();
        AudioManager.Instance.PlaySFXClick();
        Debug.Log("Enseño pausa");
    }
    public void ShowChamberCanvas()
    {
        HideMap();
        ShowChamber();
        AudioManager.Instance.PlaySFXOpen();
    }

    public void ReturnSettings()
    {
        HideSettings();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        HideChamber();
        AudioManager.Instance.PlaySFXClose();

    }

    public void ReturnPause()
    {
        HidePause();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        AudioManager.Instance.PlaySFXClose();

    }

    public void BackToPreviousScene()
    {
        HideMapCanvases();
        AudioManager.Instance.PlaySFXSelect();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (activeSceneIndex)
        {
            case 2:
                EnterLobbyScene();
                break;

            case 3:
                ExitChamberScene();
                break;

            default:
                break;
        }
        SceneManager.LoadScene(activeSceneIndex - 1);
    }
    public void BackToLobby()
    {
        HideMapCanvases();
        AudioManager.Instance.PlaySFXSelect();
        SceneManager.LoadScene("Lobby");
    }
    public void EnterLobbyScene()
    {
        HideMainMenuCanvases();
        AudioManager.Instance.PlaySFXConfirmation();
        SceneManager.LoadScene("Lobby");
    }
    public void EnterMapScene()
    {
        SceneManager.LoadScene("Map");
        AudioManager.Instance.PlayMapMusic();
        ShowMapCanvas();
    }

    public void EnterChamberScene()
    {
        SceneManager.LoadScene("Chamber");
        AudioManager.Instance.PlayChamberMusic();
        ShowChamberCanvas();
    }

    public void ExitChamberScene()
    {
        SceneManager.LoadScene("Map");
        AudioManager.Instance.PlayMapMusic();
        ShowMapCanvas();
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

    private void ShowCanvasByIndex(int canvasIndex)
    {
        if (canvasIndex >= 0 && canvasIndex < ArrayCanvas.Length)
        {
            ArrayCanvas[canvasIndex].gameObject.SetActive(true);

        }
        else
        {
            Debug.LogWarning("Canvas index out of range.");
        }
    }

    private void HideAllCanvas()
    {
        Debug.Log("Cierro todos los canvas");
        int i = 0;
        if (i >= 0 && i < ArrayCanvas.Length)
        {
            ArrayCanvas[i].gameObject.SetActive(false);

        }
        else
        {
            Debug.LogWarning("Canvas index out of range.");
        }
    }

    private void HideCanvasByIndex(int canvasIndex)
    {
        if (canvasIndex >= 0 && canvasIndex < ArrayCanvas.Length)
        {
            ArrayCanvas[canvasIndex].gameObject.SetActive(false);

        }
        else
        {
            Debug.LogWarning("Canvas index out of range.");
        }
    }

    private void HideMainMenu()
    {
        HideCanvasByIndex(0);
    }
    private void ShowMainMenu()
    {
        ShowCanvasByIndex(0);
    }

    private void HideLobby()
    {
        HideCanvasByIndex(1);
    }
    private void ShowLobby()
    {
        ShowCanvasByIndex(1);
    }
    private void HideMap()
    {
        HideCanvasByIndex(2);
    }
    private void ShowMap()
    {
        ShowCanvasByIndex(2);
    }

    private void HideChamber()
    {
        HideCanvasByIndex(3);
    }
    private void ShowChamber()
    {
        ShowCanvasByIndex(3);
    }

    private void HideSettings()
    {
        HideCanvasByIndex(4);
    }
    private void ShowSettings()
    {
        ShowCanvasByIndex(4);
    }

    private void HideCredits()
    {
        HideCanvasByIndex(5);
    }
    private void ShowCredits()
    {
        ShowCanvasByIndex(5);
    }

    private void HidePause()
    {
        HideCanvasByIndex(6);
    }

    private void ShowPause()
    {
        ShowCanvasByIndex(6);
    }
    private void HideEndGame()
    {
        HideCanvasByIndex(7);
    }

    private void ShowEndGame()
    {
        ShowCanvasByIndex(7);
    }

    private void HideMainMenuCanvases()
    {
        HideMainMenu();
        HideCredits();
    }

    private void HideMapCanvases()
    {
        HideMap();
        HidePause();
    }
}
