using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BossChamberController : MonoBehaviour, IChamberController {
    [SerializeField] private float AnimationMovementSpped;
    [SerializeField] private float AnimationMovementTime;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject Boss;
    [SerializeField] private GameObject Door;
    private Animator doorAnimator;




    void Start()
    {
        doorAnimator = Door.GetComponent<Animator>();
    }





    public void StartDungeonEnterAnimation() {
    
    StartCoroutine(EnterDungeon());
  }

  // Update is called once per frame
  public void initiallise(int level) {

    StartDungeonEnterAnimation();
  }

  public void killBoss() {

        exit.SetActive(true);
        StartCoroutine(OpenDoor());
    
  }

 

  public void OnExit(GameObject exit /* unused */) {
    UIManager.Instance.ShowVictoryCanvas();
  }

  IEnumerator EnterDungeon() {
    yield return new WaitForSeconds(0.5f);
    playerAnimator.SetFloat("Moving", 1);
    for (int i = 0; i < AnimationMovementTime / Time.fixedDeltaTime; i++)
    {
        player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * AnimationMovementSpped);
        yield return new WaitForSeconds(Time.fixedDeltaTime);
    }
    playerAnimator.SetFloat("Moving", 0);
    yield return new WaitForSeconds(0.5f);
   
    doorAnimator.SetBool("Closed", true);
   
    yield return new WaitForSeconds(2);
    player.GetComponent<PlayerCombat>().enabled = true;
   
    Boss en = Boss.GetComponent<Boss>();
    en.SetPlayer(player);
    en.dieEvent = new UnityEngine.Events.UnityEvent();
    en.dieEvent.AddListener(killBoss);

    
  }

  IEnumerator OpenDoor() {
    yield return new WaitForSeconds(2);
      doorAnimator.SetBool("Closed", false);
  }

  IEnumerator CloseDoor() {
    yield return new WaitForSeconds(2);
      doorAnimator.SetBool("Closed", true);
    
  }
}
