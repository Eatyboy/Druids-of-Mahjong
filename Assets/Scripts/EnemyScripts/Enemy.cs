using System.Collections;
using UnityEngine;

[Tooltip("Abstract Class that Enemies should inherit from")]
// Only Common Functionality and can be Overrided
// Create New Enemy Scripts inheriting from Enemy
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public int currentHP = 100; // Default: 10
    public int maxHP = 100; // Default: 10
    public int qiOnDeath = 100;
    public int attackDamage = 1;


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

    public virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.4f);

        //Debug.Log("Enemy Attacks!");
        Player.instance.ChangeHealth(-attackDamage);

        yield return new WaitForSeconds(0.4f);

        yield return GameManager.instance.EndEnemyTurn();
    }

    protected virtual void OnDeath()
    {
        Player.instance.AddQi(qiOnDeath);
    }

    // IDamageable
    public virtual void TakeDamage(int dmg) 
    {
        currentHP -= dmg;
        healthBar.SetHealth(currentHP);

        if (currentHP <= 0) { 
            OnDeath(); 
        }
    }
}
