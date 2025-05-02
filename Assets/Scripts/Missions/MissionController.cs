
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    #region Variables
    //[SerializeField] private List<GameObject> PosibleItem;
    [SerializeField] private int numMissions;
    //[SerializeField] private List<int> missionDifficulty;
    [SerializeField] private List<int> missionPrice;

    [SerializeField] private List<ScriptableMission> posibleMissions;

    private InventorySafeController inventory;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        inventory = InventorySafeController.Instance;

    }
    #endregion

    #region Missions
    public List<Mission> GetMissions()
    {

        if (inventory == null)
        {
            inventory = InventorySafeController.Instance;

        }

        if(inventory.getMissions().Count == 0)
        {
            List<Mission> missions = new List<Mission>();
            for (int i = 0; i < numMissions; i++)
            {
                missions.Add(GenerateMission(i));

            }
            inventory.setsMissions(missions);
        }
        return inventory.getMissions();

    }
    public Mission GenerateMission(int index)
    {

        //List<int> rateList = new List<int>();
        //int totalRate = 0;
        //for (int i = 0; i < PosibleItem.Count; i++)
        //{
        //    Spawneable spw = PosibleItem[i].GetComponent<ObjectDrop>();
        //    int actualRate = 0;
        //    int levelsPerIndex = 15 / numMissions;
        //    for (int j = 0; j < levelsPerIndex; j++) {
        //        actualRate = spw.getSpawnRate(index * levelsPerIndex + j);
        //    }
        //    actualRate= actualRate/numMissions;
        //    totalRate += actualRate;

        //    rateList.Add(totalRate);
        //}
        //float random = Random.value * totalRate;
        //int k = 0;
        //while (rateList[k] < random) k++;
        //ObjectDrop item = PosibleItem[k].GetComponent<ObjectDrop>();
        //int value = item.getValue();
        //int quantityIn = Mathf.CeilToInt((float)(missionDifficulty[index]) / value);
        //int price = Mathf.CeilToInt((float)(0.75f + Random.value * 0.5f * missionPrice[index]));
        //Mission mis = new Mission(item.item, quantityIn, price);
        //return mis;

        List<ScriptableMission> missions= new List<ScriptableMission>();
        foreach (ScriptableMission miss in posibleMissions)
        {
            if (miss.level== index)
            {
                missions.Add(miss);
            }
        }
        int random = (int)(Random.value * missions.Count);
        ScriptableMission scrMiss = missions[random];
        List<RecipeItem> recipes= new List<RecipeItem>();
        foreach (missionItem missItem in scrMiss.items)
        {
            recipes.Add(new RecipeItem(missItem.item, missItem.quantity));
        }
        Mission mis = new Mission(recipes, missionPrice[index], scrMiss.title );
        return mis;

    }
    public void completeMission(int index) {
        Mission mis = inventory.getMissions()[index];
        bool correct = true;
        foreach (RecipeItem recpItem in mis.getItems())
        {
            if (!inventory.hasItem(recpItem.GetItem(), recpItem.GetQuantity()))
            {
                correct = false;

            }
        }
        if (correct)
        {
            foreach (RecipeItem recpItem in mis.getItems())
            {
                if (!inventory.hasItem(recpItem.GetItem(), recpItem.GetQuantity()))
                {
                    correct = false;

                }
            }

            inventory.addMoney(mis.getPrice());
            foreach (RecipeItem recpItem in mis.getItems())
            {
                inventory.removeItem(recpItem.GetItem(), recpItem.GetQuantity());
            }
            
            mis = GenerateMission(index);
            inventory.setMission(mis, index);
        }
    }
    #endregion

}
