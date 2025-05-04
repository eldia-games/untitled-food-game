using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManaUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider healthSlider, manaSlider;
    public Gradient healthGradient, manaGradient;
    public Image healthFill, manaFill;
    public TMP_Text maxhealthTMP, currentHealthTMP, maxManaTMP, currentManaTMP;



    public void ResetPlayer()
    {
        SetMaxHealth(PlayerStatsController.Instance.getMaxHealth());
        SetMaxMana(PlayerStatsController.Instance.getMaxMana());
        SetHealth(PlayerStatsController.Instance.getHealth());
        SetMana(PlayerStatsController.Instance.getMana());
    }

    public void SetMaxHealth(float health) {
        maxhealthTMP.text = health.ToString();
        healthSlider.maxValue = health;
        healthFill.color = healthGradient.Evaluate(1.0f);
    }

    public void SetHealth(float health) {
        currentHealthTMP.text = health.ToString();
        healthSlider.value = health;
        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    public void SetMaxMana(float mana)
    {
        maxManaTMP.text = mana.ToString();
        manaSlider.maxValue = mana;
        manaFill.color = manaGradient.Evaluate(1.0f);

    }

    public void SetMana(float mana)
    {
        currentManaTMP.text = Mathf.RoundToInt(mana).ToString();
        manaSlider.value = mana;
        manaFill.color = manaGradient.Evaluate(manaSlider.normalizedValue);
    }

    public void RegenMana(float manaRegenRate)
    {
        if (manaSlider.value < manaSlider.maxValue)
        {
           GainMana(manaRegenRate);
        }
    }

    public void GainMana(float manaIncrement) {
        if (manaSlider.value < manaSlider.maxValue) {
            if (manaSlider.value + manaIncrement < manaSlider.maxValue)
            {
                SetMana(manaSlider.value + manaIncrement);
            }
            else {
                SetMana(manaSlider.maxValue);
            }

        }
    }

    public void GainHealth(float healthIncrement)
    {
        if (healthSlider.value < healthSlider.maxValue)
        {
            if (healthSlider.value + healthIncrement < healthSlider.maxValue)
            {
                SetHealth(healthSlider.value + healthIncrement);
            }
            else
            {
                SetHealth(healthSlider.maxValue);
            }

        }
    }

    public void LoseMana(float manaDecrease)
    {
        if (manaSlider.value - manaDecrease > 0)
        {
            SetMana(manaSlider.value - manaDecrease);
        }
        else
        {
            SetMana(0);
            //llamar a mensaje de mana insuficiente
        }


    }

    public void LoseHealth(float healthDecrease)
    {
        if (healthSlider.value - healthDecrease > 0)
        {
            SetHealth(healthSlider.value - healthDecrease);
        }
        else
        {
            SetHealth(0);
            //llamar a muerte de personaje y pantalla de game over
        }

        
    }

}
