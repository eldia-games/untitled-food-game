using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomController : MonoBehaviour, IChamberController {
  [SerializeField] private List<GameObject> chests;
  [SerializeField] private GameObject player;
  [SerializeField] private GameObject exit;
  [SerializeField] private Animator playerAnimator;
  private bool openedChest = false;

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  public void initiallise(int level /* unused */) {
    openedChest = false;
  }

  public void OpenChest() {
    if (openedChest) return;
    openedChest = true;
    for (int i = 0; i < chests.Count; i++)
      chests[i].GetComponent<Interactable>().Deactivate();
  }

  public void StartDungeonEnterAnimation() {
    playerAnimator.SetFloat("Moving", 1);
    StartCoroutine(EnterDungeon());
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    //for (int i = 0; i < 0.5f / Time.fixedDeltaTime; i++) {
    //  player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 10f);
    //  yield return new WaitForSeconds(Time.fixedDeltaTime);
    //}

    playerAnimator.SetFloat("Moving", 0);
    //yield return new WaitForSeconds(0.1f);

    exit.SetActive(true);
    player.GetComponent<PlayerCombat>().enabled = true;
  }
}
