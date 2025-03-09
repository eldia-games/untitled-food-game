using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

  #region MonoBehaviour

  public static GameManager Instance { get; private set; }

  private void Awake() {
    if (Instance != null && Instance != this) {
      Destroy(gameObject);
    } else {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  #endregion

  #region map

  private int mapSize_;
  private List<Tile> map_;

  public List<Tile> map {
    get { return map_; }
    set {
      Assert.IsNull(map_);  // Do NOT overwrite maps...
      map_ = value;
      mapSize_ = (int) Mathf.Sqrt(map_.Count);
    }
  }

  public void ClearMap() {
    map_ = null;
    mapSize_ = 0;
  }

  public Vector2Int tile { get; set; }

  public RoomType room { get { return map[tile.x * mapSize_ + tile.y].GetRoom(); } }

  #endregion

  #region SceneManagement

  public void EnterMainMenuScene() {
    SceneManager.LoadScene("MainMenu");
    AudioManager.Instance.PlayMenuMusic();
  }

  public void EnterLobbyScene() {
    SceneManager.LoadScene("Lobby");
    AudioManager.Instance.PlayLobbyMusic();
  }

  public void EnterMapScene() {
    SceneManager.LoadScene("Map");
    AudioManager.Instance.PlayMapMusic();
  }

  public void EnterChamberScene() {
    SceneManager.LoadScene("Chamber");
    AudioManager.Instance.PlayChamberMusic();
  }

  #endregion

}
