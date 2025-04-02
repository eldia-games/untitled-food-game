using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChamber : MonoBehaviour {
  [SerializeField] private GameObject exit;
  [SerializeField] private GameObject lever;
  [SerializeField] private GameObject player;
  [SerializeField] private Animator playerAnimator;
  [SerializeField] private GameObject[] doors;
  [SerializeField] private GameObject[] spawns;

  public void Create(DungeonController controller) {
    if (exit   != null) controller.SetExit(exit);
    if (lever  != null) controller.SetLever(lever);
    if (player != null) controller.SetPlayer(player, playerAnimator);
    foreach (GameObject door in doors) controller.AddDoor(door);
    foreach (GameObject spawn in spawns) controller.AddSpawn(spawn);
  }
}
