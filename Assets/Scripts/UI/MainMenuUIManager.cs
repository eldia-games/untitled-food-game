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
        hideAllCanvas();
        ShowMainMenuCanvas();
    }

    public void ShowMainMenuCanvas()
    {
        showMainMenu();
        AudioManager.Instance.PlayMenuMusic();
    }

    public void BackToMainMenuCanvas()
    {
        showMainMenu();
        hideCredits();
        hideSettings();
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowCreditCanvas()
    {
        hideMainMenu();
        showCredits();
        AudioManager.Instance.PlaySFXClick();

    }

    public void ShowSettingsCanvas()
    {
        hideMainMenu();
        showSettings();
        AudioManager.Instance.PlaySFXClick();

    }

    public void ShowMapCanvas()
    {;
        showMap();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void ShowPauseCanvas()
    {
        hideMap();
        hideChamber();
        showPause();
        AudioManager.Instance.PlaySFXClick();
        Debug.Log("Enseño pausa");
    }
    public void ShowChamberCanvas()
    {
        hideMap();
        showChamber();
        AudioManager.Instance.PlaySFXOpen();
    }

    public void ReturnSettings()
    {
        hideSettings();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        showCanvasByIndex(activeSceneIndex);
        AudioManager.Instance.PlaySFXClose();

    }

    public void ReturnPause()
    {
        hidePause();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        showCanvasByIndex(activeSceneIndex);
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

    private void showCanvasByIndex(int canvasIndex)
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

    private void hideAllCanvas()
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

    private void hideCanvasByIndex(int canvasIndex)
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

    private void hideMainMenu()
    {
        hideCanvasByIndex(0);
    }
    private void showMainMenu()
    {
        showCanvasByIndex(0);
    }

    private void hideLobby()
    {
        hideCanvasByIndex(1);
    }
    private void showLobby()
    {
        showCanvasByIndex(1);
    }
    private void hideMap()
    {
        hideCanvasByIndex(2);
    }
    private void showMap()
    {
        showCanvasByIndex(2);
    }

    private void hideChamber()
    {
        hideCanvasByIndex(3);
    }
    private void showChamber()
    {
        showCanvasByIndex(3);
    }

    private void hideSettings()
    {
        hideCanvasByIndex(4);
    }
    private void showSettings()
    {
        showCanvasByIndex(4);
    }

    private void hideCredits()
    {
        hideCanvasByIndex(5);
    }
    private void showCredits()
    {
        showCanvasByIndex(5);
    }

    private void hidePause()
    {
        hideCanvasByIndex(6);
    }

    private void showPause()
    {
        showCanvasByIndex(6);
    }
    private void hideEndGame()
    {
        hideCanvasByIndex(7);
    }

    private void showEndGame()
    {
        showCanvasByIndex(7);
    }

    private void HideMainMenuCanvases()
    {
        hideMainMenu();
        hideCredits();
    }

    private void HideMapCanvases()
    {
        hideMap();
        hidePause();
    }
}
