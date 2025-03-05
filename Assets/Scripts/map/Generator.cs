using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public class Generator : MonoBehaviour {
  [SerializeField] private TileData tileData;
  [SerializeField] private Vector3Int frw;  // Fence, river, wall columns
  [SerializeField] private int mapSize;
  [SerializeField] private int numPaths;
  [SerializeField] private float[] roomOdds;

  private Transform transform_;
  private List<Tile> map_;

  void Start() {
    transform_ = transform;

    Assert.IsTrue(mapSize % 2 != 0, "mapSize has to be an odd integer");
    Assert.IsTrue(frw.x < frw.y && frw.y < frw.z, "The values in frw have to increase");
    Assert.IsTrue(frw.x % 2 != 0 && frw.y % 2 != 0 && frw.z % 2 != 0, "The values in frw have to be odd integers");

    // Initialize the map
    Initialize();
    for (int i = 0; i < numPaths; ++i) Traverse();
    Fill();
    Instantiate();
  }

  /**
   * Initialize the map as a pathless grid
   */
  void Initialize() {
    Vector3 xOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, -1);
    Vector3 yOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, +1);
    Vector3 mapOff = (xOff + yOff) * (mapSize * -.5f);

    map_ = new List<Tile>(mapSize * mapSize);
    for (int i = 0; i < mapSize; ++i) {
      for (int j = 0; j < mapSize; ++j) {
        TileType type = TileType.Void;
        if (i % 2 == 0 || j % 2 == 0) type = TileType.Road;
        if (i + j == frw.x)           type = TileType.Fence;
        if (i + j == frw.y)           type = TileType.River;
        if (i + j == frw.z)           type = TileType.Wall;
        if (i % 2 == 0 && j % 2 == 0) type = TileType.Room;

        Tile tile = new Tile(tileData, xOff * i + yOff * j + mapOff, type);
        map_.Add(tile);
      }
    }
  }

  /**
   * Carve a path through the map
   */
  void Traverse() {
    Vector2Int curr   = Vector2Int.zero;
    Vector2Int next   = Vector2Int.zero;
    Vector2Int offset = Vector2Int.zero;

    List<Vector2Int> path = new List<Vector2Int>(mapSize * 2 - 1);
    path.Add(curr);
    while (curr.x * mapSize + curr.y < mapSize * mapSize - 1) {
      Tile tile = GetTile(curr);

      // Choose next tile
      offset = Random.value < 0.5 ? Vector2Int.up : Vector2Int.right;
      next = curr + offset;

      // If road already exists switch to the other option
      if (tile.HasRoad(offset)) {
        offset = new Vector2Int(offset.y, offset.x);
        next = curr + offset;
      }

      // If next is out of bounds switch to the other option
      bool outOfBounds = next.x >= mapSize || next.y >= mapSize;
      if (outOfBounds) {
        offset = new Vector2Int(offset.y, offset.x);
        next = curr + offset;
      }

      // Add two tiles to the path and continue
      path.Add(curr = next);
      next = curr + offset;
      path.Add(curr = next);
    }

    // Connect all tile in the path
    for (int i = 0; i < path.Count - 1; ++i) {
      curr = path[i + 0];
      next = path[i + 1];
      offset = next - curr;

      GetTile(curr).Connect(offset);
      GetTile(next).Connect(-offset);
    }
  }

  /**
   * Fills the map with rooms
   */
  void Fill() {
    for (int i = 0; i < mapSize; ++i) {
      for (int j = 0; j < mapSize; ++j) {
        Tile tile = GetTile(i, j);
        if (!tile.HasRoom()) continue;

        RoomType room = RoomType.Tavern;
        do room = SampleRoom();
        while (!ValidateRoom(room, i + j));
        if (i + j == 0)               room = RoomType.Tavern;
        if (i + j == 2)               room = RoomType.Grain;
        if (i + j == frw.y + 1)       room = RoomType.Trees;
        if (i + j == frw.z + 1)       room = RoomType.Treasure;
        if (i + j == 2 * mapSize - 4) room = RoomType.Rest;
        if (i + j == 2 * mapSize - 2) room = RoomType.Boss;
        tile.SetRoom(room);
      }
    }
  }

  bool ValidateRoom(RoomType room, int depth) {
    return
      !(room == RoomType.Rest  && depth < mapSize - 1) &&
      !(room == RoomType.Elite && depth < mapSize - 1);
  }

  RoomType SampleRoom() {
    float r = Random.value;

    for (int i = 0; i < 9; ++i) {
      if (r < roomOdds[i]) return (RoomType)i;
      r -= roomOdds[i];
    }

    return RoomType.Tavern;
  }
  
  /**
   * Index into tile map
   */
  private Tile GetTile(int i, int j) {
    return map_[i * mapSize + j];
  }

  /**
   * Index into tile map
   */
  private Tile GetTile(Vector2Int index) {
    return GetTile(index.x, index.y);
  }

  /**
   * Instantiate the map as a set of tiles
   */
  void Instantiate() {
    for (int i = 0; i < map_.Count; ++i)
      map_[i].Instantiate(transform_);
  }
}
