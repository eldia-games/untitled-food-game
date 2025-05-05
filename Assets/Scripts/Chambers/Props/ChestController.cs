using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour {

  [SerializeField] private List<GameObject> loot;
  [SerializeField] private UnityEvent<GameObject> openFunction;

  private GameObject chosenItem_;

  void Start() {
    int lootIndex = (int)(Mathf.Floor(Random.value * loot.Count));
    chosenItem_ = loot[lootIndex];
  }

  public void OnOpen() {
    if (chosenItem_ == null) return;

    GameObject objectCreated = Instantiate(chosenItem_, transform.position, Quaternion.identity);
    ObjectDrop objectdrop;
    GetComponent<Animator>().SetBool("isClosed", false);
    AudioManager.Instance.PlayUseChest();
    GetComponent<Interactable>().Deactivate();

    if ((objectdrop = chosenItem_.GetComponent<ObjectDrop>()) != null)
      objectdrop.quantity = Mathf.CeilToInt(persistence.Instance.getLevel() / (float)objectCreated.GetComponent<ObjectDrop>().getValue());

    if (openFunction != null)
      openFunction.Invoke(gameObject);
  }
}
