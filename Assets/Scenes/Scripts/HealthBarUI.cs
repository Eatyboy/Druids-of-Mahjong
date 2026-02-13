using UnityEngine;
using UnityEngine.UI;


public class HealthBarUI : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public Slider slider;

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;
        slider.value = currentHealth / maxHealth;
    }
}
