using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class HealthManaUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider healthSlider;
    public Slider manaSlider;
    public Gradient healthGradient;
    public Gradient manaGradient;
    public Image healthFill;
    public Image manaFill;

    static int MANA_BASE_REGEN = 10;

    public void SetMaxHealth(float health) {
        healthSlider.maxValue = health;
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(1.0f);

    }

    public void setHealth(float health) { 
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    public void SetMaxMana(float mana)
    {
        manaSlider.maxValue = mana;
        manaSlider.value = mana;

        manaFill.color = manaGradient.Evaluate(1.0f);

    }

    public void setMana(float mana)
    {
        manaSlider.value = mana;

        manaFill.color = manaGradient.Evaluate(manaSlider.normalizedValue);
    }

    public void regenMana(float manaRegenRate)
    {
        if (manaSlider.value < manaSlider.maxValue)
        {
           increaseMana(manaRegenRate*MANA_BASE_REGEN);

            manaFill.color = manaGradient.Evaluate(1.0f);
        }
    }

    public void increaseMana(float manaIncrement) {
        if (manaSlider.value < manaSlider.maxValue) {
            if (manaSlider.value + manaIncrement < manaSlider.maxValue)
            {
                manaSlider.value += manaIncrement;
            }
            else {
                setMana(manaSlider.maxValue);
            }

        }
    }

    public void increaseHealth(float healthIncrement)
    {
        if (healthSlider.value < healthSlider.maxValue)
        {
            if (healthSlider.value + healthIncrement < healthSlider.maxValue)
            {
                healthSlider.value += healthIncrement;
            }
            else
            {
                setMana(healthSlider.maxValue);
            }

        }
    }

}
