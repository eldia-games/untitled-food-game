using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
  [SerializeField] private TileData tileData;
  [SerializeField] private int mapSize;
  [SerializeField] private List<int> riverColumns;
  [SerializeField] private int numPaths;
  [SerializeField] private int mapUpscale;

  private Transform transform_;
  private List<Tile> map_;
  private List<GameObject> tiles_;

  void Start() {
    transform_ = transform;

    // Initialize the map
    Initialize();
    for (int i = 0; i < numPaths; ++i) Traverse();
    Instantiate();
  }

  /**
   * Initialize the map as a pathless grid
   */
  void Initialize() {
    Vector3 xOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, -1);
    Vector3 yOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, +1);
    Vector3 mapOff = (xOff + yOff) * ((mapSize * mapUpscale - 1) * -.5f);

    map_ = new List<Tile>(mapSize * mapSize * mapUpscale * mapUpscale);
    for (int i = 0; i < mapSize * mapUpscale; ++i) {
      for (int j = 0; j < mapSize * mapUpscale; ++j) {
        Tile tile = new Tile(tileData, xOff * i + yOff * j + mapOff, riverColumns.Contains(i + j - mapSize * (mapUpscale - 1)) ? 0b10000 : 0);
        map_.Add(tile);
      }
    }
  }

  /**
   * Carve a path through the map
   */
  void Traverse() {
    int temp = (int)(mapSize * (mapUpscale - 1) * 0.5f);
    Vector2Int begin  = new Vector2Int(temp, temp);

    Vector2Int curr   = Vector2Int.zero;
    Vector2Int next   = Vector2Int.zero;
    Vector2Int offset = Vector2Int.zero;

    List<Vector2Int> path = new List<Vector2Int>(mapSize * 2 - 1);
    path.Add(curr);
    while (curr.x * mapSize + curr.y < mapSize * mapSize - 1) {
      Tile tile = GetTile(begin + curr);

      // Choose next tile
      if (!tile.IsRiver())
        offset = Random.Range(0, 2) == 0 ? Vector2Int.up : Vector2Int.right;
      next = curr + offset;

      // If next is out of bounds switch to the other option
      bool outOfBounds = next.x >= mapSize || next.y >= mapSize;
      if (outOfBounds && tile.IsRiver()) {
        path.Clear();
        path.Add(curr = Vector2Int.zero);
        continue;
      } else if (outOfBounds) {
        offset = new Vector2Int(offset.y, offset.x);
        next = curr + offset;
      }

      // If two roads go through the same river in different directions, restart the road from scratch
      if (tile.IsRiver() && tile.HasRoad() && !tile.HasRoad(offset)) {
        path.Clear();
        path.Add(curr = Vector2Int.zero);
        continue;
      }

      // Add tile to the path and go to next tile
      path.Add(curr = next);
    }

    // Connect all tile in the path
    for (int i = 0; i < path.Count - 1; ++i) {
      curr = path[i + 0];
      next = path[i + 1];
      offset = next - curr;

      GetTile(begin + curr).Connect( offset);
      GetTile(begin + next).Connect(-offset);
    }
  }

  private Tile GetTile(Vector2Int index) {
    return map_[index.x * mapSize * mapUpscale + index.y];
  }

  /**
   * Instantiate the map as a set of tiles
   */
  void Instantiate() {
    tiles_ = new List<GameObject>(map_.Count);
    for (int i = 0; i < map_.Count; ++i) {
      GameObject tile = map_[i].Instantiate(transform_);
      tiles_.Add(tile);
    }
  }
}
