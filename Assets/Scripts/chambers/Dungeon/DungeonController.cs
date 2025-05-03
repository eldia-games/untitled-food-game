using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonController : MonoBehaviour, IChamberController {
  [SerializeField] private List<GameObject> monsters;
  [SerializeField] private bool trap;
  [SerializeField, Range(1, 100)] private int baseEnemyAmount;

  private List<GameObject> doors;
  private GameObject lever;
  private GameObject exit;
  private List<Vector3> spawns;
  private GameObject player;
  private Animator playerAnimator;
  private List<Animator> doorAnimator;
  private Animator leverAnimator;
  private Vector3[] pos;
  private List<int> monstersSpawned;
  private bool leverUsed = false;
  private int enemiesLeft;
  private List<GameObject> monsterList;

  [SerializeField] private DungeonGenerator generator;

  void Awake() {
    monstersSpawned = new List<int>();
    //generator_ = GetComponent<DungeonGenerator>();
    doors = new List<GameObject>();
    spawns = new List<Vector3>();
    doorAnimator = new List<Animator>();
    pos = new Vector3[] { 
      new Vector3(-1.5f, 0, -1.5f),
      new Vector3(+0.0f, 0, -1.5f),
      new Vector3(+1.5f, 0, -1.5f),
      new Vector3(-1.5f, 0, +0.0f),
      new Vector3(+0.0f, 0, +0.0f),
      new Vector3(+1.5f, 0, +0.0f),
      new Vector3(-1.5f, 0, +1.5f),
      new Vector3(+0.0f, 0, +1.5f),
      new Vector3(+1.5f, 0, +1.5f),
    };
  }

  void Start() {}

  public void SetTrap(bool trap) {
    this.trap = trap;
  }

  public void AddDoor(GameObject door) {
    doors.Add(door);
    doorAnimator.Add(door.GetComponent<Animator>());
  }

  public void AddSpawn(Vector3 spawn) {
    spawns.Add(spawn);
    monstersSpawned.Add(0);
  }

  public void SetPlayer(GameObject player, Animator playerAnimator) {
    this.player = player;
    this.playerAnimator = playerAnimator;
  }

  public void SetLever(GameObject lever) {
    this.lever = lever;
    leverAnimator = lever.GetComponent<Animator>();
    UnityEvent<GameObject> uevent = new UnityEvent<GameObject>();
    uevent.AddListener(UseLever);
    lever.GetComponent<Interactable>().SetAction(uevent, "Use lever", InteractionType.NormalInteraction);
  }

  public void SetExit(GameObject exit) {
    this.exit = exit;
    UnityEvent<GameObject> uevent = new UnityEvent<GameObject>();
    uevent.AddListener(OnExit);
    exit.GetComponent<Interactable>().SetAction(uevent, "Use exit", InteractionType.NormalInteraction);
  }

  public void StartDungeonEnterAnimation() {
    //playerAnimator.SetFloat("Moving", 1);
    StartCoroutine(EnterDungeon());
  }

  // Update is called once per frame
  public void initiallise(int level) {
    generator.Initialize(this);
    monsterList = new List<GameObject>();
    List<int> rateList = new List<int>();
    int totalRate = 0;
    for (int i = 0; i < monsters.Count; i++) {
      if (monsters[i] == null) continue;
      totalRate += monsters[i].GetComponent<Spawneable>().getSpawnRate(level - 1);
      rateList.Add(totalRate);
    }
    float forceLeft = level * baseEnemyAmount;
    while (spawns.Count > 0) {
      if (monstersSpawned.Min() >= 9) break;
      int spawnPoint = (int)Mathf.Round(Random.value * (spawns.Count - 1));
      while (monstersSpawned[spawnPoint] >= 9) {
        spawnPoint = (int)Mathf.Round(Random.value * (spawns.Count - 1));
      }

      if (forceLeft <= 0) break;

      float random = Random.value * totalRate;
      int i = 0;
      while (rateList[i] < random) i++;
      int value = monsters[i].GetComponent<Spawneable>().getValue();
      // Asignar el player a los enemigos
      // monsters[i].GetComponent<BaseEnemy>().SetPlayer(player);
      if (value <= forceLeft) {
        forceLeft -= value;

        GameObject instancedObject = Instantiate(monsters[i], spawns[spawnPoint] + pos[monstersSpawned[spawnPoint]], Quaternion.identity);
        monsterList.Add(instancedObject);
        monstersSpawned[spawnPoint] += 1;
        enemiesLeft++;
      }
    }
    StartDungeonEnterAnimation();
  }

  public void killEnemy() {
    if (--enemiesLeft == 0 && trap) StartCoroutine(OpenDoor());
  }

  public void UseLever(GameObject leverActual) {
    if (leverActual == lever && !leverUsed) {
      leverUsed = true;
      leverAnimator.SetBool("LeverLeft", !leverAnimator.GetBool("LeverLeft"));
      AudioManager.Instance.PlayMoveLever();
      exit.SetActive(true);
      StartCoroutine(OpenDoor());
    }
  }

  public void OnExit(GameObject exit /* unused */) {
    GameManager.Instance.EnterMapScene();
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    playerAnimator.SetFloat("Moving", 1);
    for (int i = 0; i < 0.5f / Time.fixedDeltaTime; i++) {
      player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 10);
      yield return new WaitForSeconds(Time.fixedDeltaTime);
    }

    playerAnimator.SetFloat("Moving", 0);
    yield return new WaitForSeconds(0.1f);

    for (int i = 0; i < doorAnimator.Count; i++) doorAnimator[i].SetBool("Closed", true);
    AudioManager.Instance.PlayMoveDoor();
    yield return new WaitForSeconds(0.25f);

    player.GetComponent<PlayerCombat>().enabled = true;
    for (int i = 0; i < monsterList.Count; i++) {
      BaseEnemy enemy = monsterList[i].GetComponent<BaseEnemy>();
      enemy.SetPlayer(player);
      enemy.dieEvent = new UnityEngine.Events.UnityEvent();
      enemy.dieEvent.AddListener(killEnemy);
      // if (trap) enemy.canDrop = false;
    }
  }

  IEnumerator OpenDoor() {
    yield return new WaitForSeconds(0.1f);
    for (int i = 0; i < doorAnimator.Count; i++) doorAnimator[i].SetBool("Closed", false);
    AudioManager.Instance.PlayMoveDoor();
  }
}
