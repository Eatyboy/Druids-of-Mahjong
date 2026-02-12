using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public static Player instance;

    public float health, maxHealth;
    public int qi = 0;

    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private QiCounter qiCounter;

    [SerializeField] private float baseMaxHealth = 10.0f;

    [Header("Combat")]
    [SerializeField] private int baseDamage = 5; //temporary dmg number
    [SerializeField] private int flowerFlatDamageBonus = 10; //temporary dmg bonus

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        qi = 0;
        maxHealth = baseMaxHealth;
        health = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
        qiCounter.SetQi(qi);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current.qKey.wasPressedThisFrame)
        //{
        //    ChangeHealth(-1.0f);
        //}

        //if (Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    ChangeHealth(1.0f);
        //}
    }

    public void ChangeHealth(float healthChange) {
        health += healthChange;
        health = Mathf.Clamp(health, 0, maxHealth);

        healthBar.SetHealth(health);
    }

    public void AddQi(int qiChange)
    {
        qi += qiChange;
        qiCounter.SetQi(qi);
    }

    public int GetFinalDamage()
{
    int damage = baseDamage;

    if (FlowerTileManager.instance != null &&
        FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.FlatDamageBuff))
    {
        damage += flowerFlatDamageBonus;
    }

        return damage;
    }
}
