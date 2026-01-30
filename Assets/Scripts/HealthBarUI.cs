using UnityEngine;


public class HealthBarUI : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float Width;
    public float Height;

    [SerializeField]
    private RectTransform healthBar;

    void Start()
    {
        Width = healthBar.sizeDelta.x;
        Height = healthBar.sizeDelta.y;
        SetMaxHealth(MaxHealth);
        Health = MaxHealth;
    }


    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }

    public void SetHealth(float currentHealth)
    {
        Health = currentHealth;

        float newWidth = (Health / MaxHealth) * Width;
        healthBar.sizeDelta = new Vector2(newWidth, Height);
    }
}
