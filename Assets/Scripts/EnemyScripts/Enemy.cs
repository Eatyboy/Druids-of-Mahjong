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

    public bool isTurnActive = false;


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
        CombatManager.instance.actionQueue.Enqueue(new EnemyTakeDamage(this, result.FinalDamage));
    }

    protected void Start()
    {
        Init();
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

    public class EnemyAttack : ICombatAction
    {
        private readonly int damage;

        public EnemyAttack(int damage)
        {
            this.damage = damage;
        }

        public IEnumerator Execute()
        {
            yield return new WaitForSeconds(0.4f);

            CombatManager.instance.actionQueue.Enqueue(new Player.PlayerTakeDamage(-damage));

            yield return new WaitForSeconds(0.4f);
        }
    }

    public virtual void MakeAttackDecision()
    {
        CombatManager.instance.actionQueue.Enqueue(new EnemyAttack(attackDamage));
    }

    protected virtual void OnDeath()
    {
        Player.instance.AddQi(qiOnDeath);
    }

    public class EnemyTakeDamage : ICombatAction
    {
        private readonly Enemy enemy;
        private readonly int damageToTake;

        public EnemyTakeDamage(Enemy enemy, int damageToTake)
        {
            this.enemy = enemy;
            this.damageToTake = damageToTake;
        }

        public IEnumerator Execute()
        {
            enemy.currentHP -= damageToTake;
            enemy.healthBar.SetHealth(enemy.currentHP);

            if (enemy.currentHP <= 0) { 
                enemy.OnDeath(); 
            }

            yield break;
        }  
    }
}
