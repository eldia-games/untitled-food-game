using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public GameObject drop;

    [Range(0,1)] public float chanceDrop;
    public int minDrop;
    public int maxDrop;
}
