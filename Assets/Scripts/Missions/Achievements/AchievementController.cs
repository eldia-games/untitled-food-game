using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    [SerializeField] private List<ScriptableAchievement> posibleAchievements;
    private InventorySafeController inventory;

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

        if (inventory == null)
        {
            inventory = InventorySafeController.Instance;

        }
        List<Achievement> actualAchievements = inventory.getAchievements();

        List<Achievement> achievements = new List<Achievement>();
        bool found = false;
        foreach (ScriptableAchievement ach in posibleAchievements)
        {
            found = true;
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
        inventory.setAchievements(achievements);

        return inventory.getAchievements();

    }

    public void stepAchievement(ScriptableAchievement ach)
    {
        List<Achievement> actualAchievements = inventory.getAchievements();
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
        inventory.saveInventory();
    }
    public void completeAchievement(Achievement ach)
    {
        List<Achievement> actualAchievements = inventory.getAchievements();
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
                        //AÑAdir dinero
                    }
                }
                break;
            }
        }
        inventory.saveInventory();
    }

}
