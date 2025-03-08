using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawneable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<int> SpawnRate;
    [SerializeField] private int value;

    public int getSpawnRate(int level)
    {
        if(SpawnRate.Count > level)
        {
            return SpawnRate[level];
        }
        return SpawnRate[SpawnRate.Count-1];
    }
    public int getValue()
    {
        return value;
    }

}
