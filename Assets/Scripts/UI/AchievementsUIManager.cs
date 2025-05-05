using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUIManager : MonoBehaviour
{
    [Header("Achievement variables")]
    private AchievementController _achievementController;
    private List<Achievement> _achievementTemp;
    private bool[] achievementStatus;

    [Header("UI References")]
    public GameObject itemUIPrefab;
    public Transform contentParent;

    [Header("Unity Paths")]
    const string titleAchievementPath = "trophy-backpanel/text-mask/title-trophy";
    const string descriptionAchievementPath = "trophy-backpanel/text-mask/description-trophy";

    const string objectiveAchievementPath = "trophy-backpanel/trophy-icon-mask/trophy-progress-box/objective-text";
    const string progressAchievementPath = "trophy-backpanel/trophy-icon-mask/trophy-progress-box/player-progress-text";

    const string rewardAchievementPath = "reward-icon-mask/trophy-frame/money-text";
    const string iconAchievementPath = "trophy-backpanel/trophy-icon-mask/trophy-icon";

    const string buttonActionPath = "completion-button";
    const string buttonTextPath = "completion-button/trophy-button-text";

    void Awake()
    {
        //_achievementController = GetComponent<ShopController>();
        //tradesTemp = _achievementController.
    }

    private void ClearExistingUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshAchievementUI()
    {
        _achievementTemp = AchievementController.Instance.GetAchievements();
        achievementStatus = new bool[_achievementTemp.Count];
        ClearExistingUI();
        contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(1250, 250 * _achievementTemp.Count);
        for (int i = 0; i < _achievementTemp.Count; i++)
        {
            GameObject newPrefab = Instantiate(itemUIPrefab, contentParent);
            achievementStatus[i] = SetupAchievementUI(newPrefab, _achievementTemp[i], i);
        }
    }

    private bool SetupAchievementUI(GameObject uiElement, Achievement achievement, int achievementStep)
    {
        bool status = false;
        InventorySafeController inventory = InventorySafeController.Instance;

        if (uiElement == null)
        {
            Debug.LogError("SetupShopUI: uiElement is null");
        }

        if (achievement == null)
        {
            Debug.LogError("SetupShopUI: trade is null");
        }

        if (inventory == null)
        {
            Debug.LogError("SetupShopUI: inventory is null");
        }

        // Variables cantidades items
        int objectiveToAchieve = achievement.getNnumRepetitions();
        int objectiveProgress = achievement.getTimesDone();
        bool objectiveAlreadyCompleted = achievement.isCompleted();

        // Configurar strings
        TextMeshProUGUI titleAchievement = uiElement.transform.Find(titleAchievementPath).GetComponent<TextMeshProUGUI>();
        if (titleAchievement != null)
        {
            titleAchievement.text = achievement.getTitle();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemNamePath component");
        }

        TextMeshProUGUI descriptionAchievement = uiElement.transform.Find(descriptionAchievementPath).GetComponent<TextMeshProUGUI>();
        if (descriptionAchievement != null)
        {
            descriptionAchievement.text = achievement.getDescription();
        }
        else
        {
            Debug.LogWarning("descriptionAchievement: Could not find sellItemNamePath component");
        }

        // Configurar cantidad progreso 
        TextMeshProUGUI objectiveToAchieveTMP = uiElement.transform.Find(objectiveAchievementPath).GetComponent<TextMeshProUGUI>();
        if (objectiveToAchieveTMP != null)
        {
            objectiveToAchieveTMP.text = objectiveToAchieve.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemShopAmountPath component");
        }

        TextMeshProUGUI objectiveProgressTMP = uiElement.transform.Find(progressAchievementPath).GetComponent<TextMeshProUGUI>();
        if (objectiveProgressTMP != null)
        {
            objectiveProgressTMP.text = objectiveProgress.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buyItemShopAmountPath component");
        }

        // Configurar recompensa
        //TextMeshProUGUI objectiveRewardTMP = uiElement.transform.Find(rewardAchievementPath).GetComponent<TextMeshProUGUI>();
        //if (objectiveRewardTMP != null)
        //{
        //    objectiveRewardTMP.text = achievement.getPrice().ToString();
        //}
        //else
        //{
        //    Debug.LogWarning("SetupShopUI: Could not find buyItemShopAmountPath component");
        //}

        // Configurar los sprites
        RawImage achievementIconRawImage = uiElement.transform.Find(iconAchievementPath).GetComponent<RawImage>();
        if (achievementIconRawImage != null && achievement.getIcon() != null)
        {
            achievementIconRawImage.texture = achievement.getIcon();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemInventorySpritePath component or itemOut is null");
        }

        status = objectiveToAchieve - objectiveProgress <= 0;

        // Configurar boton estatus
        TextMeshProUGUI buttonText = uiElement.transform.Find(buttonTextPath).GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (objectiveAlreadyCompleted)
                buttonText.text = "Completed";
            else
                buttonText.text = status ? "Completed" : "Can't complete";
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buttonTextPath component");
        }

        //Configurar boton listener
        //Button button = uiElement.transform.Find(buttonActionPath).GetComponent<Button>();
        //if (button != null)
        //{
        //    button.onClick.AddListener(() =>
        //    {
        //        AchievementAction(achievementStep, _achievementController, objectiveAlreadyCompleted);
        //    });
        //}
        //else
        //{
        //    Debug.LogWarning("SetupShopUI: Could not find buttonActionPath component");
        //}

        return status;
    }

    public void AchievementAction(int achievementIndex, AchievementController _achievementController, bool isAlreadyDone)
    {
        if (!isAlreadyDone && achievementStatus[achievementIndex])
        {
            AudioManager.Instance.PlaySFXConfirmation();
            _achievementController.completeAchievement(_achievementTemp[achievementIndex]);
            RefreshAchievementUI();
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }
}
