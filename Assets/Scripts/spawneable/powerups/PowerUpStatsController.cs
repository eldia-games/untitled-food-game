using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum powerUpType
{
    None,
    Health,
    Damage,
    Movement,
    //TODO
}

public class PowerUpStatsController : MonoBehaviour
{
    [SerializeField] private PowerUpStats stats;
    // Start is called before the first frame update
    public static PowerUpStatsController Instance { get; private set; }

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


    public void PowerUp(powerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case powerUpType.None:
                break;
            case powerUpType.Health:
                PlayerStatsController.Instance.augmentMaxHealhtprct(stats.maxLife);
                break;
            case powerUpType.Damage:
                PlayerStatsController.Instance.augmentMaxDamageprct(stats.damage);
                break;
            case powerUpType.Movement:
                PlayerStatsController.Instance.augmentMaxmoveSpeedprct(stats.MovementSpeed);
                break;

        }
    }

}
