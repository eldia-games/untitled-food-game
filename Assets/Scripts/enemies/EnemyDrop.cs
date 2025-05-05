using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyDrop
{
    [SerializeField] public  GameObject drop;

    [SerializeField][Range(0,1)] public float chanceDrop;
    [SerializeField] public int minDrop;
    [SerializeField] public int maxDrop;
}
