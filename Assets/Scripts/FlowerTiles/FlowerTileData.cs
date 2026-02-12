using UnityEngine;

public enum FlowerTileType
{
    None,
    FlatDamageBuff
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;

    [Header("Stats")]
    public int flatDamageBonus;
}
