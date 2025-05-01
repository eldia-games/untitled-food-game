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
        fillUIObject(missionTemp, objectsToDisplay);
        RefreshMissionUI();
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
   
   void Start()
   {

   }
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
            ClearExistingUI();
    
            foreach (UIObjectData data in objectsToDisplay)
            {
                GameObject newItem = Instantiate(itemUIPrefab, contentParent);
                SetupItemUI(newItem, data);
            }
        }
    
        void ClearExistingUI()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
    
        void SetupItemUI(GameObject uiElement, UIObjectData data)
        {
            // Configurar nombre
            Text nameText = uiElement.transform.Find("tmp-mission-name").GetComponent<Text>();
            nameText.text = data.recipeName;
    
            // Configurar dinero (nuevo campo)
            Text moneyText = uiElement.transform.Find("money-quantity-text").GetComponent<Text>();
            moneyText.text = $"Dinero: {data.moneyAmount}";
    
            // Configurar items
            Transform itemsContainer = uiElement.transform.Find("layout-items");
            SetupItems(data.recipes, itemsContainer);
        }
    
        void SetupItems(List<RecipeItem> items, Transform container)
        {
            for (int i = 0; i < items.Count && i < 3; i++)
            {
                if (i < container.childCount)
                {
                    Transform itemSlot = container.GetChild(i);
                    Text itemText = itemSlot.GetComponentInChildren<Text>();
                //itemText.text = $"{items[i].itemName} x{items[i].quantity
                    itemText.text = $"{items[i].GetItem().name} x{items[i].GetQuantity()}";
                itemSlot.gameObject.SetActive(true);
                }
            }
    
            // Desactivar slots vacíos
            for (int i = items.Count; i < 3; i++)
            {
                if (i < container.childCount)
                {
                    container.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    
}
