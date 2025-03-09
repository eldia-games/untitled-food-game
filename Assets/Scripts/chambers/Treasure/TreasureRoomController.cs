using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> chests;
    [SerializeField] private List<GameObject> loot;
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
        items = new List<GameObject>();
        List<int> rateList = new List<int>();
        int totalRate = 0;
        for (int i = 0; i < loot.Count; i++)
        {

            totalRate += loot[i].GetComponent<Spawneable>().getSpawnRate(persistence.Instance.getLevel() - 1);

            rateList.Add(totalRate);

        }
        for (int i = 0; i < chests.Count; i++)
        {
            float random = Random.value * totalRate;
            int j = 0;

            while (rateList[j] < random)
            {
                j++;
            }
            items.Add(loot[j]);

        }

    }

    public void OpenChest(GameObject chest)
    {
        if (!openedChest)
        {
            openedChest = true;
            chest.GetComponent<Animator>().SetBool("isClosed", false);
            GameObject itemToSpwn = null;
            for (int i = 0; i < chests.Count;i++)
            {
                if(chests[i] == chest)
                {
                    itemToSpwn= items[i];
                    Debug.Log(itemToSpwn);
                }
                chest.GetComponent<Interactable>().Desactive();
            }
            if (itemToSpwn != null)
            {
                GameObject objectCreated=Instantiate(itemToSpwn, chest.transform.position, Quaternion.identity);
                ObjectDrop objectdrop;
                if ((objectdrop = itemToSpwn.GetComponent<ObjectDrop>()) != null)
                {
                    objectdrop.quantity = (int)Mathf.Ceil((float)(persistence.Instance.getLevel()) / objectCreated.GetComponent<ObjectDrop>().getValue());
                }
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
