using UnityEngine;

[Tooltip("Abstract Class that Enemies should inherit from")]
// Only Common Functionality and can be Overrided
// Create New Enemy Scripts inheriting from Enemy
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int currentHP = 100; // Default: 10
    [SerializeField]
    protected int maxHP = 100; // Default: 10

    [SerializeField]
    protected Transform player; // Reference to deal damage to player
    [SerializeField] private HealthBarUI healthBar;

    protected void OnEnable()
    {
        HandAttackResolver.OnAttackResolved += HandleAttackResolved;
    }

    protected void OnDisable()
    {
        HandAttackResolver.OnAttackResolved -= HandleAttackResolved;
    }

    void HandleAttackResolved(AttackResult result)
    {
        TakeDamage(result.FinalDamage);
    }

    protected void Start()
    {
        Init();
        Debug.Log("Initializing Enemy");
    }
    protected virtual void Update()
    {

    }

    protected void Init()
    {
        // Setup any variables and object assignments here
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        healthBar.SetHealth(currentHP);
        if (player == null) { /*Debug.Log("Missing Player Reference");*/ }
    }

    protected virtual void Attack()
    {
        //Debug.Log("Enemy Attacks!");
    }

    protected virtual void OnDeath()
    {
        //Debug.Log("Enemy Died!");
    }

    // IDamageable
    public virtual bool TakeDamage(int dmg) 
    {
        currentHP -= dmg;
        healthBar.SetHealth(currentHP);

        if (currentHP < 0) { 
            OnDeath(); 
            return true; 
        }
        return false;
    }
}
