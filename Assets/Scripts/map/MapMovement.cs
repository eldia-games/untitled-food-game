using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MapMovement : MonoBehaviour {
  [SerializeField] private float rotationSpeed;
  [SerializeField] private float movementSpeed;
  [SerializeField] private float reachDelay;

  private bool lock_;
  private Vector3 target_;
  private float targetTime_;
  private bool targetReached_;

  private Animator animator_;
  private Transform transform_;
  private Transform childTransform_;

  // Start is called before the first frame update
  void Start() {
    lock_ = false;
    target_ = Vector3.zero;
    targetTime_ = 0.0f;
    targetReached_ = false;

    animator_ = GetComponentInChildren<Animator>();
    transform_ = transform;
    childTransform_ = transform.GetChild(0);
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (target_ == Vector3.zero) return;

    Vector3 movement = target_ - transform_.position;
    Quaternion lookAt = Quaternion.LookRotation(movement, Vector3.up);
    childTransform_.rotation = Quaternion.RotateTowards(childTransform_.rotation, lookAt, rotationSpeed * Time.fixedDeltaTime);
    if (targetReached_) return;

    transform_.position = Vector3.MoveTowards(transform_.position, target_, movementSpeed * Time.fixedDeltaTime);
    float distance = movement.magnitude;
    animator_.SetFloat("distance", distance);
    if (distance > 1e-6f) return;

    target_ += Vector3.forward;
    targetTime_ = Time.time;
    targetReached_ = true;
  }

  public void SetTarget(Vector3 position) {
    lock_ = true;
    target_ = position;
  }

  public void ClearTarget() {
    lock_ = false;
    target_ = Vector3.zero;
  }

  public bool IsLocked() {
    return lock_;
  }

  public bool TargetReached() {
    return targetReached_ && Time.time - targetTime_ > reachDelay;
  }
}
