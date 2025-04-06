
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    [SerializeField] private List<GameObject> PosibleItem;
    [SerializeField] private int numMissions;
    [SerializeField] private List<int> missionDifficulty;
    [SerializeField] private List<int> missionPrice;

    private InventorySafeController inventory;
    
    void Start()
    {
        inventory= InventorySafeController.Instance;

    }
    public List<Mission> GetMissions()
    {
        if (inventory != null)
        {
            if(inventory.getMissions().Count == 0)
            {
                List<Mission> missions = new List<Mission>();
                for (int i = 0; i < numMissions; i++)
                {
                    missions.Add(GenerateMission(i));

                }
                inventory.setsMissions(missions);
            }
        }
        return inventory.getMissions();
    }
    public Mission GenerateMission(int index)
    {

        List<int> rateList = new List<int>();
        int totalRate = 0;
        for (int i = 0; i < PosibleItem.Count; i++)
        {
            Spawneable spw = PosibleItem[i].GetComponent<ObjectDrop>();
            int actualRate = 0;
            for (int j = 0; j < numMissions; j++) {
                actualRate = spw.getSpawnRate(index * numMissions + j);
            }
            actualRate= actualRate/numMissions;
            totalRate += actualRate;

            rateList.Add(totalRate);
        }
        float random = Random.value * totalRate;
        int k = 0;
        while (rateList[k] < random) k++;
        ObjectDrop item = PosibleItem[k].GetComponent<ObjectDrop>();
        int value = item.getValue();
        int quantityIn = Mathf.CeilToInt((float)(missionDifficulty[index]) / value);
        int price = Mathf.CeilToInt((float)(0.75f + Random.value * 0.5f * missionPrice[index]));
        Mission mis = new Mission(item.item, quantityIn, price);
        return mis;

    }
    public void completeMission(int index) {
        Mission mis = inventory.getMissions()[index];
        if (inventory.hasItem(mis.getItem(), mis.getQuantity()))
        {


            inventory.addMoney(mis.getPrice());
            inventory.removeItem(mis.getItem(), mis.getQuantity());
        }
    }


}
