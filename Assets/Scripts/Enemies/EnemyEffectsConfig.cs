using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEffectsConfig", menuName = "Config/Enemy Effects")]
public class EnemyEffectsConfig : ScriptableObject
{
    [Header("Flash Settings")]
    [Tooltip("Duration of the white fresnel flash (seconds)")]
    public float flashDuration = 0.2f;
    [Tooltip("Maximum intensity of the flash (multiplier on fresnel)")]
    public float flashMaxPower  = 1f;

    [Header("Shrink Settings")]
    [Range(0.1f, 1f)]
    [Tooltip("Factor to scale the enemy when damaged (1 = no change)")]
    public float shrinkFactor   = 0.9f;
    [Tooltip("Total time to shrink and return (seconds)")]
    public float shrinkDuration = 0.1f;
}

