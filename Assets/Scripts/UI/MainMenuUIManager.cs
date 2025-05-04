using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas[] ArrayCanvas;
    [SerializeField] private HealthManaUIManager healthManaUIManager;
    [SerializeField] private ChamberUIManager chamberUIManager;
    [SerializeField] private WeaponSelectionUIManager weaponSelectionUIManager;
    [SerializeField] private PopUpUIManager popUpUIManager;
    [SerializeField] private ShopUIManager shopUIManager;
    [SerializeField] private MissionUIManager missionUIManager;
    [SerializeField] private AchievementsUIManager achievementsUIManager;
    [SerializeField] private PauseUIManager pauseUIManager;
    [SerializeField] private UpgradesUIManager upgradesUIManager;
    public bool pauseLocked;
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
        pauseLocked = false;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape)&& SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 4)
        {
            TogglePauseCanvas();
        }
    }

    private void Start()
    {
        HideAllCanvas();
        ShowMainMenu();
        AudioManager.Instance.PlayMenuMusic();
    }
    private void pauseGame()
    {
        Time.timeScale = 0;
    }

    private void resumeGame()
    {
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Main Menu
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

    #endregion

    #region Lobby
    public void ShowLobbyCanvas()
    {
        HideAllCanvas();
        ShowLobby();
        AudioManager.Instance.PlaySFXConfirmation();
    }
    public void BackToLobbyCanvas()
    {
        pauseLocked = false;
        HideLobbyCanvases();
        ShowLobby();
        AudioManager.Instance.PlaySFXClose();
    }

    public bool LobbyOutlinesState()
    {
        return ArrayCanvas[1].gameObject.activeSelf;
    }

    #endregion

    #region Map
    public void ShowMapCanvas()
    {
        pauseLocked = false;
        HideAllCanvas();
        ShowMap();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void BackToMapCanvas()
    {
        pauseLocked = false;
        HideMapCanvases();
        ShowMap();
        AudioManager.Instance.PlaySFXClose();
    }
    #endregion

    #region Chamber
    public void ShowChamberCanvas()
    {
        pauseLocked = false;
        HideAllCanvas();
        ShowChamber();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    #endregion

    #region Basement
    public void ShowBasementCanvas()
    {
        pauseLocked = false;
        HideAllCanvas();
        ShowBasement();
    }
    
    public void HideBasementCanvas()
    {
        pauseLocked = false;
        HideBasement();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Tutorial
    public void ShowWelcomeCanvas()
    {
        pauseGame();
        HideBasement();
        ShowWelcome();
        AudioManager.Instance.PlaySFXConfirmation();
    }

    public void HideWelcomeCanvas()
    {
        HideWelcome();
        ShowBasement();
        AudioManager.Instance.PlaySFXClose();
        resumeGame();
    }

    public void AdvanceWelcomeCanvas()
    {
        HideCanvasByIndex(18);
        ShowCanvasByIndex(19);
        AudioManager.Instance.PlaySFXConfirmation();
    }

    #endregion

    #region Pause
    public void ShowPauseCanvas()
    {
        pauseUIManager.RefreshPauseUI();
        Time.timeScale = 0; // Pause the game
        ShowPause();
        AudioManager.Instance.PlaySFXClick();
    }

    public void TogglePauseCanvas()
    {
        if (!pauseLocked)
        {
            if (Time.timeScale > 0)
            {
                Time.timeScale = 0; // Pause the game
                HideLobby();
                ShowPause();
                AudioManager.Instance.PlaySFXClick();
            }
            else
            {
                Time.timeScale = 1; // Resume the game
                HidePause();
                int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                ShowCanvasByIndex(activeSceneIndex);
                AudioManager.Instance.PlaySFXClose();
            }
        }
    }

    public void ReturnFromPause()
    {
        Time.timeScale = 1; // Resume the game
        HidePause();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Settings
    public void ShowSettingsCanvas()
    {
        pauseLocked = true;
        ShowSettings();
        AudioManager.Instance.PlaySFXClick();
    }

    public void ReturnFromSettings()
    {
        pauseLocked = false;
        HideSettings();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ShowCanvasByIndex(activeSceneIndex);
        HideLobby();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Rebind
    public void ReturnFromRebind()
    {
        pauseLocked = false;
        HideRebind();
        ShowSettings();
        AudioManager.Instance.PlaySFXClose();
    }

    public void ShowControlsRebind()
    {
        pauseLocked = true;
        HideSettings();
        ShowRebind();
        AudioManager.Instance.PlaySFXClick();
    }

    public void HideControlsRebind()
    {
        HideRebind();
        ShowSettings();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Credits
    public void ShowCreditsCanvas()
    {
        HideMainMenu();
        ShowCredits();
        AudioManager.Instance.PlaySFXClick();
    }

    #endregion

    #region Mission
    public void ShowMisionCanvas()
    {
        pauseLocked = true;
        HideLobby();
        ShowMissions();
        refreshMission();
        AudioManager.Instance.PlaySFXClick();
    }

    public void refreshMission()
    {
        missionUIManager.RefreshMissionUI();
    }

    public void HideMissionsCanvas()
    {
        pauseLocked = false;
        HideMissions();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Help
    public void ShowHelpCanvas()
    {
        pauseLocked = true;
        HideLobby();
        ShowHelp();
        AudioManager.Instance.PlaySFXClick();
    }

    #endregion

    #region Weapons
    public void ShowWeaponsCanvas()
    {
        pauseLocked = true;
        HideLobby();
        ShowWeapon();
        AudioManager.Instance.PlaySFXClick();
    }

    #endregion

    #region Upgrades
    public void ShowUpgradesCanvas()
    {
        pauseLocked = true;
        HideLobby();
        ShowUpgrades();
        upgradesUIManager.RefreshUpgrades();
        AudioManager.Instance.PlaySFXClick();
    }

    #endregion

    #region Achievements
    public void ShowAchievementsCanvas()
    {
        pauseLocked = true;
        HideLobby();
        achievementsUIManager.RefreshAchievementUI();
        ShowAchievements();
        AudioManager.Instance.PlaySFXClick();
    }

    #endregion

    #region Endgame
    public void ShowEndGameCanvas()
    {
        pauseLocked = true;
        Time.timeScale = 0;
        HideAllCanvas();
        ShowEndGame();
        AudioManager.Instance.PlayEndGameMusic();
    }

    #endregion

    #region PopUp
    public void ShowPopUpCanvas(string action,bool active)
    {
        popUpUIManager.hideChamberPopUp();
        popUpUIManager.hidePowerUpPopUp();
        popUpUIManager.displaypopUpHelp(action, active);
        ShowPopUp();
        AudioManager.Instance.PlaySFXSelect();
    }

    public void ShowChamberNamePopUpCanvas(RoomType room)
    {
        popUpUIManager.displayPopUpChamber(room);
        ShowPopUp();
        AudioManager.Instance.PlaySFXSelect();
        StartCoroutine(HideChamberNameCanvasAfterDelay(3f));
    }

    public void ShowPowerUpPopUpCanvas(powerUpType powerup)
    {
        popUpUIManager.displayPowerUp(powerup);
        ShowPopUp();
        AudioManager.Instance.PlaySFXSelect();
        StartCoroutine(HidePowerUpCanvasAfterDelay(3f));
    }

    public void HidePopUpCanvas()
    {
        popUpUIManager.hideHelpPopUp();
        //AudioManager.Instance.PlaySFXClose();
    }

    private IEnumerator HidePopUpCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePopUp();
        AudioManager.Instance.PlaySFXClose();
    }

    private IEnumerator HideHelpCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUpUIManager.hideHelpPopUp();
        AudioManager.Instance.PlaySFXClose();
    }

    private IEnumerator HideChamberNameCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUpUIManager.hideChamberPopUp();
        AudioManager.Instance.PlaySFXClose();
    }

    private IEnumerator HidePowerUpCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popUpUIManager.hidePowerUpPopUp();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Shop
    public void RefreshShop(List<Trade> trades,ShopController shop)
    {
        shopUIManager.RefreshShopUI(trades, shop);
    }

    public void ShowShopCanvas()
    {
        pauseLocked = true;
        Time.timeScale = 0;
        ShowShop();
        AudioManager.Instance.PlaySFXOpen();
    }

    public void HideShopCanvas()
    {
        pauseLocked = false;
        Time.timeScale = 1;
        HideShop();
        AudioManager.Instance.PlaySFXClose();
    }
    #endregion

    #region Victory
    public void ShowVictoryCanvas()
    {
        pauseLocked = true;
        Time.timeScale = 0;
        HideAllCanvas();
        AudioManager.Instance.PlayVictoryMusic();
        ShowVictory();
    }

    public void HideVictoryCanvas()
    {
        Time.timeScale = 1;
        HideVictory();
        AudioManager.Instance.PlaySFXClose();
    }

    #endregion

    #region Persistency
    public bool canLoadGame()
    {
        return InventorySafeController.Instance.canLoadGame() && PowerUpStatsController.Instance.canLoadGame();
    }

    #endregion

    #region Player Stats UI

    public void ResetPlayerUiStats()
    {
        healthManaUIManager.ResetPlayer();
    }

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

    public void SetDamage(float damage)
    {
        healthManaUIManager.SetDamage(damage);
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        healthManaUIManager.SetAttackSpeed(attackSpeed);
    }

    public void SetPush(float push)
    {
        healthManaUIManager.SetPush(push);
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

    #region Boss Stats UI

    public void ShowBossHealth()
    {
        chamberUIManager.ShowBossHealth();
    }

    public void HideBossHealth()
    {
        chamberUIManager.HideBossHealth();
    }
    public void ResetBossHealth()
    {
        chamberUIManager.ResetBossHealth();
    }

    public void SetMaxBossHealth(float health)
    {
        chamberUIManager.SetMaxHealth(health);
    }

    public void SetBossHealth(float health)
    {
        chamberUIManager.SetHealth(health);
    }
    #endregion

    #region Weapon Selector
    public void ShowPrevWeapon()
    {
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
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneIndex == 1) 
            GameManager.Instance.EnterMapScene();
        else
            print("Estoy en la sala de entrenamiento");

        AudioManager.Instance.PlaySFXConfirmation();
        HideWeapon();
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
                GameManager.Instance.EnterLobbyScene();
                break;

            case 4:
                GameManager.Instance.EnterLobbyScene();
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

    private void HideBasement()
    {
        HideCanvasByIndex(4);
    }
    private void ShowBasement()
    {
        ShowCanvasByIndex(4);
    }

    private void HideSettings()
    {
        HideCanvasByIndex(5);
    }
    private void ShowSettings()
    {
        ShowCanvasByIndex(5);
    }

    private void HideCredits()
    {
        HideCanvasByIndex(6);
    }
    private void ShowCredits()
    {
        ShowCanvasByIndex(6);
    }

    private void HidePause()
    {
        HideCanvasByIndex(7);
    }

    private void ShowPause()
    {
        ShowCanvasByIndex(7);
    }
    private void HideMissions()
    {
        HideCanvasByIndex(8);
    }

    private void ShowMissions()
    {
        ShowCanvasByIndex(8);
    }
    private void HideHelp()
    {
        HideCanvasByIndex(9);
    }

    private void ShowHelp()
    {
        ShowCanvasByIndex(9);
    }
    private void HideWeapon()
    {
        HideCanvasByIndex(10);
    }

    private void ShowWeapon()
    {
        ShowCanvasByIndex(10);
    }
    private void HideUpgrades()
    {
        HideCanvasByIndex(11);
    }

    private void ShowUpgrades()
    {
        ShowCanvasByIndex(11);
    }
    private void HideAchievements()
    {
        HideCanvasByIndex(12);
    }

    private void ShowAchievements()
    {
        ShowCanvasByIndex(12);
    }
    private void HideEndGame()
    {
        HideCanvasByIndex(13);
    }

    private void ShowEndGame()
    {
        ShowCanvasByIndex(13);
    }

    private void ShowPopUp()
    {
        ShowCanvasByIndex(14);
    }

    private void HidePopUp()
    {
        popUpUIManager.hidePopUps();
        HideCanvasByIndex(14);
    }

    private void ShowShop()
    {
        ShowCanvasByIndex(15);
    }

    private void HideShop()
    {
        HideCanvasByIndex(15);
    }

    private void ShowRebind()
    {
        ShowCanvasByIndex(16);
    }

    private void HideRebind()
    {
        HideCanvasByIndex(16);
    }
    private void ShowVictory()
    {
        ShowCanvasByIndex(17);
    }

    private void HideVictory()
    {
        HideCanvasByIndex(17);
    }

    private void ShowWelcome()
    {
        ShowCanvasByIndex(18);
    }

    private void HideWelcome()
    {
        HideCanvasByIndex(19);
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
    public void HideAllCanvas()
    {
        for (int i = 0; i >= 0 && i < ArrayCanvas.Length; i++)
        {
            ArrayCanvas[i].gameObject.SetActive(false);
        }
    }

    #endregion 
}
