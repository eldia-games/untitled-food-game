using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberGenerator : MonoBehaviour {
  [SerializeField] private bool persistanceStart;
  private GameManager _GameManager;
  private persistence _PersistenceObject;

  void Start() {
    _GameManager = GameManager.Instance;
    _PersistenceObject = persistence.Instance;

    if (persistanceStart) {
      PersistanceStart();
      return;
    }

    GameManagerStart();
  }

  void GameManagerStart() {
    TypeChamberGenerator[] generators = this.GetComponents<TypeChamberGenerator>();
    RoomType type = _GameManager.room;
    int level = _GameManager.tile.x + _GameManager.tile.y;
    for (int i = 0; i < generators.Length; i++) {
      if (type == generators[i].getChamberType()) {
        generators[i].createChamber(level);
        return;
      }
    }
  }

  void PersistanceStart() {
    TypeChamberGenerator[] generators = this.GetComponents<TypeChamberGenerator>();
    RoomType type = _PersistenceObject.getType();
    int level = _PersistenceObject.getLevel();
    for (int i = 0; i < generators.Length; i++) {
      if (type == generators[i].getChamberType()) {
        generators[i].createChamber(level);
        return;
      }
    }
  }
}
