using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Corridors {
  North = 1 << 1,
  South = 1 << 2,
  East  = 1 << 3,
  West  = 1 << 4,
}

public class DungeonTile : MonoBehaviour {
  [SerializeField] private Transform north;
  [SerializeField] private Transform south;
  [SerializeField] private Transform east;
  [SerializeField] private Transform west;

  private int corridors;
}
