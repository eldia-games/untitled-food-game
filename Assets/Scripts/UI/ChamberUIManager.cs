using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChamberUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentBeers;
    [SerializeField] private TMP_Text maxBeers;

    #region PlayerBeers
    public void setPlayerBeers(int beers, int maxBeers)
    {
        setBeers(beers);
        setMaxBeers(maxBeers);
    }
    public void resetPlayerBeersToDefault(int maxBeers)
    {
        setBeers(maxBeers);
        setMaxBeers(maxBeers);
    }

    private void setBeers(int beers)
    {
        currentBeers.text = beers.ToString();
    }

    private void setMaxBeers(int maxBeers)
    {
        currentBeers.text = maxBeers.ToString();
    }
    #endregion

    #region BossHealth
    [SerializeField] private GameObject bossHealth;
    public Slider healthSlider;
    public Gradient healthGradient;
    public Image healthFill;

    public TMP_Text maxhealthTMP, currentHealthTMP;

    public void HideBossHealth()
    {
        bossHealth.SetActive(false);
    }

    public void ShowBossHealth()
    {
        bossHealth.SetActive(true);
        ResetBossHealth();
    }
    public void ResetBossHealth()
    {
        //llamar al boss y pedir datos
        //SetMaxHealth(getHealthBoss());
        //SetHealth(getMaxHealthBoss());
    }

    public void SetMaxHealth(float health)
    {
        maxhealthTMP.text = health.ToString();
        healthSlider.maxValue = health;
        healthFill.color = healthGradient.Evaluate(1.0f);
    }

    public void SetHealth(float health)
    {
        currentHealthTMP.text = health.ToString();
        healthSlider.value = health;
        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    #endregion
}
