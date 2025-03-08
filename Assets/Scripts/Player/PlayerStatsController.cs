using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    // Start is called before the first frame update
    public static PlayerStatsController Instance { get; private set; }

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

    public void heal(float heal)
    {
        Debug.Log(stats.HP);
        stats.HP = Mathf.Min(stats.HP + heal, stats.maxLife);
        Debug.Log(stats.HP);
    }
    public void healprct(float heal)
    {
        stats.HP = Mathf.Min(stats.HP + stats.maxLife *heal, stats.maxLife);
    }

    public void augmentMaxHealht(int heal)
    {
        stats.maxLife += heal;
    }
    public void augmentMaxHealhtprct(float heal)
    {
        stats.maxLife = (int)Mathf.Round(stats.maxLife*(1 + heal));
    }
    public void augmentMaxDamage(int damage)
    {
        stats.damage += damage;
    }
    public void augmentMaxDamageprct(float damage)
    {
        stats.damage = (int)Mathf.Round(stats.damage * (1 + damage));
    }
    public void augmentMaxmoveSpeed(float speed)
    {
        stats.MovementSpeed += speed;
    }
    public void augmentMaxmoveSpeedprct(float speed)
    {
        stats.MovementSpeed = (int)Mathf.Round(stats.MovementSpeed * (1 + speed));
    }
}
