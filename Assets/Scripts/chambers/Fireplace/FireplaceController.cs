using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceController : MonoBehaviour, IChamberController {
  [SerializeField] private GameObject player;
  [SerializeField] private CinemachineDollyCart cart;
  [SerializeField] private GameObject exit;
  [SerializeField] private GameObject fire;
  [SerializeField] private Animator playerAnimator;

  private bool healed = false;
  private int healLevel = 0;

  public void initiallise(int level) {
    healLevel = 7 * level;
    StartDungeonEnterAnimation();
  }

  public void Heal(GameObject fireplace) {
    if (!healed) {
      healed = true;

      PlayerStatsController.Instance.healprct(healLevel);
      fireplace.GetComponent<Interactable>().Deactivate();
      InventoryList.Instance.refillBeers();
      UIManager.Instance.ResetPlayerUiStats();
    }
  }

  public void StartMovingCart() {
    cart.m_Position = 0;
    cart.m_Speed = 2;
  }

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  public void StartDungeonEnterAnimation() {

    StartCoroutine(EnterDungeon());
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

  


        playerAnimator.SetFloat("Moving", 1);
        for (int i = 0; i < 0.5f / Time.fixedDeltaTime; i++)
        {
            player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 4);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        playerAnimator.SetFloat("Moving", 0);
        yield return new WaitForSeconds(0.1f);
        exit.SetActive(true);

        player.GetComponent<PlayerCombat>().enabled = true;
        
    }
}
