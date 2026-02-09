using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        CombatManager.instance.actionQueue.Enqueue(() => EnemyTakeDamage(result.FinalDamage));
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
    }

    public IEnumerator EnemyAttack(Tile intentedTile)
    {
        EnemyAttackTileObject attackTile = Instantiate(tilePrefab, UIManager.instance.transform);
        attackTile.Initialize(intentedTile);
        attackTile.rt.position = (Vector2)Camera.main.WorldToScreenPoint(transform.position) + attackTileOffset;

        Player.ParryContext parryContext;
        List<Tile> expandedPlayerHand = PlayerHand.instance.currentHand
            .Select(obj => obj.tileData)
            .Concat(new[] { attackTile.tileData })
            .ToList();
        List<Tile> parryHand = MahjongHands.GetAllHandCombinations(expandedPlayerHand)
            .Where(hand => hand.Contains(intentedTile))
            .OrderByDescending(hand => MahjongHands.GetMahjongHand(hand))
            .FirstOrDefault().ToList();
        MahjongHandTypes parryHandType = MahjongHands.GetMahjongHand(parryHand);
        bool canParry = parryHandType != MahjongHandTypes.None
            && parryHandType != MahjongHandTypes.Pair
            && parryHandType != MahjongHandTypes.ThreePairs
            && parryHandType != MahjongHandTypes.AllPairs;
        if (canParry)
        {
            parryContext = new(this, parryHandType, parryHand);
            Player.instance.OpenParryWindow(parryContext);
        }
        else
        {
            parryContext = new(this, MahjongHandTypes.None, null);
        }

        float attackTime = canParry ? Player.instance.baseParryWindow : 2.0f;
        yield return new WaitForSeconds(attackTime);

        FlowerTileManager.instance.ActivateFlowerTilesOnIncomingDamage(-attackDamage);
        if (parryContext.wasParried)
        {
        }
        else
        {
            CombatManager.instance.actionQueue.Enqueue(() => Player.instance.PlayerTakeDamage(attackDamage));
            FlowerTileManager.instance.ActivateFlowerTilesOnTakeDamage(-attackDamage);
            Destroy(attackTile.gameObject);
        }
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
