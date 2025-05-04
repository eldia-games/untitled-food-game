using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement : MonoBehaviour
{
    #region Variables

    [SerializeField] ScriptableAchievement scriptableAch;
    [SerializeField] int timesDone;
    [SerializeField] bool completed;

    #endregion

    #region Achievements

    public Achievement(ScriptableAchievement ach, int timesDone, bool completed)
    {
        this.scriptableAch = ach;
        this.timesDone = timesDone;
        this.completed = completed;
    }

    public Texture2D getIcon()
    {
        return scriptableAch.icon;
    }

    public string getDescription()
    {
        return scriptableAch.description;
    }

    public string getTitle()
    {
        return scriptableAch.title;
    }
    public int getNnumRepetitions()
    {
        return scriptableAch.numRepetitions; ;
    }
    public int getPrice()
    {
        return scriptableAch.price;
    }

    public int getTimesDone()
    {
        return timesDone;
    }
    public bool isCompleted()
    {
        return completed;
    }
    public ScriptableAchievement getScriptableAchievement()
    {
        return scriptableAch;
    }
    public void addStep()
    {
        timesDone++;
    }

    public void complete()
    {
        completed = true;
    }



    #endregion


}
