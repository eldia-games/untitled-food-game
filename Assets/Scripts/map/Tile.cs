using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
  private TileData tileData_;
  private Vector3 position_;
  private int type_;

  public Tile(TileData tileData, Vector3 position, int type) {
    tileData_ = tileData;
    position_ = position;
    type_ = type;
  }

  public void Connect(Vector2Int offset) {
    if (offset.x < 0) type_ |= 0b0001;  // Path from top
    if (offset.y < 0) type_ |= 0b0010;  // Path from bottom
    if (offset.y > 0) type_ |= 0b0100;  // Path to top
    if (offset.x > 0) type_ |= 0b1000;  // Path to bottom
  }

  public bool HasRoad() {
    return (type_ & 0b1111) != 0;
  }

  public bool HasRoad(Vector2Int offset) {
    int road = 0;
    if (offset.x < 0) road |= type_ & 0b0001;  // Path from top
    if (offset.y < 0) road |= type_ & 0b0010;  // Path from bottom
    if (offset.y > 0) road |= type_ & 0b0100;  // Path to top
    if (offset.x > 0) road |= type_ & 0b1000;  // Path to bottom
    return road != 0;
  }

  public bool IsRiver() {
    return (type_ & 0b10000) != 0;
  }

  public GameObject Instantiate(Transform parent) {
    int index = type_;
    // if (type_ == 0b10000) index = 0b10000;
    if (type_ == 0b11001) index = 0b10001;
    if (type_ == 0b10110) index = 0b10010;

    GameObject instance = tileData_.lookup[index];
    instance = GameObject.Instantiate(instance, position_, instance.transform.rotation, parent);
    return instance;
  }

  public override string ToString() {
    string value = Convert.ToString(type_, 2);
    while (value.Length < 5) value = "0" + value;
    return "0b" + value;
  }
}
