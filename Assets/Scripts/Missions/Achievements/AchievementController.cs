using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    [SerializeField] private List<ScriptableAchievement> posibleAchievements;

    public static AchievementController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public List<Achievement> GetAchievements()
    {
        List<Achievement> actualAchievements = InventorySafeController.Instance.getAchievements();

        List<Achievement> achievements = new List<Achievement>();
        bool found = false;
        foreach (ScriptableAchievement ach in posibleAchievements)
        {
            found = false;
            for (int i = 0; i < actualAchievements.Count; i++)
            {

                
                if(ach== actualAchievements[i].getScriptableAchievement())
                {
                    achievements.Add(actualAchievements[i]);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                achievements.Add(new Achievement(ach,0,false));
            }

        }
        InventorySafeController.Instance.setAchievements(achievements);

        return InventorySafeController.Instance.getAchievements();

    }

    public void stepAchievement(ScriptableAchievement ach)
    {
        List<Achievement> actualAchievements = InventorySafeController.Instance.getAchievements();
        for (int i = 0; i < actualAchievements.Count; i++)
        {
            if (actualAchievements[i].getScriptableAchievement() == ach)
            {
                Achievement achi= actualAchievements[i];
                if (!achi.isCompleted())
                {
                    int maxStep = achi.getNnumRepetitions();
                    int actualStep = achi.getTimesDone();
                    if (actualStep < maxStep)
                    {
                        achi.addStep();
                    }
                    
                    
                }
                break;
            }
        }
        InventorySafeController.Instance.saveInventory();
    }
    public void completeAchievement(Achievement ach)
    {
        List<Achievement> actualAchievements = InventorySafeController.Instance.getAchievements();
        for (int i = 0; i < actualAchievements.Count; i++)
        {
            if (actualAchievements[i].getScriptableAchievement() == ach.getScriptableAchievement())
            {
                Achievement achi = actualAchievements[i];
                if (!achi.isCompleted())
                {
                    int maxStep = achi.getNnumRepetitions();
                    int actualStep = achi.getTimesDone();
                    if (actualStep >= maxStep)
                    {
                        achi.complete();
                        //Añadir dinero
                    }
                }
                break;
            }
        }
        InventorySafeController.Instance.saveInventory();
    }

}
