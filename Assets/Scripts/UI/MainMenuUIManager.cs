using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas[] ArrayCanvas;
    [SerializeField] private HealthManaUIManager healthManaUIManager;
    [SerializeField] private WeaponSelectionUIManager weaponSelectionUIManager;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Se intento crear una segunda instancia del objeto UI Manager");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& SceneManager.GetActiveScene().buildIndex == 3)
        {
            ShowPauseCanvas();
        }
        if (Input.GetKeyDown(KeyCode.M) && SceneManager.GetActiveScene().buildIndex == 3)
        {
            ShowEndGameCanvas();
        }
    }

    private void Start()
    {
        HideAllCanvas();
        ShowMainMenu();
        AudioManager.Instance.PlayMenuMusic();
    }

    public void ShowMainMenuCanvas()
    {
        HideAllCanvas();
        ShowMainMenu();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void BackToMainMenuCanvas()
    {
        HideMainMenuCanvases();
        ShowMainMenu();
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowLobbyCanvas()
    {
        HideAllCanvas();
        ShowLobby();
        AudioManager.Instance.PlaySFXConfirmation();
    }
    public void BackToLobbyCanvas()
    {
        HideLobbyCanvases();
        ShowLobby();
        AudioManager.Instance.PlaySFXClose();
    }

    public bool LobbyOutlinesState()
    {
        return ArrayCanvas[1].gameObject.activeSelf;
    }

    public void ShowMapCanvas()
    {
        HideAllCanvas();
        ShowMap();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void BackToMapCanvas()
    {
        HideMapCanvases();
        ShowMap();
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowChamberCanvas()
    {
        HideAllCanvas();
        ShowChamber();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void ShowPauseCanvas()
    {
        Time.timeScale = 0; // Pausar el juego
        ShowPause();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ReturnFromPause()
    {
        Time.timeScale = 1; // Reanudar el juego
        HidePause();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowSettingsCanvas()
    {
        ShowSettings();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ReturnFromSettings()
    {
        HideSettings();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        AudioManager.Instance.PlaySFXClose();
        Debug.Log("Vuelvo desde opciones a donde estaba antes");
    }

    public void ShowCreditsCanvas()
    {
        HideMainMenu();
        ShowCredits();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowMisionCanvas()
    {
        HideLobby();
        ShowMissions();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowHelpCanvas()
    {
        HideLobby();
        ShowHelp();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowWeaponsCanvas()
    {
        HideLobby();
        ShowWeapon();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowUpgradesCanvas()
    {
        HideLobby();
        ShowUpgrades();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowAchievementsCanvas()
    {
        HideLobby();
        ShowAchievements();
        AudioManager.Instance.PlaySFXClick();
    }
    public void ShowEndGameCanvas()
    {
        HideAllCanvas();
        ShowEndGame();
        AudioManager.Instance.PlayEndGameMusic();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Player Stats UI
    public void SetMaxHealth(float health)
    {
        healthManaUIManager.SetMaxHealth(health);
    }

    public void SetHealth(float health)
    {
        healthManaUIManager.SetHealth(health);
    }

    public void SetMaxMana(float mana)
    {
        healthManaUIManager.SetMaxMana(mana);
    }

    public void SetMana(float mana)
    {
        healthManaUIManager.SetMana(mana);
    }

    public void RegenMana(float manaRegenRate)
    {
        healthManaUIManager.RegenMana(manaRegenRate);
    }

    public void GainMana(float manaGain)
    {
        healthManaUIManager.GainMana(manaGain);
    }

    public void GainHealth(float healthgain)
    {
        healthManaUIManager.GainHealth(healthgain);
    }

    public void LoseMana(float manalost)
    {
        healthManaUIManager.LoseMana(manalost);
    }

    public void LoseHealth(float healthlost)
    {
        healthManaUIManager.LoseHealth(healthlost);
    }

    #endregion

    #region WeaponSelector
    public void ShowPrevWeapon()
    {
        Debug.Log("Funciona boton");
        weaponSelectionUIManager.ShowPreviousWeapon();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ShowNextWeapon()
    {
        weaponSelectionUIManager.ShowNextWeapon();
        AudioManager.Instance.PlaySFXClick();
    }

    public void SelectWeapon()
    {
        weaponSelectionUIManager.PlayerSelectedWeapon();
        GameManager.Instance.EnterMapScene();
    }

    #endregion 

    #region Scene Handling

    public void BackToPreviousScene()
    {
        AudioManager.Instance.PlaySFXSelect();
        HidePause();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (activeSceneIndex)
        {
            case 2:
                GameManager.Instance.EnterLobbyScene();
                break;

            case 3:
                GameManager.Instance.EnterMapScene();
                break;

            case 1:
                GameManager.Instance.EnterMainMenuScene();
                break;

            default:
                break;
        }
    }

    #endregion

    #region Show Hide By
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
    #endregion

    #region Hide Show Specific Canvas
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
    private void HideMissions()
    {
        HideCanvasByIndex(7);
    }

    private void ShowMissions()
    {
        ShowCanvasByIndex(7);
    }
    private void HideHelp()
    {
        HideCanvasByIndex(8);
    }

    private void ShowHelp()
    {
        ShowCanvasByIndex(8);
    }
    private void HideWeapon()
    {
        HideCanvasByIndex(9);
    }

    private void ShowWeapon()
    {
        ShowCanvasByIndex(9);
    }
    private void HideUpgrades()
    {
        HideCanvasByIndex(10);
    }

    private void ShowUpgrades()
    {
        ShowCanvasByIndex(10);
    }
    private void HideAchievements()
    {
        HideCanvasByIndex(11);
    }

    private void ShowAchievements()
    {
        ShowCanvasByIndex(11);
    }
    private void HideEndGame()
    {
        HideCanvasByIndex(12);
    }

    private void ShowEndGame()
    {
        ShowCanvasByIndex(12);
    }
    #endregion 

    #region Hide Show Groups
    private void HideMainMenuCanvases()
    {
        HideCredits();
    }
    private void HideLobbyCanvases()
    {
        HideAchievements();
        HideMissions();
        HideUpgrades();
        HideWeapon();
        HideHelp();
    }

    private void HideMapCanvases()
    {
        HideMap();
        HidePause();
    }
    private void HideAllCanvas()
    {
        for (int i = 0; i >= 0 && i < ArrayCanvas.Length; i++)
        {
            ArrayCanvas[i].gameObject.SetActive(false);
        }
    }

    #endregion 
}
