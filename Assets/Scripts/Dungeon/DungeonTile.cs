using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DungeonTile : MonoBehaviour {
  [SerializeField] private Transform[] nsew = new Transform[4];
  [SerializeField] private GameObject door;
  [SerializeField] private GameObject wall;

  private Transform transform_;

  void Start() {
    transform_ = transform;
  }

  public void Create(int corridors) {
    Assert.IsTrue(corridors >= 0 && corridors <= 15, "Corridors debe ser un número entre 0b0000 y 0b1111");
    for (int i = 0; i < 4; ++i) {
      GameObject template = ((1 << i) & corridors) != 0 ? door : wall;
      Instantiate(template, nsew[i]);  // .position, nsew[i].rotation, transform_
    }
  }
}
