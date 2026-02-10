using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int currentHP = 100; // Default: 10
    public int maxHP = 100; // Default: 10
    public int qiOnDeath = 100;
    public int attackDamage = 1;

    [SerializeField] private EnemyAttackTileObject tilePrefab;
    [SerializeField] private Vector2 attackTileOffset;

    public bool isTurnActive = false;

    [SerializeField] private HealthBarUI healthBar;

    protected void Start()
    {
        Init();
    }

    protected void Init()
    {
        // Setup any variables and object assignments here
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);
        healthBar.SetHealth(currentHP);
    }

    public IEnumerator EnemyAttack(Tile intentedTile)
    {
        EnemyAttackTileObject attackTile = Instantiate(tilePrefab, UIManager.instance.transform);
        attackTile.Initialize(intentedTile);
        attackTile.rt.position = (Vector2)Camera.main.WorldToScreenPoint(transform.position) + attackTileOffset;

        yield return new WaitForSeconds(2.0f);

        FlowerTileManager.instance.ActivateFlowerTilesOnIncomingDamage(-attackDamage);
        CombatManager.instance.actionQueue.Enqueue(() => Player.instance.PlayerTakeDamage(-attackDamage));
        FlowerTileManager.instance.ActivateFlowerTilesOnTakeDamage(-attackDamage);

        Destroy(attackTile.gameObject);
    }

    public virtual void MakeAttackDecision()
    {
        Tile intentedTile = TilesManager.instance.GenerateRandomTile();
        CombatManager.instance.actionQueue.Enqueue(() => EnemyAttack(intentedTile));
    }

    protected virtual void OnDeath()
    {
        Player.instance.AddQi(qiOnDeath);
    }

    public IEnumerator EnemyTakeDamage(int damageToTake)
    {
        currentHP -= damageToTake;
        healthBar.SetHealth(currentHP);

        if (currentHP <= 0) { 
            OnDeath(); 
        }

        yield break;
    }  
}
