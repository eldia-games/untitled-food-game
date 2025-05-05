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
  [SerializeField] private ObjectDrop Beer;

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



    for (int i = 0; i < 4; i++) {
      int j = 0;
      float random = Random.value * totalRateIn;
      while (rateListIn[j] < random) j++;

      int k = 0;
      float random2 = Random.value * totalRateOut;
      while (rateListOut[k] < random2) k++;

      ObjectDrop objectIn = ObjectsIn[j].GetComponent<ObjectDrop>();
      ObjectDrop objectOut = ObjectsOut[k].GetComponent<ObjectDrop>();
        if (i == 3)
        {
                objectOut = Beer;
        }

      int quantityIn = Mathf.CeilToInt((float)(level) / objectIn.getValue());
      int quantityOut = Mathf.CeilToInt((float)(level) / objectOut.getValue());

      trades_.Add(new Trade(objectIn.item, quantityIn, objectIn.indexLoot, objectOut.item, quantityOut, objectOut.indexLoot));
    }

    UIManager.Instance.RefreshShop(trades_, this);
    StartDungeonEnterAnimation();
  }

public void OpenShop()
{
        UIManager.Instance.RefreshShop(trades_, this);
        UIManager.Instance.ShowShopCanvas();
}
  public List<Trade> getTrades() {
    return trades_;
  }

  public void Trade(int tradeIndex) {
    if (!tradeDone_) {

            InventoryList inventory = InventoryList.Instance;
      Trade trad = trades_[tradeIndex];
      if (inventory.HasItems(trad.getItemIn(), trad.getQuantityIn()))
      {
        UIManager.Instance.HideShopCanvas();

     
        inventory.RemoveItemNoDrop(trad.getItemIn(), trad.getQuantityIn());
                if (tradeIndex == 3)
                {
                    InventoryList.Instance.addBeers(trad.getQuantityOut());
                    UIManager.Instance.ResetPlayerUiStats();
                }else
                {
                    inventory.AddItem(trad.getItemOut(), trad.getIndexOut(), trad.getQuantityOut(), true);
                }
            }
    }
  }

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
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
