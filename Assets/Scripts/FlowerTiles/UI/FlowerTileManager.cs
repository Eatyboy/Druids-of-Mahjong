using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowerTileManager : MonoBehaviour
{
    public static FlowerTileManager instance;

    [SerializeField] private List<FlowerTileData> flowerTileDataObjects;
    private Dictionary<FlowerTileType, FlowerTileData> flowerTileMap = new();

    private List<FlowerTileInstance> playerFlowerTiles => GameManager.playerData.flowerTiles;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < flowerTileDataObjects.Count; i++)
        {
            var flowerTile = flowerTileDataObjects[i];

            if (flowerTile == null)
            {
                Debug.LogError($"Flower tile manager has a null flower tile data at index {i}");
            }
            else if (flowerTile.flowerTile == FlowerTileType.None)
            {
                Debug.LogWarning($"{flowerTile.name} is a None flower tile");
            }
            else if (flowerTile.effectConfig == null)
            {
                Debug.LogWarning($"{flowerTile.name} is a None flower tile");
            }
            else
            {
                flowerTileMap.Add(flowerTile.flowerTile, flowerTile);
            }
        }
    }

    public bool IsFlowerTileActive(FlowerTileType flowerTileType)
    {
        return playerFlowerTiles.Any((flowerTile) => flowerTile.data.flowerTile == flowerTileType);
    }

    public void ActivateFlowerTilesOnPreAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            Copier copier = ft.effect as Copier;

            if (copier != null)
            {
                copier.ClearCopiedEffect();

                break;
            }
        }

        for (int i = 0; i < playerFlowerTiles.Count; i++)
        {
            FlowerTileInstance ft = playerFlowerTiles[i];
            if (ft.effect == null) continue;

            Copier copier = ft.data.flowerTile == FlowerTileType.Copier ? ft.effect as Copier : null;
            
            if (copier != null && i > 0 && playerFlowerTiles[i - 1].effect != null)
            {
                FlowerTileInstance prev = playerFlowerTiles[i - 1];
                
                FlowerTileEffect copiedEffect = prev.data.effectConfig.CreateInstance();

                copier.AddCopiedEffect(copiedEffect);
            }

            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnPreAttack(attackContext),
                nameof(ft.effect.OnPreAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            if (ft.effect == null) continue;
            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnIntraAttack(attackContext),
                nameof(ft.effect.OnIntraAttack)
            );
        }
    }
    public void ActivateFlowerTilesOnPostAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            if (ft.effect == null) continue;
            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnPostAttack(attackContext),
                nameof(ft.effect.OnPostAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnIncomingDamage(int dmg)
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            if (ft.effect == null) continue;
            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnIncomingAttack(dmg),
                nameof(ft.effect.OnIncomingAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnTakeDamage(int dmg)
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            if (ft.effect == null) continue;
            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnTakeDamage(dmg),
                nameof(ft.effect.OnTakeDamage)
            );
        }
    }

    public void ActivateFlowerTilesOnTurnStart()
    {
        foreach (FlowerTileInstance ft in playerFlowerTiles)
        {
            if (ft.effect == null) continue;
            CombatManager.instance.EnqueueAction(
                () => ft.effect.OnTurnStart(),
                nameof(ft.effect.OnTurnStart)
            );
        }
    }

    public FlowerTileInstance GetRandomFlowerTile()
    {
        FlowerTileData data = Utils.GetRandomItemInList(flowerTileDataObjects);
        return new FlowerTileInstance(data);
    }
}
