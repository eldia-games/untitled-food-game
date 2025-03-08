using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {

  #region MonoBehaviour

  public static GameManager instance { get; private set; }

  private void Awake() {
    if (instance != null && instance != this) {
      Destroy(gameObject);
    } else {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  #endregion

  #region map

  private List<Tile> map_;

  public List<Tile> map {
    get { return map_; }
    set { Assert.IsNull(map_); map_ = value; }
  }

  public void ClearMap() {
    map_ = null;
  }

  public Vector2Int tile { get; set; }

  #endregion

}
