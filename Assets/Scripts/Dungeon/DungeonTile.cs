using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DungeonTile : MonoBehaviour {
  [SerializeField] private Transform[] nsew = new Transform[4];
  [SerializeField] private GameObject door;
  [SerializeField] private GameObject wall;
  [SerializeField] private GameObject[] contents;

  public void Create(int corridors, bool exit) {
    Assert.IsTrue(corridors >= 0 && corridors <= 15, "Corridors debe ser un número entre 0b0000 y 0b1111");
    for (int i = 0; i < 4; ++i) {
      GameObject template = ((1 << i) & corridors) != 0 ? door : wall;
      Instantiate(template, nsew[i]);  // .position, nsew[i].rotation, transform_
    }

    if (exit) {
      Instantiate(contents[0], transform);
      return;
    }

    GameObject content = contents[Random.Range(1, contents.Length)];
    GameObject instance = Instantiate(content, transform);
    instance.transform.Rotate(0, Random.Range(0, 4) * 90, 0);
  }
}
