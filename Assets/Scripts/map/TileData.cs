using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Map/TileData", order = 1)]
public class TileData : ScriptableObject {
  public GameObject[] fences;
  public GameObject[] rivers;
  public GameObject[] walls;
  public GameObject[] roads;
  public GameObject[] rooms;
}
