using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChamberUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentBeersTMP;
    [SerializeField] private TMP_Text maxBeersTMP;

    #region PlayerBeers

    public void setPlayerBeers()
    {
        setBeers(InventoryList.Instance.getBeers());
        setMaxBeers(InventoryList.Instance.getMaxBeers());
    }
    public void resetPlayerBeersToDefault(int maxBeers)
    {
        setBeers(maxBeers);
        setMaxBeers(maxBeers);
    }

    private void setBeers(int beers)
    {
        currentBeersTMP.text = beers.ToString();
    }

    private void setMaxBeers(int maxBeers)
    {
        maxBeersTMP.text = maxBeers.ToString();
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

    public void ShowBossHealth(float health, float maxHealth)
    {
        ResetBossHealth(health, maxHealth);
        bossHealth.SetActive(true);

    }
    public void ResetBossHealth(float health, float maxHealth)
    {
        //llamar al boss y pedir datos
        SetMaxHealth(health);
        SetHealth(maxHealth);
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
