using UnityEngine;

public enum FlowerTileType
{
    None,
    ComboChecker, // Increases damage of a hand based on the type of hand played (Pair, Set, Run, etc)
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;
}
