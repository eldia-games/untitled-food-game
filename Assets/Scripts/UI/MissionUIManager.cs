using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MissionUIManager : MonoBehaviour
{
    [SerializeField] private RawImage[] spriteItemMission;
    [SerializeField] private TMP_Text[] textMoneyQuantity;
    [SerializeField] private TMP_Text[] textItemQuantity;
    [SerializeField] private TMP_Text[] textInventoryQuantity;
    [SerializeField] private TMP_Text[] textMission;
    [SerializeField] private Button[] buttonMission;
    [SerializeField] private TMP_Text[] buttonTextMission;

    private MissionController _missionController;
    private List<Mission> missionTemp;
    private bool[] missionStatus;

    void Awake()
    {
        Debug.Log("estoy despierto");
        _missionController = GetComponent<MissionController>();
        missionTemp = _missionController.GetMissions();
        missionStatus = new bool[missionTemp.Count];
    }

    public void RefreshMissionUI()
    {
        missionTemp = _missionController.GetMissions();
        for (int i = 0; i < missionTemp.Count; i++)
        {
            try
            {
                InventorySafeController inventory = InventorySafeController.Instance;
                Mission mission = missionTemp[i];
                Items item = mission.getItems()[0].GetItem();
                int quantityItem = mission.getItems()[0].GetQuantity();
                int quantityMoney = mission.getPrice();
                int quantityInventory = inventory.getQuantity(item);

                spriteItemMission[i].texture = item.icon;
                string itemString = item.itemName;
                textItemQuantity[i].text = quantityItem.ToString();
                textMoneyQuantity[i].text = quantityMoney.ToString();
                textInventoryQuantity[i].text = quantityInventory.ToString();


                textMission[i].text = string.Format("Loot <color=yellow>{0}</color> number of {1} to obtain the following reward: ",
                                        quantityItem.ToString(),
                                        itemString
                                        );


                bool missionCompletable = inventory.hasItem(mission.getItems()[0].GetItem(), mission.getItems()[0].GetQuantity());
                if(missionCompletable){
                    buttonTextMission[i].text = "Complete mission";
                    buttonMission[i].enabled = true;
                }
                else
                {
                    buttonTextMission[i].text = "Can't complete";
                    buttonMission[i].enabled = false;
                }
                missionStatus[i] = missionCompletable;
                
            }
            catch (Exception e)
            {
                Debug.Log(e);
                print("error index out of bounds: " + i);
            }
        }
    }
    public bool ObtainMissionStatus(int missionIndex)
    {
        return missionStatus[missionIndex];
    }

    public bool MissionAction(int missionIndex)
    {
        InventorySafeController inventory = InventorySafeController.Instance;
        Mission mission = missionTemp[missionIndex];
        if (inventory.hasItem(mission.getItems()[0].GetItem(), mission.getItems()[0].GetQuantity()))
        {
            _missionController.completeMission(missionIndex);
            return true;
        }
        else
        {
            return false;
        }
    }
}
