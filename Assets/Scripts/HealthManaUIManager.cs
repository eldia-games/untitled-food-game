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

    public void setMaxHealth(float health) {
        healthSlider.maxValue = health;
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(1.0f);

    }

    public void setHealth(float health) { 
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    public void setMaxMana(float mana)
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
           gainMana(manaRegenRate*MANA_BASE_REGEN);
        }
    }

    public void gainMana(float manaIncrement) {
        if (manaSlider.value < manaSlider.maxValue) {
            if (manaSlider.value + manaIncrement < manaSlider.maxValue)
            {
                setMana(manaSlider.value + manaIncrement);
            }
            else {
                setMana(manaSlider.maxValue);
            }

        }
    }

    public void gainHealth(float healthIncrement)
    {
        if (healthSlider.value < healthSlider.maxValue)
        {
            if (healthSlider.value + healthIncrement < healthSlider.maxValue)
            {
                setHealth(healthSlider.value + healthIncrement);
            }
            else
            {
                setHealth(healthSlider.maxValue);
            }

        }
    }

    public void loseMana(float manaDecrease)
    {
        if (manaSlider.value - manaDecrease > 0)
        {
            setMana(manaSlider.value - manaDecrease);
        }
        else
        {
            setMana(0);
            //llamar a mensaje de mana insuficiente
        }


    }

    public void loseHealth(float healthDecrease)
    {
        if (healthSlider.value - healthDecrease > 0)
        {
            setHealth(healthSlider.value - healthDecrease);
        }
        else
        {
            setHealth(0);
            //llamar a muerte de personaje y pantalla de game over
        }

        
    }

}
