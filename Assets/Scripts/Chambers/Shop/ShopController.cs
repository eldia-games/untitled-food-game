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

    //[SerializeField] private GameObject Seller;


    private List<Trade> trades;
    private bool TradeDone = false;












    public void StartDungeonEnterAnimation() {
    StartCoroutine(EnterDungeon());
  }

  // Update is called once per frame
  public void initiallise(int level) {
        trades = new List<Trade>();
        int totalRateIn = 0;
        List<int> rateListIn = new List<int>();
        for (int i = 0; i < ObjectsIn.Count; i++)
        {
            totalRateIn += ObjectsIn[i].GetComponent<ObjectDrop>().getSpawnRate(level - 1);
            rateListIn.Add(totalRateIn);
        }

        int totalRateOut = 0;
        List<int> rateListOut = new List<int>();
        for (int i = 0; i < ObjectsOut.Count; i++)
        {
            totalRateOut += ObjectsOut[i].GetComponent<ObjectDrop>().getSpawnRate(level - 1);
            rateListOut.Add(totalRateOut);
        }

        for (int i = 0;i < 3; i++)
        {
            float random = Random.value * totalRateIn;
            int j = 0;
            while (rateListIn[j] < random) j++;
            float random2 = Random.value * totalRateOut;
            int k = 0;
            while (rateListOut[k] < random2) k++;
            ObjectDrop objectIn = ObjectsIn[j].GetComponent<ObjectDrop>();
            int quantityIn= Mathf.CeilToInt((float)(level)/objectIn.getValue());
            ObjectDrop objectOut = ObjectsOut[k].GetComponent<ObjectDrop>();
            int quantityOut = Mathf.CeilToInt((float)(level) / objectOut.getValue());

            trades.Add(new Trade(objectIn.item, quantityIn, objectIn.indexLoot, objectOut.item, quantityOut, objectOut.indexLoot));



        }
        UIManager.Instance.refreshShop(trades,this);
        StartDungeonEnterAnimation();
  }

  
   public void OpenShop()
    {
        UIManager.Instance.ShowShopCanvas();
    }

    public void Trade(int tradeIndex)
    {

        if (!TradeDone)
        {
            UIManager.Instance.HideShopCanvas();
            InventoryManager inventory = InventoryManager.Instance;
            Trade trad = trades[tradeIndex];
            if (inventory.HasItems(trad.getItemIn(), trad.getQuantityIn()))
            {
                inventory.RemoveItemNoDrop(trad.getItemIn(), trad.getQuantityIn());
                inventory.AddItem(trad.getItemOut(), trad.getIndexOut(), trad.getQuantityOut(), true);
            }
        }

    }
  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.5f);
    playerAnimator.SetFloat("Moving", 1);
    for (int i = 0; i < AnimationMovementTime / Time.fixedDeltaTime; i++) {
      player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * AnimationMovementSpped);
      yield return new WaitForSeconds(Time.fixedDeltaTime);
    }
    playerAnimator.SetFloat("Moving", 0);
    yield return new WaitForSeconds(0.5f);

    player.GetComponent<PlayerCombat>().enabled = true;
    exit.SetActive(true);
    
  }

  
}
