using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Achievementd", menuName = "Missions/Achievement")]
public class ScriptableAchievement : ScriptableObject
{

    public Texture2D icon;
    public string description;
    public string title;
    public int numRepetitions;
    public int price;
    

}
