using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persistence : MonoBehaviour
{
    public static persistence Instance { get; private set; }
    [SerializeField] private RoomType chamberType;
    [SerializeField] private int chamberLevel;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //chamberType = 0;
            //chamberLevel = 2;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    public void setType(RoomType type)
    {
        chamberType=type;
    }
    public RoomType getType()
    {
        return chamberType;
    }
    public void setLevel(int level)
    {
        chamberLevel = level;
    }
    public int getLevel()
    {
        return chamberLevel;
    }
}
