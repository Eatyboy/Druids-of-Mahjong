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
    [SerializeField] private FlowerTile flowerTilePrefab;

    [Header("Data")]
    public List<FlowerTile> playerFlowerTiles = new();

    [Header("Testing")]
    [SerializeField] private FlowerTileData testData;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    public void AddFlowerTile(FlowerTileData flowerTileData)
    {
        FlowerTile addedFlowerTile = Instantiate(flowerTilePrefab, flowerTileContainer);
        addedFlowerTile.Initialize(flowerTileData, infoController);
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
            AddFlowerTile(testData);
        }
    }

    public void ActivateFlowerTilesOnPlay()
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.actionQueue.Enqueue(() => ft.effectClass.OnPlayHand(PlayerHand.instance.GetPlayerHandTileData(), PlayerHand.instance.GetSelectedTileData()));
        }
    }

    public void ActivateFlowerTilesOnIncomingDamage(int dmg)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.actionQueue.Enqueue(() => ft.effectClass.OnIncomingAttack(dmg));
        }
    }

    public void ActivateFlowerTilesOnTakeDamage(int dmg)
    {
        foreach (FlowerTile ft in playerFlowerTiles)
        {
            CombatManager.instance.actionQueue.Enqueue(() => ft.effectClass.OnTakeDamage(dmg));
        }
    }
}
