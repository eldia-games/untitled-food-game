using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BossChamberController : MonoBehaviour, IChamberController {
  [SerializeField] private GameObject exit;
  [SerializeField] private GameObject player;
  [SerializeField] private Animator playerAnimator;
  [SerializeField] private GameObject boss;
  [SerializeField] private GameObject door;
   [Header("Achievements")]
    [SerializeField] private ScriptableAchievement Sword1;
    [SerializeField] private ScriptableAchievement Sword10;
    [SerializeField] private ScriptableAchievement Axe1;
    [SerializeField] private ScriptableAchievement Axe10;
    [SerializeField] private ScriptableAchievement Bow1;
    [SerializeField] private ScriptableAchievement Bow10;
    [SerializeField] private ScriptableAchievement Staff1;
    [SerializeField] private ScriptableAchievement Staff10;
    private Animator doorAnimator;

  void Start() {
    doorAnimator = door.GetComponent<Animator>();
  }

  public void initiallise(int level) {
    StartCoroutine(EnterDungeon());
  }

  public void killBoss() {
    exit.SetActive(true);
    StartCoroutine(OpenDoor());
  }

  public void OnExit(GameObject exit /* unused */) {
    UIManager.Instance.ShowVictoryCanvas();
        switch (GameManager.Instance.getCurrentWeaponType())
        {
            case 0: //espada
                AchievementController.Instance.stepAchievement(Sword1);
                AchievementController.Instance.stepAchievement(Sword10);
                break;
            case 1: // hacha
                AchievementController.Instance.stepAchievement(Axe1);
                AchievementController.Instance.stepAchievement(Axe10);
                break;
            case 2: // bayesta
                AchievementController.Instance.stepAchievement(Bow1);
                AchievementController.Instance.stepAchievement(Bow10);
                break;
            case 4: // staff  
                AchievementController.Instance.stepAchievement(Staff1);
                AchievementController.Instance.stepAchievement(Staff10);
                break;
            default:
                break;
        }
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.1f);

    playerAnimator.SetFloat("Moving", 1);
    for (int i = 0; i < 0.5f / Time.fixedDeltaTime; i++) {
      player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 10f);
      yield return new WaitForSeconds(Time.fixedDeltaTime);
    }

    playerAnimator.SetFloat("Moving", 0);
    yield return new WaitForSeconds(0.1f);

    doorAnimator.SetBool("Closed", true);
    AudioManager.Instance.PlayMoveDoor();
    yield return new WaitForSeconds(0.25f);

    player.GetComponent<PlayerCombat>().enabled = true;
    Boss en = boss.GetComponent<Boss>();
    en.SetPlayer(player);
    en.dieEvent = new UnityEngine.Events.UnityEvent();
    en.dieEvent.AddListener(killBoss);
  }

  IEnumerator OpenDoor() {
    yield return new WaitForSeconds(2);
    doorAnimator.SetBool("Closed", false);
    AudioManager.Instance.PlayMoveDoor();
  }
}
