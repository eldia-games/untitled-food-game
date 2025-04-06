using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Generator : MonoBehaviour {
  [SerializeField] private TileData tileData;
  [SerializeField] private int mapSize;
  [SerializeField] private int numPaths;
  [SerializeField] private float[] roomOdds;
  [SerializeField] private Transform playerTransform;
  [SerializeField] private MapMovement playerMovement;
  [SerializeField] private LayerMask tileLayer;

  private GameManager gameManager_;
  private Transform transform_;
  private List<Tile> map_;
  private Vector2Int tile_;
  private Vector2Int target_;
  private Camera camera_;
  private int fence_;
  private int river_;
  private int wall_;

  #region MonoBehaviour

  void Start() {
    gameManager_ = GameManager.Instance;
    transform_ = transform;
    camera_ = Camera.main;

    Assert.IsTrue(mapSize % 2 != 0, "mapSize has to be an odd integer");

    fence_ = (mapSize * 2 / 6) - 2;
    river_ = (mapSize * 2 / 3) - 1;
    wall_  = (mapSize * 4 / 3) - 1;

    if (gameManager_.map == null) {
      // Initialize the map
      Initialize();
      for (int i = 0; i < numPaths; ++i) Traverse();
      Fill(0, 0, RoomType.Tavern);

      gameManager_.map = map_;
      gameManager_.tile = tile_ = Vector2Int.zero;
    } else {
      map_ = gameManager_.map;
      tile_ = gameManager_.tile;
    }

    Instantiate();
  }

  private void Update() {
    if (!playerMovement.IsLocked()) {
      TileSelect();
      return;
    }

    if (playerMovement.TargetReached()) {
      gameManager_.EnterChamberScene();
    }
  }

  #endregion

  #region MapGeneration

  /**
   * Initialize the map as a pathless grid
   */
  void Initialize() {
    map_ = new List<Tile>(mapSize * mapSize);
    for (int i = 0; i < mapSize; ++i) {
      for (int j = 0; j < mapSize; ++j) {
        TileType type = TileType.Void;
        if (i % 2 == 0 || j % 2 == 0) type = TileType.Road;
        if (i + j == fence_) type = TileType.Fence;
        if (i + j == river_) type = TileType.River;
        if (i + j ==  wall_) type = TileType.Wall;
        if (i % 2 == 0 && j % 2 == 0) type = TileType.Room;

        Tile tile = new Tile(tileData, type);
        map_.Add(tile);
      }
    }
  }

  /**
   * Carve a path through the map
   */
  void Traverse() {
    Vector2Int curr = Vector2Int.zero;
    Vector2Int next = Vector2Int.zero;
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

  void Fill(int i, int j, RoomType room) {
    Tile tile = GetTile(i, j);
    Assert.IsTrue(tile.HasRoom());
    tile.SetRoom(room);

    RoomType nextU = RoomType.Tavern;
    RoomType nextR = RoomType.Tavern;
    int depth = i + j + 2;
    do {
      nextU = SampleRoom(depth);
      nextR = SampleRoom(depth);
    } while (!ValidateRooms(nextU, nextR, depth));

    if (tile.HasRoad(Vector2Int.up   )) Fill(i, j + 2, nextU);
    if (tile.HasRoad(Vector2Int.right)) Fill(i + 2, j, nextR);
  }

  RoomType SampleRoom(int depth) {
    if (depth == 0) return RoomType.Tavern;
    if (depth == 2) return RoomType.Grain;
    //if (depth == river_ + 1) return RoomType.Trees;
    if (depth == wall_ + 1) return RoomType.Treasure;
    if (depth == 2 * mapSize - 4) return RoomType.Rest;
    if (depth == 2 * mapSize - 2) return RoomType.Boss;

    RoomType room = RoomType.Tavern;
    List<RoomType> lateRooms = new List<RoomType> { RoomType.Rest, RoomType.Elite };
    do {
      float r = Random.value;
      for (int i = 0; i < 9; ++i) {
        if ((r -= roomOdds[i]) < 0) {
          room = (RoomType)i;
          break;
        }
      }
    } while (lateRooms.Contains(room) && depth < mapSize - 1);

    return room;
  }

  bool ValidateRooms(RoomType nextU, RoomType nextR, int depth) {
    return
      depth == 0 ||
      depth == 2 ||
      //depth == river_ + 1 ||
      depth == wall_ + 1 ||
      depth == 2 * mapSize - 4 ||
      depth == 2 * mapSize - 2 ||
      nextU != nextR;
  }

  #endregion

  #region MapAccess

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

  #endregion

  #region MapDisplay

  /**
   * Instantiate the map as a set of tiles
   */
  void Instantiate() {
    Vector3 xOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, -1);
    Vector3 yOff = new Vector3(Mathf.Sin(Mathf.PI / 3.0f) * 2, 0, +1);
    Vector3 mapOff = (xOff + yOff) * (mapSize * -.5f);

    for (int i = 0; i < mapSize; ++i)
      for (int j = 0; j < mapSize; ++j)
        GetTile(i, j).Instantiate(xOff * i + yOff * j + mapOff, transform_);

    Tile tile = GetTile(tile_);
    if (tile.HasRoad(Vector2Int.up   )) GetTile(tile_ + 2 * Vector2Int.up   ).EnableInteractions();
    if (tile.HasRoad(Vector2Int.right)) GetTile(tile_ + 2 * Vector2Int.right).EnableInteractions();

    playerTransform.position = xOff * tile_.x + yOff * tile_.y + mapOff;
    playerMovement.ClearTarget();
  }

  #endregion

  #region MapMovement

  private void TileSelect() {
    Tile tile = GetTile(tile_);
    Tile nextU = tile.HasRoad(Vector2Int.up   ) ? GetTile(tile_ + 2 * Vector2Int.up   ) : null;
    Tile nextR = tile.HasRoad(Vector2Int.right) ? GetTile(tile_ + 2 * Vector2Int.right) : null;

    nextU?.Outline();
    nextR?.Outline();

    Ray ray = camera_.ScreenPointToRay(Input.mousePosition); // Lanza un rayo desde la c�mara
    if (!Physics.Raycast(ray, out RaycastHit hit, 100.0f, tileLayer)) return;
    GameObject instance = hit.transform.parent.gameObject;

    TileHover(nextU, Vector2Int.up   , instance);
    TileHover(nextR, Vector2Int.right, instance);
  }

  private void TileHover(Tile tile, Vector2Int path, GameObject instance) {
    if (tile == null || !tile.IsInstance(instance)) return;

    tile.Highlight();
    if (!Input.GetMouseButtonDown(0)) return;
    
    playerMovement.SetTarget(instance.transform.position);
    gameManager_.tile += 2 * path;
  }

  #endregion
}
