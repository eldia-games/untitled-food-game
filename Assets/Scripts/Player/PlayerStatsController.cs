using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour {
  [SerializeField] private PlayerStats stats;
  public static PlayerStatsController Instance { get; private set; }

  private void Awake() {
    if (Instance != null && Instance != this) {
      Destroy(gameObject);
    } else {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  public void heal(float heal) {
    stats.HP = Mathf.Min(stats.HP + heal, stats.maxLife);
  }

  public void healprct(float heal) {
    stats.HP = Mathf.Min(stats.HP + stats.maxLife * heal, stats.maxLife);
  }

  public void augmentMaxHealht(int healht) {
    stats.maxLife += healht;
  }

  public void augmentMaxHealhtprct(float healht) {
    stats.maxLife = (int)Mathf.Round(stats.maxLife * (1 + healht));
  }

  public void augmentHeal(float heal) {
    stats.heal += heal;
  }

  public void augmentHealprct(float heal) {
    stats.heal = Mathf.Round(stats.heal * (1 + heal));
  }

  public void augmentMaxDamage(float damage) {
    stats.damage += damage;
  }

  public void augmentMaxDamageprct(float damage) {
    stats.damage = Mathf.Round(stats.damage * (1 + damage));
  }

  public void augmentAttackSpeed(float attackSpeed) {
    stats.velAttack += attackSpeed;
  }

  public void augmentAttackSpeedprct(float attackSpeed) {
    stats.velAttack = Mathf.Round(stats.velAttack * (1 + attackSpeed));
  }

  public void augmentMaxmoveSpeed(float speed) {
    stats.MovementSpeed += speed;
  }

  public void augmentMaxmoveSpeedprct(float speed) {
    stats.MovementSpeed = Mathf.Round(stats.MovementSpeed * (1 + speed));
  }

  public void augmentPushForce(float pushForce) {
    stats.pushForce += pushForce;
  }

  public void augmentPushForceprct(float pushForce) {
    stats.pushForce = Mathf.Round(stats.pushForce * (1 + pushForce));
  }

  public void augmentMaxMana(int mana) {
    stats.maxMana += mana;
  }

  public void augmentMaxManaprct(float mana) {
    stats.maxMana = (int)Mathf.Round(stats.maxMana * (1 + mana));
  }

  public void augmentManaRegen(float regen) {
    stats.manaRegen += regen;
  }

  public void augmentManaRegenrct(float regen) {
    stats.manaRegen = Mathf.Round(stats.manaRegen * (1 + regen));
  }
}
