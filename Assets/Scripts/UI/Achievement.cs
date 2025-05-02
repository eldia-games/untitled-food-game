using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement
{
    #region Variables
    [SerializeField] private int id;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private bool completed;
    #endregion

    #region Missions
    public Achievement(string title, string description, bool completed)
    {
        this.title = title;
        this.description = description;
        this.completed = completed;
    }

    public bool getCompleted()
    {
        return completed;
    }

    public string getTitle()
    {
        return title;
    }
    public string getDescription()
    {
        return description;
    }

    #endregion

}
