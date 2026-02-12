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
    [SerializeField] private float attackDuration = 2.0f;

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

    public IEnumerator EnemyAttack(Tile intendedTile)
    {
        EnemyAttackTileObject attackTile = Instantiate(tilePrefab, UIManager.instance.transform);
        attackTile.Initialize(intendedTile, (Vector2)Camera.main.WorldToScreenPoint(transform.position), attackTileOffset);
        yield return StartCoroutine(attackTile.PlayDrawAnimation());
        // attackTile.rt.position = (Vector2)Camera.main.WorldToScreenPoint(transform.position) + attackTileOffset;

        ParryHandler.ParryContext parryContext;
        List<Tile> expandedPlayerHand = PlayerHand.instance.currentHand
            .Select(obj => obj.tileData)
            .Concat(new[] { attackTile.tileData })
            .ToList();
        (MahjongHandTypes type, List<Tile> tiles) parryHand = MahjongHands
            .GetAllHandCombinations(expandedPlayerHand)
            .Where(hand => hand.tiles.Contains(intendedTile))
            .OrderByDescending(hand => hand.type)
            .FirstOrDefault();
        bool canParry = parryHand.type != MahjongHandTypes.None
            && parryHand.type != MahjongHandTypes.Pair
            && parryHand.type != MahjongHandTypes.ThreePairs
            && parryHand.type != MahjongHandTypes.AllPairs;

        if (canParry)
        {
            List<TileObject> parryTileObjects = new();
            foreach (Tile tile in parryHand.tiles)
            {
                var obj = PlayerHand.instance.currentHand.FirstOrDefault(t => t.tileData == tile);
                if (obj != null)
                    parryTileObjects.Add(obj);
            }
            parryContext = new(this, parryHand.type, parryHand.tiles, parryTileObjects, attackTile);
            Player.instance.parryHandler.OpenParryWindow(parryContext);
        }
        else
        {
            parryContext = new(this, MahjongHandTypes.None, null, null, attackTile);
            parryContext.Resolve(false);
        }

        yield return (canParry)
            ? new WaitUntil(() => parryContext.resolved)
            : new WaitForSeconds(attackDuration);

        if (parryContext.wasParried)
        {
            yield return StartCoroutine(attackTile.PlayParriedAnimation());
        }
        else
        {
            yield return StartCoroutine(attackTile.PlayAttackAnimation());
            CombatManager.instance.EnqueueAction(() => Player.instance.PlayerTakeDamage(attackDamage), nameof(Player.instance.PlayerTakeDamage));
            FlowerTileManager.instance.ActivateFlowerTilesOnTakeDamage(-attackDamage);
        }

        Destroy(attackTile.gameObject);
    }

    public virtual void MakeAttackDecision()
    {
        Tile intendedTile = TilesManager.instance.GenerateRandomTile();
        CombatManager.instance.EnqueueAction(() => EnemyAttack(intendedTile), nameof(EnemyAttack));
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
