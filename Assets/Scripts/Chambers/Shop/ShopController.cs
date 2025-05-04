using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ShopController : MonoBehaviour, IChamberController {
  [SerializeField] private List<GameObject> ObjectsIn;
  [SerializeField] private List<GameObject> ObjectsOut;
  [SerializeField] private GameObject exit;
  [SerializeField] private GameObject player;
  [SerializeField] private Animator playerAnimator;
  [SerializeField] private float AnimationMovementSpped;
  [SerializeField] private float AnimationMovementTime;

  private List<Trade> trades_;
  private bool tradeDone_ = false;

  public void StartDungeonEnterAnimation() {
    StartCoroutine(EnterDungeon());
  }

  public void initiallise(int level) {
    trades_ = new List<Trade>();
    int totalRateIn = 0;
    List<int> rateListIn = new List<int>();
    for (int i = 0; i < ObjectsIn.Count; i++) {
      totalRateIn += ObjectsIn[i].GetComponent<ObjectDrop>().getSpawnRate(level - 1);
      rateListIn.Add(totalRateIn);
    }

    int totalRateOut = 0;
    List<int> rateListOut = new List<int>();
    for (int i = 0; i < ObjectsOut.Count; i++) {
      totalRateOut += ObjectsOut[i].GetComponent<ObjectDrop>().getSpawnRate(level - 1);
      rateListOut.Add(totalRateOut);
    }

    for (int i = 0; i < 3; i++) {
      int j = 0;
      float random = Random.value * totalRateIn;
      while (rateListIn[j] < random) j++;

      int k = 0;
      float random2 = Random.value * totalRateOut;
      while (rateListOut[k] < random2) k++;

      ObjectDrop objectIn = ObjectsIn[j].GetComponent<ObjectDrop>();
      ObjectDrop objectOut = ObjectsOut[k].GetComponent<ObjectDrop>();

      int quantityIn = Mathf.CeilToInt((float)(level) / objectIn.getValue());
      int quantityOut = Mathf.CeilToInt((float)(level) / objectOut.getValue());

      trades_.Add(new Trade(objectIn.item, quantityIn, objectIn.indexLoot, objectOut.item, quantityOut, objectOut.indexLoot));
    }

    UIManager.Instance.RefreshShop(trades_, this);
    StartDungeonEnterAnimation();
  }

  public List<Trade> getTrades() {
    return trades_;
  }

  public void Trade(int tradeIndex) {
    if (!tradeDone_) {
      UIManager.Instance.HideShopCanvas();
      InventoryManager inventory = InventoryManager.Instance;
      Trade trad = trades_[tradeIndex];
      if (inventory.HasItems(trad.getItemIn(), trad.getQuantityIn())) {
        inventory.RemoveItemNoDrop(trad.getItemIn(), trad.getQuantityIn());
        inventory.AddItem(trad.getItemOut(), trad.getIndexOut(), trad.getQuantityOut(), true);
      }
    }
  }

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    //playerAnimator.SetFloat("Moving", 1);
    //for (int i = 0; i < AnimationMovementTime / Time.fixedDeltaTime; i++) {
    //  player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * AnimationMovementSpped);
    //  yield return new WaitForSeconds(Time.fixedDeltaTime);
    //}
    playerAnimator.SetFloat("Moving", 0);
    //yield return new WaitForSeconds(0.5f);

    player.GetComponent<PlayerCombat>().enabled = true;
    exit.SetActive(true);
  }
}
