using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class MissionUIManager : MonoBehaviour
{
    [Header("Mission variables")]
    private RecipeController _missionController;
    private List<Mission> missionTemp;
    private bool[] missionStatus;

    [Header("UI References")]
    public GameObject itemUIPrefab;
    public Transform contentParent;

    void Awake()
    {
        _missionController = GetComponent<RecipeController>();
        missionTemp = _missionController.GetMissions();
        missionStatus = new bool[missionTemp.Count];
    }

    //public void RefreshMissionUI()
    //{
    //    missionTemp = _missionController.GetMissions();
    //    for (int i = 0; i < missionTemp.Count; i++)
    //    {
    //        try
    //        {
    //            InventorySafeController inventory = InventorySafeController.Instance;
    //            Mission mission = missionTemp[i];
    //            Items item = mission.getItems()[0].GetItem();
    //            int quantityItem = mission.getItems()[0].GetQuantity();
    //            int quantityMoney = mission.getPrice();
    //            int quantityInventory = inventory.getQuantity(item);
    //
    //            spriteItemMission[i].texture = item.icon;
    //            string itemString = item.itemName;
    //            textItemQuantity[i].text = quantityItem.ToString();
    //            textMoneyQuantity[i].text = quantityMoney.ToString();
    //            textInventoryQuantity[i].text = quantityInventory.ToString();
    //
    //
    //            textMission[i].text = string.Format("Loot <color=yellow>{0}</color> number of {1} to obtain the following reward: ",
    //                                    quantityItem.ToString(),
    //                                    itemString
    //                                    );
    //
    //
    //            bool missionCompletable = inventory.hasItem(mission.getItems()[0].GetItem(), mission.getItems()[0].GetQuantity());
    //            if(missionCompletable){
    //                buttonTextMission[i].text = "Complete mission";
    //                buttonMission[i].enabled = true;
    //            }
    //            else
    //            {
    //                buttonTextMission[i].text = "Can't complete";
    //                buttonMission[i].enabled = false;
    //            }
    //            missionStatus[i] = missionCompletable;
    //            
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.Log(e);
    //            print("error index out of bounds: " + i);
    //        }
    //    }
    //}
    public bool ObtainMissionStatus(int missionIndex)
    {
        return missionStatus[missionIndex];
    }

    public void MissionAction(int missionIndex)
    {
        if (missionStatus[missionIndex])
        {
            AudioManager.Instance.PlaySFXConfirmation();
            _missionController.completeMission(missionIndex);
            RefreshMissionUI();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

   public void RefreshMissionUI()
   {
        missionTemp = _missionController.GetMissions();
        ClearExistingUI();
        //FillMissionStatus(missionStatus, missionTemp);

        for (int i = 0; i < missionTemp.Count; i++)
        {
            GameObject newItem = Instantiate(itemUIPrefab, contentParent);
            missionStatus[i] = SetupItemUI(newItem, missionTemp[i], i);
        }
    }
   
   private void ClearExistingUI()
   {
   foreach (Transform child in contentParent)
        {
        Destroy(child.gameObject);
        }
   }
    
   private bool SetupItemUI(GameObject uiElement, Mission mis, int missionStep)
   {
        // Configurar nombre
        TextMeshProUGUI nameText = uiElement.transform.Find("mission-backpanel/text-mask/tmp-mission-name").GetComponent<TextMeshProUGUI>();
        nameText.text = mis.getTitle();

        // Configurar dinero 
        TextMeshProUGUI moneyText = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        moneyText.text = mis.getPrice().ToString();

        // Configurar items 
        Transform itemsContainer = uiElement.transform.Find("mission-backpanel/layout-items");
        InventorySafeController inventory = InventorySafeController.Instance;
        bool status = SetupIngredients(mis.getItems(), itemsContainer, inventory);

        //Configurar botones
        TextMeshProUGUI buttonText = uiElement.transform.Find("complete-button/tmp-mission-complete").GetComponent<TextMeshProUGUI>();
        if (status)
            buttonText.text = "Complete Recipe";
        else
            buttonText.text = "Can't complete";

        Button button = uiElement.transform.Find("complete-button").GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            MissionAction(missionStep);
        });

        return status;
    }

    //void SetupItems(List<RecipeItem> items, Transform container, InventorySafeController inventory)
    //{
    //    for (int i = 0; i < items.Count && i < 3; i++)
    //    {
    //         Transform itemSlot = container.GetChild(i);
    //
    //         //Mission items
    //         TextMeshProUGUI itemMissionAmount = itemSlot.Find("item-quantity-box/item-mission-quantity-text").GetComponent<TextMeshProUGUI>();
    //         itemMissionAmount.text = items[i].GetQuantity().ToString();
    //
    //         //Inventory items 
    //         //TextMeshProUGUI itemInventoryAmount = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
    //         TextMeshProUGUI itemInventoryAmount = itemSlot.Find("item-quantity-box/item-inventory-quantity-text").GetComponent<TextMeshProUGUI>();
    //         int quantityInventory = inventory.getQuantity(items[i].GetItem());
    //         itemInventoryAmount.text = quantityInventory.ToString();
    //
    //         //Sprite items
    //         RawImage spriteItem = itemSlot.GetComponentInChildren<RawImage>();
    //         spriteItem.texture = items[i].GetItem().icon;
    //         
    //         
    //
    //         itemSlot.gameObject.SetActive(true);
    //    }
    //    for(int i = 0; i < 3 - items.Count; i++)
    //    {
    //         Transform itemSlot = container.GetChild(2-i);
    //         itemSlot.gameObject.SetActive(false);
    //     }
    //}

    private bool SetupIngredients(List<RecipeItem> items, Transform container, InventorySafeController inventory)
    {
        int missingItems = 0;
        for (int i = 0; i < items.Count && i < 3; i++)
        {
            Transform itemSlot = container.GetChild(i);

            try
            {
                int quantityMission = items[i].GetQuantity();
                int quantityInventory = inventory.getQuantity(items[i].GetItem());
                missingItems += quantityMission - quantityInventory;

                // Mission items
                var missionBox = itemSlot.Find("item-quantity-box");
                if (missionBox == null)
                {
                    Debug.LogError($"Mission box not found in iteration {i}");
                    continue;
                }

                var missionText = missionBox.Find("item-mission-quantity-text");
                if (missionText != null)
                {
                    var itemMissionAmount = missionText.GetComponent<TextMeshProUGUI>();
                    itemMissionAmount.text = quantityMission.ToString();
                }

                // Inventory items 
                var inventoryText = missionBox.Find("item-inventory-quantity-text");
                if (inventoryText == null)
                {
                    Debug.LogError($"Inventory text not found in iteration {i}. Current children:");
                    foreach (Transform child in missionBox)
                    {
                        Debug.Log(child.name);
                    }
                    continue;
                }

                var itemInventoryAmount = inventoryText.GetComponent<TextMeshProUGUI>();
                if (itemInventoryAmount == null)
                {
                    Debug.LogError($"TextMeshPro component missing in iteration {i}");
                    continue;
                }
                
                //Color format
                if(quantityInventory < quantityMission)
                { 
                    itemInventoryAmount.color = Color.red;
                }else
                {
                    itemInventoryAmount.color = Color.green;
                }
                itemInventoryAmount.text = quantityInventory.ToString();

                // Sprite items
                var spriteItem = itemSlot.GetComponentInChildren<RawImage>();
                if (spriteItem != null)
                {
                    spriteItem.texture = items[i].GetItem().icon;
                }

                itemSlot.gameObject.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in iteration {i}: {e.Message}");
                continue;
            }
        }

        for (int i = 0; i < 3 - items.Count; i++)
        {
            Transform itemSlot = container.GetChild(2 - i);
            itemSlot.gameObject.SetActive(false);
        }

        if (missingItems > 0)
            return false;
        else 
            return true;
    }

}
