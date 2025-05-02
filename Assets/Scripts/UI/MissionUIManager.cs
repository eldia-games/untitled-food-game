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
        _missionController = GetComponent<MissionController>();
        missionTemp = _missionController.GetMissions();
        missionStatus = new bool[missionTemp.Count];
    }
    void Start()
    {
        //RefreshMissionUI();
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

    // Nuevo    
    [System.Serializable]
    public class UIObjectData
    {
        public string recipeName;
        public int moneyAmount; 
        public List<RecipeItem> recipes = new List<RecipeItem>();
    }
    
   [Header("UI References")]
   public GameObject itemUIPrefab;
   public Transform contentParent;
   
   [Header("Test Data")]
   public List<UIObjectData> objectsToDisplay = new List<UIObjectData>();
   
    public void fillUIObject(List<Mission> missionList, List<UIObjectData> objectsToDisplay)
    {
        foreach (Mission mis in missionList)
        {
            foreach (UIObjectData ui in objectsToDisplay)
            {
                ui.recipeName = mis.getTitle(); 
                ui.moneyAmount = mis.getPrice();
                ui.recipes = mis.getItems();
            }
        }
        Debug.Log("UI object llenado");
    }
    
        public void RefreshMissionUI()
        {
            missionTemp = _missionController.GetMissions();
            ClearExistingUI();
    
            foreach (Mission mis in missionTemp)
            {
                GameObject newItem = Instantiate(itemUIPrefab, contentParent);
                SetupItemUI(newItem, mis);
            }
        }
    
        void ClearExistingUI()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
    
        void SetupItemUI(GameObject uiElement, Mission mis)
        {
        // Configurar nombre
        TextMeshProUGUI nameText = uiElement.transform.Find("mission-backpanel/text-mask/tmp-mission-name").GetComponent<TextMeshProUGUI>();
        nameText.text = mis.getTitle();

        // Configurar dinero 
        TextMeshProUGUI moneyText = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        moneyText.text = mis.getPrice().ToString();

        // Configurar items (asumiendo que SetupItems también usa TextMeshPro)
        Transform itemsContainer = uiElement.transform.Find("mission-backpanel/layout-items");
        InventorySafeController inventory = InventorySafeController.Instance;
        SetupItems(mis.getItems(), itemsContainer, inventory);
    }
    
   void SetupItems(List<RecipeItem> items, Transform container, InventorySafeController inventory)
   {
       for (int i = 0; i < items.Count && i < 3; i++)
       {
            Transform itemSlot = container.GetChild(i);
            TextMeshProUGUI itemMissionAmount = itemSlot.Find("item-quantity-box/item-mission-quantity-text").GetComponent<TextMeshProUGUI>();
            //TextMeshProUGUI itemInventoryAmount = itemSlot.Find("item-quantity-box/item-inventory-quantity-text").GetComponent<TextMeshProUGUI>();
            //int quantityInventory = inventory.getQuantity(items[i].GetItem());
            
            itemMissionAmount.text = items[i].GetQuantity().ToString();
            //itemInventoryAmount.text = quantityInventory.ToString();

            itemSlot.gameObject.SetActive(true);
       }
       for(int i = 0; i < 3 - items.Count; i++)
       {
            Transform itemSlot = container.GetChild(2-i);
            itemSlot.gameObject.SetActive(false);
        }
   }
    
}
