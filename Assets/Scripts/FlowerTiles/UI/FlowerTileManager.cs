using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerTileManager : MonoBehaviour
{
    public static FlowerTileManager instance;

    [Header("References")]
    [SerializeField] private RectTransform flowerTileContainer;
    [SerializeField] private FlowerTileInfoController infoController;
    [SerializeField] private List<FlowerTile> flowerTilePrefabs;
    private Dictionary<FlowerTileType, FlowerTile> flowerTileMap = new();

    [Header("Data")]
    public List<FlowerTile> playerFlowerTiles = new();

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        foreach (FlowerTile flowerTile in flowerTilePrefabs)
        {
            if (flowerTile == null)
            {
                Debug.LogError("Flower tile manager has a null flower tile");
            }
            else if (flowerTile.data == null) 
            {
                Debug.LogError($"{flowerTile.name} has null data");
            }
            else if (flowerTile.data.flowerTile == FlowerTileType.None)
            {
                Debug.LogWarning($"{flowerTile.name} is a None flower tile");
            }
            else
            {
                flowerTileMap.Add(flowerTile.data.flowerTile, flowerTile);
            }
        }

        foreach (FlowerTileType ft in GameManager.playerData.flowerTiles)
        {
            if (flowerTileMap.TryGetValue(ft, out FlowerTile flowerTile))
            {
                playerFlowerTiles.Add(flowerTile);
            }
            else
            {
                Debug.LogError($"Failed to add {ft} to player's flower tiles");
            }
        }
    }

    public void AddFlowerTile(FlowerTile flowerTilePrefab)
    {
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, flowerTileContainer);
        addedFlowerTile.Initialize(infoController);
        playerFlowerTiles.Add(addedFlowerTile);
    }

    public bool IsFlowerTileActive(FlowerTileType flowerTileType)
    {
        return playerFlowerTiles.Any((flowerTile) => flowerTile.data.flowerTile == flowerTileType);
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            AddFlowerTile(Utils.GetRandomItemInList(flowerTilePrefabs));
        }
    }

    public void ActivateFlowerTilesOnPreAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnPreAttack(attackContext),
                nameof(ft.effectClass.OnPreAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnIntraAttack(attackContext),
                nameof(ft.effectClass.OnIntraAttack)
            );
        }
    }
    public void ActivateFlowerTilesOnPostAttack(Player.PlayerAttackContext attackContext)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnPostAttack(attackContext),
                nameof(ft.effectClass.OnPostAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnIncomingDamage(int dmg)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnIncomingAttack(dmg),
                nameof(ft.effectClass.OnIncomingAttack)
            );
        }
    }

    public void ActivateFlowerTilesOnTakeDamage(int dmg)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnTakeDamage(dmg),
                nameof(ft.effectClass.OnTakeDamage)
            );
        }
    }

    public void ActivateFlowerTilesOnTurnStart()
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.EnqueueAction(
                () => ft.effectClass.OnTurnStart(),
                nameof(ft.effectClass.OnTurnStart)
            );
        }
    }
}
