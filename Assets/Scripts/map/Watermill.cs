using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermill : MonoBehaviour {
  [SerializeField] private float speed;

  private Transform transform_;

  void Start() {
    transform_ = transform;
  }

  void FixedUpdate() {
    transform_.Rotate(Vector3.forward * speed * Time.fixedDeltaTime);
  }
}
