using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeRecollectionChamberController : MonoBehaviour, IChamberController {

  [SerializeField] private List<GameObject> spawns;
  [SerializeField] private List<GameObject> recollectables;
  [SerializeField] private GameObject player;
  [SerializeField] private GameObject exit;
  [SerializeField] private Animator playerAnimator;

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  public void initiallise(int level) {
    List<int> rateList = new List<int>();
    int totalRate = 0;
    for (int i = 0; i < recollectables.Count; i++) {
      totalRate += recollectables[i].GetComponent<Spawneable>().getSpawnRate(persistence.Instance.getLevel() - 1);
      rateList.Add(totalRate);
    }

    for (int i = 0; i < spawns.Count; i++) {
      float random = Random.value * totalRate;
      int k = 0;
      while (rateList[k] < random) k++;
      GameObject instancedObject = Instantiate(recollectables[k], spawns[i].transform.position, Quaternion.identity);
    }
    StartDungeonEnterAnimation();
  }

  public void StartDungeonEnterAnimation() {
    playerAnimator.SetFloat("Moving", 1);
    StartCoroutine(EnterDungeon());
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    //for (int i = 0; i < 1.5f / Time.fixedDeltaTime; i++) {
    //  player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 1.5f);
    //  yield return new WaitForSeconds(Time.fixedDeltaTime);
    //}

    playerAnimator.SetFloat("Moving", 0);
    //yield return new WaitForSeconds(0.5f);

    exit.SetActive(true);
    player.GetComponent<PlayerCombat>().enabled = true;
  }
}
