using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
  private const int NORTH = 1 << 0;
  private const int SOUTH = 1 << 1;
  private const int EAST  = 1 << 2;
  private const int WEST  = 1 << 3;

  [SerializeField] private NavMeshSurface navmesh;
  [SerializeField] private GameObject frame;
  [SerializeField] private GameObject grass;
  [SerializeField] private GameObject exit;
  [SerializeField] private bool initOnStart = false;
  [SerializeField] private int scale;
  [SerializeField, Range(1, 9)] private int size;
  [SerializeField, Range(1, 5)] private int padding;

  private List<int> rooms;
  private Transform transform_;

  public void Start() {
    if (initOnStart) Initialize(null);
  }

  public void Initialize(DungeonController controller) {
    Initialize();
    Connect();
    Create(controller);
    BuildNavMesh();
  }

  private void Initialize() {
    transform_ = transform;
    rooms = new List<int>(size * size);
    for (int i = 0; i < size; ++i) {
      for (int j = 0; j < size; ++j) {
        rooms.Add(0);
      }
    }
  }

  private void Connect() {
    Vector2Int origin = new Vector2Int(size / 2, 0);
    Connect(origin, origin + Vector2Int.down);
  }

  private void Connect(Vector2Int to, Vector2Int from) {
    {
      Vector2Int delta = from - to;
      if (delta == Vector2Int.up)    rooms[to.x * size + to.y] |= NORTH;
      if (delta == Vector2Int.down)  rooms[to.x * size + to.y] |= SOUTH;
      if (delta == Vector2Int.right) rooms[to.x * size + to.y] |= EAST;
      if (delta == Vector2Int.left)  rooms[to.x * size + to.y] |= WEST;
    }

    // Shuffle list of candidates
    Vector2Int[] candidates = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
    for (int i = 0; i < candidates.Length; ++i) {
      int j = Random.Range(i, candidates.Length);
      Vector2Int temp = candidates[i];
      candidates[i] = candidates[j];
      candidates[j] = temp;
    }

    // For every candidate, try to expand a path
    for (int i = 0; i < candidates.Length; ++i) {
      Vector2Int delta = candidates[i];
      Vector2Int next = to + delta;
      if (next.x < 0 || next.x >= size) continue;
      if (next.y < 0 || next.y >= size) continue;
      if (rooms[next.x * size + next.y] != 0) continue;

      if (delta == Vector2Int.up)    rooms[to.x * size + to.y] |= NORTH;
      if (delta == Vector2Int.down)  rooms[to.x * size + to.y] |= SOUTH;
      if (delta == Vector2Int.right) rooms[to.x * size + to.y] |= EAST;
      if (delta == Vector2Int.left)  rooms[to.x * size + to.y] |= WEST;

      Connect(next, to);
    }
  }

  private void Create(DungeonController controller) {
    Vector3 origin = new Vector3(size * -.5f, 0, 0);
    int safe = (size / 2) * size + 0;
    int r = safe;
    while (r == safe) r = Random.Range(0, size * size);
    for (int i = -padding; i < size + padding; ++i) {
      for (int j = -padding; j < size + padding; ++j) {
        Vector3 position = (origin + Vector3.right * i + Vector3.forward * j) * scale;
        Quaternion rotation = Quaternion.identity;
        if (j == -1 && i == size / 2) {
          GameObject room = Instantiate(exit, position, rotation, transform_);
          room.GetComponent<DungeonChamber>().Create(controller, false);
        } else if (i < 0 || i >= size || j < 0 || j >= size) {
          Instantiate(grass, position, rotation, transform_);
        } else {
          int index = i * size + j;
          GameObject room = Instantiate(frame, position, rotation, transform_);
          room.GetComponent<DungeonTile>().Create(controller, rooms[i * size + j], index == safe, index == r);
        }
      }
    }
  }

  private void BuildNavMesh(){
    navmesh.BuildNavMesh();
  }
}
