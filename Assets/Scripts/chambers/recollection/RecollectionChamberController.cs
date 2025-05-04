using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecollectionChamberController : MonoBehaviour, IChamberController {
  [SerializeField] private List<Transform> spawns;
  [SerializeField] private List<GameObject> recollectables;
  [SerializeField] private GameObject player;
  [SerializeField] private GameObject exit;
  [SerializeField] private Animator playerAnimator;

  [SerializeField] private float centerDistanceW = 1.5f;
  [SerializeField] private float centerDistanceH = 3.0f;


  private Vector3[] pos;

  public void OnExit() {
    GameManager.Instance.EnterMapScene();
  }

  private void Awake() {
    pos = new Vector3[] {
      new Vector3(-1.5f, 0, -3.75f),
      new Vector3(+0.0f, 0, -3.75f),
      new Vector3(+1.5f, 0, -3.75f),
      new Vector3(-1.5f, 0, +0.00f),
      new Vector3(+0.0f, 0, +0.00f),
      new Vector3(+1.5f, 0, +0.00f),
      new Vector3(-1.5f, 0, +3.75f),
      new Vector3(+0.0f, 0, +3.75f),
      new Vector3(+1.5f, 0, +3.75f),
    };
  }

  public void initiallise(int level) {
    List<int> rateList = new List<int>();
    int totalRate = 0;
    for (int i = 0; i < recollectables.Count; i++) {
      totalRate += recollectables[i].GetComponent<Spawneable>().getSpawnRate(level - 1);
      rateList.Add(totalRate);
    }

    for (int i = 0; i < spawns.Count; i++) {
      float random = Random.value * totalRate;
      int k = 0;
      while (rateList[k] < random) k++;
      for (int j = 0; j < 9; j++) {
        GameObject instancedObject = Instantiate(recollectables[k], spawns[i]);
        instancedObject.transform.localPosition = pos[j];
      }
    }
  }

  public void StartDungeonEnterAnimation() {
    playerAnimator.SetFloat("Moving", 1);
    StartCoroutine(EnterDungeon());
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    //for (float i = 0; i < 0.5f; i += Time.fixedDeltaTime) {
    //  player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 7.5f);
    //  yield return new WaitForSeconds(Time.fixedDeltaTime);
    //}

    playerAnimator.SetFloat("Moving", 0);
    //yield return new WaitForSeconds(0.5f);

    exit.SetActive(true);
    player.GetComponent<PlayerCombat>().enabled = true;
  }
}
