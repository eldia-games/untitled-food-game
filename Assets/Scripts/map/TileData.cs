using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Map/TileData", order = 1)]
public class TileData : ScriptableObject {
  public GameObject[] lookup;
}
