using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> chests;
    [SerializeField] private List<GameObject> loot;
    [SerializeField] private GameObject player;
    private bool openedChest = false;
    private List<GameObject> items;


    public void Start()
    {
        
        player.GetComponent<PlayerCombat>().enabled = true;
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
            Debug.Log(j);
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
}
