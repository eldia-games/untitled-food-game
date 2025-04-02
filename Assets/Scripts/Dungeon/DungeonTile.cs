using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DungeonTile : MonoBehaviour {
  [SerializeField] private Transform[] nsew = new Transform[4];
  [SerializeField] private GameObject door;
  [SerializeField] private GameObject wall;
  [SerializeField] private GameObject[] contents;

  public void Create(int corridors, bool exit, DungeonController controller) {
    GameObject template;
    Assert.IsTrue(corridors >= 0 && corridors <= 15, "Corridors debe ser un número entre 0b0000 y 0b1111");
    for (int i = 0; i < 4; ++i) {
      template = ((1 << i) & corridors) != 0 ? door : wall;
      Instantiate(template, nsew[i]);  // .position, nsew[i].rotation, transform_
    }

    if (exit) {
      template = Instantiate(contents[0], transform);
    } else {
      template = contents[Random.Range(1, contents.Length)];
      template = Instantiate(template, transform);
      template.transform.Rotate(0, Random.Range(0, 4) * 90, 0);
    }

    template.GetComponent<DungeonChamber>().Create(controller);
  }
}
