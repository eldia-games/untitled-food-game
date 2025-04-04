using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Tile {
  private TileData tileData_;
  private int roads_;
  private TileType type_;
  private RoomType room_;
  private GameObject instance_;
  private Outline outline_;
  private Collider collider_;

  public Tile(TileData tileData, TileType type) {
    tileData_ = tileData;
    roads_    = 0;
    type_     = type;
    room_     = 0;
  }

  #region RoadLogic

  public void Connect(Vector2Int offset) {
    if (offset.x < 0) roads_ |= 0b0001;  // Path from top
    if (offset.y < 0) roads_ |= 0b0010;  // Path from right
    if (offset.y > 0) roads_ |= 0b0100;  // Path to top
    if (offset.x > 0) roads_ |= 0b1000;  // Path to right
  }

  public bool HasRoad(Vector2Int offset) {
    int road = 0;
    if (offset.x < 0) road |= roads_ & 0b0001;  // Path from top
    if (offset.y < 0) road |= roads_ & 0b0010;  // Path from right
    if (offset.y > 0) road |= roads_ & 0b0100;  // Path to top
    if (offset.x > 0) road |= roads_ & 0b1000;  // Path to right
    return road != 0 && type_ == TileType.Room;
  }

  #endregion

  #region RoomLogic

  public bool HasRoom() {
    return type_ == TileType.Room && roads_ != 0;
  }

  public RoomType GetRoom() {
    return room_;
  }

  public void SetRoom(RoomType type) {
    Assert.IsTrue(type_ == TileType.Room);
    room_ = type;
  }

  #endregion

  #region InstanceLogic

  public void Instantiate(Vector3 position, Transform parent) {
    int index = roads_;
    if (type_ == TileType.Fence || type_ == TileType.River || type_ == TileType.Wall) {
      if (roads_ == 0b0000) index = 0;
      if (roads_ == 0b1001) index = 1;
      if (roads_ == 0b0110) index = 2;
    }

    switch (type_) {
      case TileType.Fence: instance_ = tileData_.fences[index]; break;
      case TileType.River: instance_ = tileData_.rivers[index]; break;
      case TileType.Wall:  instance_ = tileData_.walls [index]; break;
      default:             instance_ = tileData_.roads [index]; break;
    }
    instance_ = GameObject.Instantiate(instance_, position, instance_.transform.rotation, parent);

    outline_ = instance_.GetComponentInChildren<Outline>();
    if (outline_ != null) outline_.enabled = false;

    collider_ = instance_.GetComponentInChildren<SphereCollider>();
    if (collider_ != null) collider_.enabled = false;

    if (type_ == TileType.Room && roads_ != 0) {
      GameObject decoration = tileData_.rooms[(int) room_];
      Vector3 pos = instance_.transform.position + Vector3.forward * 2;
      Quaternion rot = decoration.transform.rotation;
      decoration = GameObject.Instantiate(decoration, pos, rot, instance_.transform);
    }
  }

  public bool IsInstance(GameObject instance) {
    return instance_ == instance;
  }

  public void EnableInteractions() {
    if (outline_ == null || collider_ == null) return;
    outline_.enabled = true;
    collider_.enabled = true;
  }

  public void Outline() {
    if (outline_ == null) return;
    outline_.OutlineColor = Color.white;
    outline_.OutlineWidth = 4.0f;
  }

  public void Highlight() {
    if (outline_ == null) return;
    outline_.OutlineColor = Color.green;
    outline_.OutlineWidth = 8.0f;
  }

  #endregion
}
