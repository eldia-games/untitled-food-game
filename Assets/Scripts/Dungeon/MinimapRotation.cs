using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRotation : MonoBehaviour {
  private Transform transform_;

  private void Start() {
    transform_ = transform;
  }

  void Update() {
    Vector3 rotation = transform_.eulerAngles;
    rotation.y = 0f;
    transform_.eulerAngles = rotation;
  }
}
