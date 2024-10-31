using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;  // Reference to the Slider component

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;  // Set the max value of the slider
        slider.value = health;      // Set the current value to max at start
    }

    public void SetHealth(int health)
    {
        slider.value = health;      // Update the slider value to reflect current health
    }
}