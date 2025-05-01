using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> chests;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject exit;
    [SerializeField] private Animator playerAnimator;
    private bool openedChest = false;
    private List<GameObject> items;





    public void OnExit()
    {
        GameManager.Instance.EnterMapScene();
    }
    public void initiallise(int level)
    {
       

    }

    public void OpenChest()
    {
        if (!openedChest)
        {
            openedChest = true;

            for (int i = 0; i < chests.Count;i++)
            {

                chests[i].GetComponent<Interactable>().Deactivate();
            }

        }
    }
    public void StartDungeonEnterAnimation()
    {
        playerAnimator.SetFloat("Moving", 1);
        StartCoroutine(EnterDungeon());
    }
    IEnumerator EnterDungeon()
    {
        for (int i = 0; i < 1.5f / Time.fixedDeltaTime; i++)
        {
            player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 1.5f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        playerAnimator.SetFloat("Moving", 0);
        yield return new WaitForSeconds(0.5f);
        exit.SetActive(true);
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerCombat>().enabled = true;

    }
}
