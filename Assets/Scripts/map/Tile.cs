using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Tile {
  private TileData tileData_;
  private Vector3 position_;
  private int roads_;
  private TileType type_;
  private RoomType room_;

  public Tile(TileData tileData, Vector3 position, TileType type) {
    tileData_ = tileData;
    position_ = position;
    roads_    = 0;
    type_     = type;
    room_     = 0;
  }

  public void Connect(Vector2Int offset) {
    if (offset.x < 0) roads_ |= 0b0001;  // Path from top
    if (offset.y < 0) roads_ |= 0b0010;  // Path from bottom
    if (offset.y > 0) roads_ |= 0b0100;  // Path to top
    if (offset.x > 0) roads_ |= 0b1000;  // Path to bottom
  }

  public bool HasRoad(Vector2Int offset) {
    int road = 0;
    if (offset.x < 0) road |= roads_ & 0b0001;  // Path from top
    if (offset.y < 0) road |= roads_ & 0b0010;  // Path from bottom
    if (offset.y > 0) road |= roads_ & 0b0100;  // Path to top
    if (offset.x > 0) road |= roads_ & 0b1000;  // Path to bottom
    return road != 0;
  }

  public bool HasRoom() {
    return type_ == TileType.Room && roads_ != 0;
  }

  public void SetRoom(RoomType type) {
    Assert.IsTrue(type_ == TileType.Room);
    room_ = type;
  }

  public GameObject Instantiate(Transform parent) {
    int index = roads_;
    if (type_ == TileType.Fence || type_ == TileType.River || type_ == TileType.Wall) {
      if (roads_ == 0b0000) index = 0;
      if (roads_ == 0b1001) index = 1;
      if (roads_ == 0b0110) index = 2;
    }

    GameObject instance;
    switch (type_) {
      case TileType.Fence: instance = tileData_.fences[index]; break;
      case TileType.River: instance = tileData_.rivers[index]; break;
      case TileType.Wall:  instance = tileData_.walls [index]; break;
      default:             instance = tileData_.roads [index]; break;
    }
    instance = GameObject.Instantiate(instance, position_, instance.transform.rotation, parent);

    if (type_ == TileType.Room && roads_ != 0) {
      GameObject decoration = tileData_.rooms[(int) room_];
      Vector3 pos = instance.transform.position + Vector3.forward * 2;
      Quaternion rot = decoration.transform.rotation;
      decoration = GameObject.Instantiate(decoration, pos, rot, instance.transform);
    }

    return instance;
  }
}
