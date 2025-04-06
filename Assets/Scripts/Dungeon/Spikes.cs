using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
  [SerializeField] private int damage = 10;
  [SerializeField] private float knockback = 5f;

  private void OnTriggerEnter(Collider other) {
    if (!other.CompareTag("Player")) return;

    PlayerCombat playerScript = other.GetComponent<PlayerCombat>();
    playerScript?.OnHurt(damage, knockback, transform.position);
  }
}
