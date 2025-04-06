using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUIManager : MonoBehaviour
{
    [SerializeField] private RawImage[] spriteItemMission;
    [SerializeField] private TMP_Text[] textItemName;
    [SerializeField] private TMP_Text[] textMoneyQuantity;
    [SerializeField] private TMP_Text[] textItemQuantity;
    [SerializeField] private TMP_Text[] textInventoryQuantity;
    [SerializeField] private TMP_Text[] textMission;
    [SerializeField] private Button[] buttonMission;
    [SerializeField] private TMP_Text[] buttonTextMission;

    private MissionController missionController;
    private List<Mission> missionTemp;
    private bool[] missionStatus;

    void Start()
    {
        _missionController = getComponent<MissionController>();
    }

    public void RefreshMissionUI()
    {
        missionTemp = _missionController.getMissions();
        for (int i = 0; i < missionsRecieved.Count; i++)
        {
            try
            {
                Mission mission = missionsRecieved[i];
                Items item = mission.getItem();
                int quantityItem = mission.getQuantity();
                int quantityMoney = mission.getQuantityMoney();

                spriteItem[i].texture = item.icon;
                textItemName[i].text = item.itemName;
                textItemQuantity[i].text = quantityItem.ToString();
                textMoneyQuantity[i].text = quantityMoney.ToString();

              
                textMission[i].text = string.Format("Loot <color=yellow>{0}</color> number of {1} to obtain the following reward: ",
                                        quantityItem.ToString(),
                                        item.itemName
                                        );

                missionController = mission;
                InventoInventorySafeControllerryManager inventory = InventorySafeController.Instance;
                Mission mission = missionTemp[missionIndex];
                bool missionCompletable = inventory.HasItems(mission.getItem(), mission.getQuantity());
                if(missionCompletable){
                    buttonTextMission[i].text = "Complete mission";
                    buttonMission[i] = enabled;
                }
                else
                {
                    buttonTextMission[i].text = "Can't complete";
                    buttonMission[i] = disabled;
                }
                missionStatus[i] = missionCompletable;
                
            }
            catch
            {
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
        if (inventory.HasItems(mission.getItem(), mission.getQuantity()))
        {
            missionController.completeMission(missionIndex);
            return true;
        }
        else
        {
            return false;
        }
    }
}
