using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region MonoBehaviour

    public static GameManager Instance { get; private set; }

    public PlayerStats playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    #region map

    private int mapSize_;
    private List<Tile> map_;

    public List<Tile> map
    {
        get { return map_; }
        set
        {
            Assert.IsNull(map_);  // Do NOT overwrite maps...
            map_ = value;
            mapSize_ = (int)Mathf.Sqrt(map_.Count);
        }
    }

    public void ClearMap()
    {
        map_ = null;
        mapSize_ = 0;
    }

    private void ClearPlayerStats()
    {
        playerStats.Reset();
    }

    public Vector2Int tile { get; set; }

    public RoomType room { get { return map[tile.x * mapSize_ + tile.y].GetRoom(); } }

    #endregion

    #region SceneManagement

    public void EnterMainMenuScene()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayMenuMusic();
        UIManager.Instance.ShowMainMenuCanvas();
        ClearMap();
        SceneManager.LoadScene("MainMenu");
    }

  public void EnterLobbyScene() {
    Time.timeScale = 1;
    AudioManager.Instance.PlayLobbyMusic();
    UIManager.Instance.ShowLobbyCanvas();
    ClearMap();
    ClearPlayerStats();
    InventoryList.Instance.ResetStats();
    SceneManager.LoadScene("Lobby");
  }

    public void EnterNewGameScene()
    {
        SceneManager.LoadScene("Basement");
        AudioManager.Instance.PlayLobbyMusic();
        UIManager.Instance.ShowBasementCanvas();
        UIManager.Instance.ShowWelcomeCanvas();
    }

    public void NewGame()
    {
        InventorySafeController.Instance.newGame();
        PowerUpStatsController.Instance.newGame();
        EnterNewGameScene();
    }

  public void VictoryReturn() {
    //a�adir persistencia guardar loot en inventario taberna
    UIManager.Instance.HideVictoryCanvas();
    InventorySafeController.Instance.addInventory(InventoryList.Instance.getItems());
    EnterLobbyScene();
    //a�adir persistencia
  }

    public void ContinueGame()
    {
        InventorySafeController.Instance.loadGame();
        PowerUpStatsController.Instance.loadGame();
        EnterLobbyScene();
        //a�adir persistencia
    }

    public void EnterMapScene()
    {
        SceneManager.LoadScene("Map");
        AudioManager.Instance.PlayMapMusic();
        UIManager.Instance.ShowMapCanvas();
    }

  public void EnterChamberScene() {
    AudioManager.Instance.PlayChamberMusic();
    UIManager.Instance.ShowChamberCanvas();
    UIManager.Instance.ShowChamberNamePopUpCanvas(room);
        if((int)room == 0)
        {
            //UIManager.Instance.ShowBossHealth();
        }
        else
        {
            UIManager.Instance.HideBossHealth();
        }
    SceneManager.LoadScene("Chamber");
  }

    public void EnterTrainingScene()
    {
        AudioManager.Instance.PlayLobbyMusic();
        UIManager.Instance.HidePopUpCanvas();
        UIManager.Instance.HideAllCanvas();
        UIManager.Instance.ShowBasementCanvas();
        SceneManager.LoadScene("Basement");
    }

    #endregion

    #region WeaponType

    public int _weaponType = 0;
    public int getCurrentWeaponType()
    {
        return _weaponType;
    }

    public void setCurrentWeaponType(int weaponType)
    {
        _weaponType = weaponType;
        playerStats.SetWeaponType(weaponType);
    }
    #endregion
}
