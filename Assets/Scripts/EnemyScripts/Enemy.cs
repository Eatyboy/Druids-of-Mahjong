using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;

public class Enemy : MonoBehaviour, IDamageable
{
    public int currentHP = 100; // Default: 10
    public int maxHP = 100; // Default: 10
    public int qiOnDeath = 100;
    public int attackDamage = 1;

    [SerializeField] private EnemyTileObject tilePrefab;
    [SerializeField] private RectTransform tileSplineContainer;
    [SerializeField] private SplineContainer drawSpline;
    [SerializeField] private SplineContainer attackSpline;
    [SerializeField] private SplineContainer parrySpline;
    [SerializeField] private Vector2 attackTileOffset;
    [SerializeField] private float attackDuration = 2.0f;

    [SerializeField] private float deathAnimationDuration = 2.0f;

    public bool isTurnActive = false;

    [SerializeField] private SpriteRenderer spriteRenderer;
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

    public void SetHealth(int health)
    {
        currentHP = health;
        maxHP = health;
        healthBar.SetMaxHealth(maxHP);
        healthBar.SetHealth(currentHP);
    }

    public IEnumerator EnemyAttack(Tile intendedTile)
    {
        EnemyTileObject attackTile = Instantiate(tilePrefab, UIManager.instance.transform);
        attackTile.Initialize(intendedTile, tileSplineContainer.anchoredPosition, 
            drawSpline.Spline, attackSpline.Spline, parrySpline.Spline);

        yield return attackTile.PlayDrawAnimation();

        ParryHandler.ParryContext parryContext;
        var augmentedPlayerHand = PlayerHand.instance.GetPlayerHandTileData().Append(attackTile.tileData).ToList();
        var optimalHandTask = MahjongHands.GetOptimalHandAsync(augmentedPlayerHand, attackTile.tileData);
        yield return new WaitUntil(() => optimalHandTask.IsCompleted);
        if (optimalHandTask.IsFaulted)
        {
            Debug.LogError("Failed to get the optimal hand for parry");
            yield break;
        }
        var (type, tiles) = optimalHandTask.Result;
        bool canParry = type != MahjongHandTypes.None
            && type != MahjongHandTypes.Pair
            && type != MahjongHandTypes.ThreePairs
            && type != MahjongHandTypes.AllPairs;

        if (canParry)
        {
            List<PlayerTileObject> parryTileObjects = new();
            foreach (Tile tile in tiles)
            {
                var obj = PlayerHand.instance.currentHand.FirstOrDefault(t => t.tileData == tile);
                if (obj != null)
                    parryTileObjects.Add(obj);
            }
            parryContext = new(this, type, tiles, parryTileObjects, attackTile);
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
            yield return attackTile.PlayParriedAnimation();
        }
        else
        {
            yield return attackTile.PlayAttackAnimation();

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

    public IEnumerator EnemyDeath()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < deathAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / deathAnimationDuration;

            spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f - t);

            yield return null;
        }

        Player.instance.AddQi(qiOnDeath);
        CombatManager.instance.QiDropped(qiOnDeath);
        PopupSystem.instance.OpenPopup(EnemyManager.instance.qiDropPopupPreset, transform.position, qiOnDeath.ToString());

        yield break;
    }

    public IEnumerator EnemyTakeDamage(int damageToTake)
    {
        currentHP -= damageToTake;
        healthBar.SetHealth(currentHP);
        PopupSystem.instance.OpenPopup(EnemyManager.instance.enemyDamagePopupPreset, transform.position, damageToTake.ToString());

        if (currentHP <= 0) { 
            CombatManager.instance.EnqueueAction(() => EnemyDeath(), nameof(EnemyDeath));
        }

        yield break;
    }  
}
