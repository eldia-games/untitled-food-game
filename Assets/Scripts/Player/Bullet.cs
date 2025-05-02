using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  public float speed = 20f;
  public int damage = 1;
  public float pushForce = 1;
  public Rigidbody rb;
  public Collider hitbox;
  public float damageModifier = 1;
  public float pushModifier = 1;


  void Start() {
    rb = GetComponent<Rigidbody>();
  }

  void FixedUpdate() {
    rb.velocity = -transform.up * speed;
  }

  void OnTriggerEnter(Collider hitInfo) {
    if (hitInfo.tag == "Player") return;

    float totalDamage = damage * damageModifier;
    float totalPush = pushForce * pushModifier;
    Vector3 position = transform.position;

    if (hitInfo.tag == "Enemy") {
      hitInfo.GetComponent<BaseEnemy>()?.OnHurt(totalDamage, totalPush, position);
      Destroy(gameObject);
    } else if (hitInfo.tag == "Boss") {
      hitInfo.GetComponent<Boss>()?.OnHurt(totalDamage, totalPush, position);
      Destroy(gameObject);
    } else if (hitInfo.tag == "Wall") {
      Destroy(gameObject);
    }
  }
}
