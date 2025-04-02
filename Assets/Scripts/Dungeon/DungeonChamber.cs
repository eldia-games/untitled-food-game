using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChamber : MonoBehaviour {
  [SerializeField] private GameObject exit;

  [SerializeField] private GameObject player;
  [SerializeField] private Animator playerAnimator;

  [SerializeField] private Transform[] enemySpawns;

  [SerializeField] private GameObject lever;
  [SerializeField] private Transform[] leverSpawns;
  [SerializeField] private GameObject[] doors;

  public void Create(DungeonController controller, bool lever) {
    if (controller == null) return;

    if (exit   != null) controller.SetExit(exit);
    if (player != null) controller.SetPlayer(player, playerAnimator);
    if (lever) controller.SetLever(CreateLever());
    foreach (GameObject door in doors) controller.AddDoor(door);
    foreach (Transform spawn in enemySpawns) controller.AddSpawn(spawn.position);
  }

  private GameObject CreateLever() {
    int r = Random.Range(0, leverSpawns.Length);
    return Instantiate(lever, leverSpawns[r]);
  }
}
